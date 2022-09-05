Remove-Item -Recurse -Force -ErrorAction SilentlyContinue ./moonsharp-a
Remove-Item -Recurse -Force -ErrorAction SilentlyContinue ./moonsharp-b

git -C ./moonsharp fetch origin
git -C ./moonsharp worktree remove "$PWD/moonsharp-a" 2> $null
git -C ./moonsharp worktree remove "$PWD/moonsharp-b" 2> $null
git -C ./moonsharp worktree add "$PWD/moonsharp-a" origin/add-clr-primitive-wrappers~1
git -C ./moonsharp worktree add "$PWD/moonsharp-b" origin/add-clr-primitive-wrappers

Remove-Item -Recurse -Force -ErrorAction SilentlyContinue ./nuget-pkgs/cache

$PackArgs = @(
  "-p:DebugType=pdbonly",
  "-p:DebugSymbols=true"
)
dotnet pack ./moonsharp-a/MoonSharp.Interpreter -o ./nuget-pkgs/src -p:PackageId=MoonSharpA -p:AssemblyName=MoonSharpA -c Release @PackArgs
dotnet pack ./moonsharp-b/MoonSharp.Interpreter -o ./nuget-pkgs/src -p:PackageId=MoonSharpB -p:AssemblyName=MoonSharpB -c Release @PackArgs
