using System;
using System.Collections.Generic;
using System.Linq;

namespace Domore {
    internal class ReleaseCommand {
        public IEnumerable<string> Input { get; }
        public IDictionary<string, string> Argument { get; }

        public ReleaseCommand(IEnumerable<string> input) {
            Input = input;
            Argument = (Input ?? new string[] { })
                .Where(inp => inp.StartsWith("-"))
                .Select(inp => inp.TrimStart('-'))
                .Select(inp => inp.Split(new[] { '=' }, 2, StringSplitOptions.RemoveEmptyEntries))
                .ToDictionary(
                    pair => pair[0],
                    pair => pair.Length > 1
                        ? pair[1]
                        : null);
        }

        public string Arg(string name) =>
            Argument.TryGetValue(name, out var value)
                ? value
                : null;
    }
}
