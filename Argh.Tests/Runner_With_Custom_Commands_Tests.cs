using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Argh.NET;
using Argh;
using NSubstitute;

namespace Argh.Tests
{
    [TestClass]
    public class Runner_With_Custom_Commands_Tests
    {
        public interface ILogger
        {
            void Log(string message);
        }

        public static class TestCommands
        {
            public static ILogger Logger = new ConsoleLogger();

            private class ConsoleLogger : ILogger
            {
                public void Log(string message) => Console.WriteLine(message);
            }

            [ArghCommand(Verb = "echo", Description = "echo text" , Aliases = "e")]
            public static int Echo(
                [ArghParameter(Description = "text to echo")] string text,
                [ArghOption(Name = "--times", Description = "repeat times", Aliases = "-t")] int times = 1,
                [ArghOption(Name = "--loud", Description = "uppercase", Aliases = "-l")] bool loud = false)
            {
                var msg = loud ? text.ToUpperInvariant() : text;
                for (var i = 0; i < times; i++)
                {
                    Logger.Log(msg);
                }
                return times;
            }

            [ArghCommand(Verb = "sum", Description = "sum numbers")]
            public static int Sum(
                [ArghParameter(Description = "first")] int a,
                [ArghParameter(Description = "second")] int b,
                [ArghOption(Name = "--inc", Description = "increment result")] bool inc = false)
            {
                var result = a + b;
                return inc ? result + 1 : result;
            }
        }

        [TestMethod]
        public void RunClass_Should_Invoke_Command_With_Arguments_And_Options()
        {
            var args = new[] { "echo", "hello", "--times", "3", "--loud" };
            var exit = Runner.RunClass(typeof(TestCommands), args);
            exit.ShouldBe(3);
        }

        [TestMethod]
        public void RunClass_Should_Support_Command_Alias()
        {
            var args = new[] { "e", "hello" };
            var exit = Runner.RunClass(typeof(TestCommands), args);
            exit.ShouldBe(1);
        }

        [TestMethod]
        public void RunClass_Generic_Should_Work()
        {
            var args = new[] { "sum", "2", "3", "--inc" };
            var exit = Runner.RunClass(typeof(TestCommands), args);
            exit.ShouldBe(6);
        }

        [TestMethod]
        public void Options_Defaults_Should_Apply_When_Not_Specified()
        {
            var args = new[] { "echo", "hi" };
            var exit = Runner.RunClass(typeof(TestCommands), args);
            exit.ShouldBe(1);
        }

        [TestMethod]
        public void Should_Capture_Console_Output_From_Command()
        {
            var sw = new StringWriter();
            var original = Console.Out;
            try
            {
                Console.SetOut(sw);
                var args = new[] { "echo", "hello", "-t", "2" };
                Runner.RunClass(typeof(TestCommands), args).ShouldBe(2);
                var output = sw.ToString();
                output.ShouldContain("hello");
            }
            finally
            {
                Console.SetOut(original);
            }
        }

        [TestMethod]
        public void Echo_Should_Log_Specified_Number_Of_Times_With_Message()
        {
            var previous = TestCommands.Logger;
            try
            {
                var substitute = Substitute.For<ILogger>();
                TestCommands.Logger = substitute;

                var args = new[] { "echo", "hello", "--times", "2" };
                var exit = Runner.RunClass(typeof(TestCommands), args);
                exit.ShouldBe(2);

                substitute.Received(2).Log("hello");
            }
            finally
            {
                TestCommands.Logger = previous;
            }
        }

        [TestMethod]
        public void Echo_Should_Log_Uppercase_When_Loud()
        {
            var previous = TestCommands.Logger;
            try
            {
                var substitute = Substitute.For<ILogger>();
                TestCommands.Logger = substitute;

                var args = new[] { "echo", "hello", "--loud" };
                var exit = Runner.RunClass(typeof(TestCommands), args);
                exit.ShouldBe(1);

                substitute.Received(1).Log("HELLO");
            }
            finally
            {
                TestCommands.Logger = previous;
            }
        }

        [TestMethod]
        public void Alias_E_Should_Log_Once()
        {
            var previous = TestCommands.Logger;
            try
            {
                var substitute = Substitute.For<ILogger>();
                TestCommands.Logger = substitute;

                var args = new[] { "e", "hi" };
                var exit = Runner.RunClass(typeof(TestCommands), args);
                exit.ShouldBe(1);

                substitute.Received(1).Log("hi");
            }
            finally
            {
                TestCommands.Logger = previous;
            }
        }

        [TestMethod]
        public void Sum_Should_Not_Log()
        {
            var previous = TestCommands.Logger;
            try
            {
                var substitute = Substitute.For<ILogger>();
                TestCommands.Logger = substitute;

                var args = new[] { "sum", "2", "3" };
                var exit = Runner.RunClass(typeof(TestCommands), args);
                exit.ShouldBe(5);

                substitute.DidNotReceive().Log(Arg.Any<string>());
            }
            finally
            {
                TestCommands.Logger = previous;
            }
        }
    }
}
