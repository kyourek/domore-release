using System.IO;
using System.Linq;

using FILE = System.IO.File;
using PATH = System.IO.Path;

namespace Domore.Conventions {
    internal class Project {
        public string Root { get; }
        public string Extension { get; }

        public string Name =>
            PATH.GetFileName(Root);

        public string Path =>
            PATH.Combine(Root, Name + Extension);

        public string BinPath =>
            PATH.Combine(Root, "bin");

        public bool Exists =>
            FILE.Exists(Path);

        public Project(string root, string extension) {
            Root = root;
            Extension = extension;
        }

        public class Setup : Project {
            public string MsiPath =>
                Directory
                    .GetFiles(BinPath, "*.msi")
                    .OrderByDescending(path => FILE.GetCreationTimeUtc(path))
                    .First();

            public string MsiName =>
                PATH.GetFileName(MsiPath);

            public string Version =>
                MsiName.Split('-').Last().Replace(".msi", "");

            public Setup(string root) : base(root, ".wixproj") {
            }
        }
    }
}
