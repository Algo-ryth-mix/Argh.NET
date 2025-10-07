using System;
using System.CommandLine;

namespace Argh.NET
{
    public class SymbolWrapper
    {
        private Symbol Symbol { get; }
        public Func<ParseResult, object> ValueGetter { get; }
        
        public bool IsArgument => Symbol is Argument;
        public bool IsOption => Symbol is Option;
        
        public Argument AsArgument => Symbol as Argument;
        public Option AsOption => Symbol as Option;
        
        public SymbolWrapper(Argument argument, Func<ParseResult, object> valueGetter)
        {
            Symbol = argument;
            ValueGetter = valueGetter;
        }
        
        public SymbolWrapper(Option option, Func<ParseResult, object> valueGetter)
        {
            Symbol = option;
            ValueGetter = valueGetter;
        }
    }
}