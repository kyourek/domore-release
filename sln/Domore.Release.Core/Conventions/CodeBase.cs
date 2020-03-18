using System;
using PATH = System.IO.Path;

namespace Domore.Conventions {
    internal class CodeBase {
        public string Root { get; }
        public string Path { get; }

        public CodeBase(string root) {
            Root = root;
            Path = PATH.IsPathRooted(Root)
                ? Root
                : PATH.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData, Environment.SpecialFolderOption.None),
                    "Domore",
                    "Release",
                    Root);
        }
    }
}
