using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Parsing;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Argh.NET.Helpers;

namespace Argh.NET
{
    public static class Runner
    {
        public static void RunClass<T>(string[] args = null, string rootCommandDescription = "")
            where T : class => RunClass(typeof(T), args, rootCommandDescription);


        public static int RunClass(Type @classType, string[] args, string rootCommandDescription = "")
        {
            args = args ?? new string[] { };

            var methodsWithAttributes = CommandDiscovery.DiscoverCommands(@classType).ToList();

            RootCommand root = new RootCommand(rootCommandDescription);

            foreach (var spec in methodsWithAttributes)
            {
                Command command = new Command(spec.Attribute.Verb, spec.Attribute.Description);
                SymbolFactory.AddAlias(spec.Attribute.Aliases, command.Aliases);

                // Process all parameters (both arguments and options) using the ParameterProcessor helper
                var symbols = ParameterProcessor.Process(spec.Method.GetParameters()).ToList();
                
                foreach (var symbolWrapper in symbols)
                {
                    if (symbolWrapper.IsArgument)
                    {
                        command.Arguments.Add(symbolWrapper.AsArgument);
                    }
                    else if (symbolWrapper.IsOption)
                    {
                        command.Options.Add(symbolWrapper.AsOption);
                    }
                }

                command.SetAction(result =>
                {
                    var values = symbols.Select(x => x.ValueGetter(result)).ToArray();
                    return (int)spec.Method.Invoke(null, values);
                });
                root.Add(command);
            }

            return CommandLineParser.Parse(root, args).Invoke();
        }
    }
}