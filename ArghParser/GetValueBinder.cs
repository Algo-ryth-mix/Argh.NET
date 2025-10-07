using System.CommandLine;

namespace Argh.NET
{
    internal static class GetValueBinder<T>
    {
        public static T GetValue(ParseResult result, Option<T> option)
        {
            return result.GetValue(option);
        }
    
        public static T GetValue(ParseResult result, Argument<T> argument)
        {
            return result.GetValue(argument);
        }
    }
}