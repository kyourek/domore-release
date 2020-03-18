using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

using FILE = System.IO.File;
using PATH = System.IO.Path;

namespace Domore.Conventions {
    internal class Solution {
        public string Parent { get; }
        public string Root => PATH.Combine(Parent, "sln");
        public string Properties => PATH.Combine(Root, "Directory.Build.props");
        public string Name => PATH.GetFileName(Parent);
        public string Path => PATH.Combine(Root, $"{Name}.sln");
        public string File => PATH.GetFileName(Path);
        public Project.Setup Setup => new Project.Setup(PATH.Combine(Root, $"{Name}-setup"));
        public IEnumerable<Project> Tests => Projects.Where(project => project.Name.EndsWith("Tests"));
        public string SetupFile => Directory
            .GetFiles(Setup.Root, "*.msi")
            .OrderByDescending(path => FILE.GetCreationTimeUtc(path))
            .First();

        public IEnumerable<Project> Projects {
            get {
                var directories = Directory.GetDirectories(Root);
                foreach (var directory in directories) {
                    var extensions = new[] { ".csproj", ".vbp", ".wixproj" };
                    foreach (var extension in extensions) {
                        var project = new Project(directory, extension);
                        if (project.Exists) {
                            yield return project;
                            break;
                        }
                    }
                }
            }
        }

        public Solution(string parent) {
            Parent = parent;
        }

        public Version GetVersion(string stage) {
            var propsPath = Properties;
            var propsXDoc = XDocument.Load(propsPath);
            var propertyGroup = propsXDoc.Root.Element("PropertyGroup");

            var fileVersion = propertyGroup.Element("FileVersion").Value;
            var fullVersion = Version.ParseFileVersion(fileVersion, stage);

            return fullVersion;
        }

        public void SetVersion(Version value) {
            var propsPath = Properties;
            var propsXDoc = XDocument.Load(propsPath);
            var propGroup = propsXDoc.Root.Element("PropertyGroup");

            propGroup.Element("VersionPrefix").Value = value.VersionPrefix;
            propGroup.Element("VersionSuffix").Value = value.VersionSuffix;
            propGroup.Element("AssemblyVersion").Value = value.AssemblyVersion;
            propGroup.Element("InformationalVersion").Value = value.InformationalVersion;
            propGroup.Element("FileVersion").Value = value.FileVersion;
            propGroup.Element("PackageVersion").Value = value.PackageVersion;

            propsXDoc.Save(propsPath);
        }

        public void SetRepository(string url, string branch, string commit) {
            var propsPath = Properties;
            var propsXDoc = XDocument.Load(propsPath);
            var propGroup = propsXDoc.Root.Element("PropertyGroup");

            propGroup.Element("RepositoryUrl").Value = url;
            propGroup.Element("RepositoryBranch").Value = branch;
            propGroup.Element("RepositoryCommit").Value = commit;

            propsXDoc.Save(propsPath);
        }
    }
}
