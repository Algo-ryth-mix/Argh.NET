# Argh.NET

Argument parsing based on method signatures, inspired by Python's [`argh`](https://argh.readthedocs.io/en/latest/) package.

## Installation

Install the NuGet package:

```sh
dotnet add package Argh.NET
```

## Example Usage

```csharp
namespace Argh.Sample;

public static class App
{
    public static int Main(string[] args)
    {
        return Runner.RunClass(typeof(App), args);
    }

    [ArghCommand(Verb = "build", Description = "Builds the solution")]
    public static int RunBuild([ArghParameter(Description = "the path to look for the solution")] string solutionPath)
    {
        Console.WriteLine($"Building solution at {solutionPath}");
        return 0;
    }

    [ArghCommand(Verb = "test", Description = "Runs the tests", Aliases = "t,tests")]
    public static int RunTests(
        [ArghParameter(Description = "the path to look for the tests")] string testPath,
        [ArghParameter(Description = "the test filter")] string filter = "*")
    {
        Console.WriteLine($"Running tests at {testPath} with filter {filter}");
        return 0;
    }
}
```

You can then run the commands like:

```sh
program.exe build .
program.exe test . "**.filtered"
```
