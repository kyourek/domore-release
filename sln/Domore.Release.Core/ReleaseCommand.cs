using System;
using System.Collections.Generic;
using System.Linq;

namespace Domore {
    using Conf;

    internal class ReleaseCommand {
        private string ConfContent { get; }
        private ConfContainer ConfContainer { get; }
        private IDictionary<string, string> Argument { get; }

        public IEnumerable<string> Input { get; }

        public ReleaseCommand(IEnumerable<string> input) {
            Input = input;
            Argument = (Input ?? new string[] { })
                .Where(inp => inp.StartsWith("-"))
                .Where(inp => inp.Contains("="))
                .Select(inp => inp.Split(new[] { '=' }, 2))
                .ToDictionary(
                    pair => pair[0].TrimStart('-'),
                    pair => pair.Length > 1 ? pair[1] : null,
                    StringComparer.OrdinalIgnoreCase);
            ConfContent = string.Join(Environment.NewLine, Argument.Select(pair => $"{pair.Key} = {pair.Value}"));
            ConfContainer = new ConfContainer { Content = ConfContent };
        }

        public T Configure<T>(T obj, string key = null) =>
            ConfContainer.Configure(obj, key);
    }
}
