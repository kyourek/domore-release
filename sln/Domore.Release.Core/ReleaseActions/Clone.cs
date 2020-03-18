using System;
using System.IO;

namespace Domore.ReleaseActions {
    internal class Clone : ReleaseAction {
        private void Delete(DirectoryInfo info) {
            if (null == info) throw new ArgumentNullException(nameof(info));

            if (info.Exists) {
                info.Attributes = FileAttributes.Normal;

                foreach (var directory in info.GetDirectories()) {
                    Delete(directory);
                }

                foreach (var file in info.GetFiles()) {
                    if (file.Exists) {
                        file.Attributes = FileAttributes.Normal;
                    }
                }

                info.Delete(recursive: true);
            }
        }

        public string Stage { get; set; }

        public string Branch {
            get => _Branch ?? (_Branch = "master");
            set => _Branch = value;
        }
        private string _Branch;

        public override void Work() {
            var path = CodeBase.Path;
            var repo = CodeBase.Repository;

            var dirInfo = new DirectoryInfo(path);
            if (dirInfo.Exists) {
                Delete(dirInfo);
            }
            dirInfo.Create();

            Process("git", "clone", repo, path);
            Process("git", "checkout", Branch);

            Solution.SetRepository(repo, Branch, Process("git", "rev-parse", "HEAD"));
        }
    }
}
