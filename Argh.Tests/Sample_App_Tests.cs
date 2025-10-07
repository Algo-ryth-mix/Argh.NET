using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Argh.NET;
using Argh.Sample;

namespace Argh.Tests
{
    [TestClass]
    public class Sample_App_Tests
    {
        [TestMethod]
        public void Build_Command_Should_Return_0()
        {
            var exit = Runner.RunClass(typeof(App), new[] { "build", ".", "--configuration", "Debug", "--verbose" });
            exit.ShouldBe(0);
        }

        [TestMethod]
        public void Test_Command_Should_Parse_All_Params_And_Options()
        {
            var exit = Runner.RunClass(typeof(App), new[] { "test", ".", "MyFilter", "--parallel", "--max-threads", "8" });
            exit.ShouldBe(0);
        }

        [TestMethod]
        public void Deploy_Command_Should_Handle_Aliases_And_Defaults()
        {
            var exit = Runner.RunClass(typeof(App), new[] { "deploy", "prod", "-e", "production", "--dry-run", "-t", "30" });
            exit.ShouldBe(0);
        }
    }
}
