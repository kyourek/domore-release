using System;
using System.Linq;
using PATH = System.IO.Path;

namespace Domore.Conventions {
    public class CodeBase {
        public string Repository { get; }

        public string Name =>
            _Name ?? (
            _Name = PATH.GetFileNameWithoutExtension(Repository.Split('/').Last()));
        private string _Name;

        public string Path =>
            _Path ?? (
            _Path = PATH.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData, Environment.SpecialFolderOption.None),
                "Domore",
                "Release",
                Name));
        private string _Path;

        public Solution Solution =>
            _Solution ?? (
            _Solution = new Solution(Name, PATH.Combine(Path, "sln")));
        private Solution _Solution;

        public CodeBase(string repository) {
            Repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }
    }
}
