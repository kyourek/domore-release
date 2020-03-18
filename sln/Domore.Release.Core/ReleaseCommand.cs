using System;
using System.Collections.Generic;
using System.Linq;

namespace Domore {
    using Conf;

    public class ReleaseCommand {
        private readonly ConfContainer Conf;

        public object Content =>
            Conf.Content;

        public ReleaseCommand(object content) {
            Conf = new ConfContainer { Content = content };
        }

        public T Configure<T>(T obj, string key = null) =>
            Conf.Configure(obj, key);

        public static ReleaseCommand From(IDictionary<string, string> content) {
            if (null == content) throw new ArgumentNullException(nameof(content));
            return new ReleaseCommand(string.Join(Environment.NewLine, content.Select(pair => $"{pair.Key} = {pair.Value}")));
        }

        public static ReleaseCommand From(IEnumerable<string> content) {
            if (null == content) throw new ArgumentNullException(nameof(content));
            return From(content
                .Where(arg => arg.StartsWith("-"))
                .Where(arg => arg.Contains("="))
                .Select(arg => arg.Split(new[] { '=' }, 2))
                .ToDictionary(
                    pair => pair[0].TrimStart('-'),
                    pair => pair.Length > 1 ? pair[1] : null,
                    StringComparer.OrdinalIgnoreCase));
        }
    }
}
