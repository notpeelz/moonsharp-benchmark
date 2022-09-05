using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;
using System.Diagnostics;
using System.Runtime.InteropServices;

static (bool Success, string Output, string Error) TryRunGitCommand(string args)
{
    static string GetGitBinary()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            return Environment.GetEnvironmentVariable("PATH", EnvironmentVariableTarget.Process)
                ?.Split(';')
                .Select(x => Path.Join(x, "git.exe"))
                .FirstOrDefault(File.Exists);
        }
        else
        {
            return Environment.GetEnvironmentVariable("PATH", EnvironmentVariableTarget.Process)
                ?.Split(':')
                .Select(x => Path.Join(x, "git"))
                .FirstOrDefault(File.Exists);
        }
    }

    var gitBinary = GetGitBinary();
    if (gitBinary == null)
    {
        throw new InvalidOperationException("Failed to find git binary in PATH");
    }

    using var process = Process.Start(new ProcessStartInfo(gitBinary, args)
    {
        WindowStyle = ProcessWindowStyle.Hidden,
        CreateNoWindow = true,
        UseShellExecute = false,
        RedirectStandardInput = true,
        RedirectStandardError = true,
        RedirectStandardOutput = true,
    });

    if (process == null)
        throw new InvalidOperationException($"Failed to run git command: {args}");

    process.Start();

    var stdOut = process.StandardOutput.ReadToEndAsync();
    var stdErr = process.StandardError.ReadToEndAsync();
    Task.WhenAll(stdOut, stdErr).GetAwaiter().GetResult();
    process.WaitForExit();

    return (process.ExitCode == 0, stdOut.Result.TrimEnd('\r', '\n'), stdErr.Result);
}

var gitDir = new Func<string>(() => {
    var (success, gitDir, error) = TryRunGitCommand("rev-parse --show-toplevel");
    if (!success)
    {
        throw new InvalidOperationException($"Failed to determine the root of the git repo: {error}");
    }

    return gitDir;
})();

var job = Job.LongRun
    .WithArguments(new Argument[]
    {
        new MsBuildArgument($"/p:RestorePackagesPath={Path.Join(gitDir, "nuget-pkgs/cache")}"),
        new MsBuildArgument($"/p:RestoreSources={Path.Join(gitDir, "nuget-pkgs/src")}"),
    });

var config = ManualConfig.Create(DefaultConfig.Instance).AddJob(job);

BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args, config);