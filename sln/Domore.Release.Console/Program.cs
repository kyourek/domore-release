using System;

namespace Domore {
    using Conf;
    using CONF = Conf.Conf;

    internal class Program {
        private static void Main(string[] args) =>
            Run(args);

        public static void Run(string[] args) {
            try {
                CONF.Container.ContentsProvider = new AppSettingsProvider();
                CONF.Container.ConfigureLogging();
                new Release(new ReleaseCommand(args));
            }
            catch (Exception ex) {
                Console.WriteLine(ex);
            }
        }
    }
}
