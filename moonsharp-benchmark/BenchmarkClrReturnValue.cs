extern alias MoonSharpA;
extern alias MoonSharpB;

using BenchmarkDotNet.Attributes;

namespace MoonSharpBenchmark
{
    public class BenchmarkClrReturnValue
    {
        private class Blah
        {
            public long Test(ushort i)
            {
                return i;
            }
        }

        private MoonSharpA::MoonSharp.Interpreter.Script scriptA;
        private MoonSharpB::MoonSharp.Interpreter.Script scriptB;
        private const string src = @"
            local b = Blah.__new()
            return b.Test(5) + 1
        ";

        [GlobalSetup]
        public void GlobalSetup()
        {
            MoonSharpA::MoonSharp.Interpreter.UserData.DefaultAccessMode = MoonSharpA::MoonSharp.Interpreter.InteropAccessMode.Reflection;
            MoonSharpA::MoonSharp.Interpreter.UserData.RegisterType<Blah>();
            scriptA = new MoonSharpA::MoonSharp.Interpreter.Script();
            scriptA.Globals["Blah"] = MoonSharpA::MoonSharp.Interpreter.UserData.CreateStatic<Blah>();

            MoonSharpB::MoonSharp.Interpreter.UserData.DefaultAccessMode = MoonSharpB::MoonSharp.Interpreter.InteropAccessMode.Reflection;
            MoonSharpB::MoonSharp.Interpreter.UserData.RegisterType<Blah>();
            scriptB = new MoonSharpB::MoonSharp.Interpreter.Script();
            scriptB.Globals["Blah"] = MoonSharpB::MoonSharp.Interpreter.UserData.CreateStatic<Blah>();
        }

        [Benchmark(Baseline = true)]
        public void TestBefore() => scriptA.DoString(src);

        [Benchmark]
        public void TestAfter() => scriptB.DoString(src);

        [IterationSetup]
        public void IterationSetup() { }

        [IterationCleanup]
        public void IterationCleanup() { }

        [GlobalCleanup]
        public void GlobalCleanup() { }
    }
}
