extern alias MoonSharpA;
extern alias MoonSharpB;

using BenchmarkDotNet.Attributes;

namespace MoonSharpBenchmark
{
    public class BenchmarkCallWithLuaNumber
    {
        private class Blah
        {
            public void Test(byte i) { }
        }

        private MoonSharpA::MoonSharp.Interpreter.Script scriptA;
        private MoonSharpB::MoonSharp.Interpreter.Script scriptB;
        private const string src = @"
            local b = Blah.__new()
            local x = 5
            return b.Test(x)
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
