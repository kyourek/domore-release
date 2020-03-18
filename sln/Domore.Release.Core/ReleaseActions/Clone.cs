using System;
using System.IO;

namespace Domore.ReleaseActions {
    internal class Clone : ReleaseAction {
        private void Delete(DirectoryInfo info) {
            if (null == info) throw new ArgumentNullException(nameof(info));

            foreach (var dir in info.GetDirectories()) {
                Delete(dir);
            }

            foreach (var fil in info.GetFiles()) {
                Log.Info("Deleting file " + fil.FullName + "...");
                fil.Attributes = FileAttributes.Normal;
                fil.Delete();
            }

            Log.Info("Deleting directory " + info.FullName + "...");

            info.Attributes = FileAttributes.Normal;
            info.Delete(true);
        }

        public string Url { get; set; }
        public string Stage { get; set; }

        public string Branch {
            get => _Branch ?? (_Branch = "master");
            set => _Branch = value;
        }
        private string _Branch;

        public override void Work() {
            var dirInfo = new DirectoryInfo(Solution.Root);
            if (dirInfo.Exists) {
                Delete(dirInfo);
            }
            dirInfo.Create();
            Process("git", "clone", Url, dirInfo.FullName);
            Process("git", "checkout", Branch);

            var thisVersion = Solution.GetVersion(Stage);
            var nextVersion = thisVersion.NextBuild();

            Solution.SetVersion(nextVersion);
            Solution.SetRepository(Url, Branch, Process("git", "rev-parse", "HEAD"));
        }
    }
}
