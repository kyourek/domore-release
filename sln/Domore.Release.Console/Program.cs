using System;

namespace Domore {
    internal class Program {
        private static void Main(string[] args) {
            try {
                new Release(args);
            }
            catch (Exception ex) {
                Console.WriteLine(ex);
            }
        }
    }
}
