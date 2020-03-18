using System;
using System.IO;

namespace Domore.ReleaseActions {
    internal class Clone : ReleaseAction {
        public string Url { get; set; }
        public string Stage { get; set; }

        public string Branch {
            get => _Branch ?? (_Branch = "master");
            set => _Branch = value;
        }
        private string _Branch;

        public override void Work() {
            var dirInfo = new DirectoryInfo(Solution.Parent);
            if (dirInfo.Exists) {
                dirInfo.Delete(recursive: true);
            }
            dirInfo.Create();
            Process("git", "clone", Url, dirInfo.FullName);
            Process("git", "checkout", Branch);

            Solution.SetRepository(Url, Branch, Process("git", "rev-parse", "HEAD"));
        }
    }
}
