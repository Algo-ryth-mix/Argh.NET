using Argh.NET;

namespace Argh.Sample;

public static class App
{
    public static int Main(string[] args)
    {
        return Runner.RunClass(typeof(App),args);
    }

    [ArghCommand(Verb = "build", Description = "Builds the solution")]
    public static int RunBuild(
        [ArghParameter(Description = "the path to look for the solution")] string solutionPath,
        [ArghOption(Name = "--configuration", Description = "Build configuration", Aliases = "-c")] string configuration = "Release",
        [ArghOption(Name = "--verbose", Description = "Enable verbose logging", Aliases = "-v")] bool verbose = false)
    {
        Console.WriteLine($"Building solution at {solutionPath}");
        Console.WriteLine($"Configuration: {configuration}");
        Console.WriteLine($"Verbose: {verbose}");
        return 0;
    }

    [ArghCommand(Verb = "test", Description = "Runs the tests", Aliases = "t,tests")]
    public static int RunTests(
        [ArghParameter(Description = "the path to look for the tests")] string testPath,
        [ArghParameter(Description = "the test filter")] string filter = "*",
        [ArghOption(Name = "--parallel", Description = "Run tests in parallel", Aliases = "-p")] bool parallel = false,
        [ArghOption(Name = "--max-threads", Description = "Maximum number of threads")] int maxThreads = 4)
    {
        Console.WriteLine($"Running tests at {testPath} with filter {filter}");
        Console.WriteLine($"Parallel: {parallel}");
        Console.WriteLine($"Max threads: {maxThreads}");
        return 0;
    }

    [ArghCommand(Verb = "deploy", Description = "Deploys the application")]
    public static int RunDeploy(
        [ArghParameter(Description = "the deployment target")] string target,
        [ArghOption(Name = "--environment", Description = "Deployment environment", Aliases = "-e,--env")] string environment = "staging",
        [ArghOption(Name = "--dry-run", Description = "Perform a dry run without actual deployment")] bool dryRun = false,
        [ArghOption(Name = "--timeout", Description = "Deployment timeout in seconds", Aliases = "-t")] int timeout = 300)
    {
        Console.WriteLine($"Deploying to {target}");
        Console.WriteLine($"Environment: {environment}");
        Console.WriteLine($"Dry run: {dryRun}");
        Console.WriteLine($"Timeout: {timeout}s");
        return 0;
    }
}