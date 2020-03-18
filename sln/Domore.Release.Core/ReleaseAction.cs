using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Domore {
    using Conventions;
    using Logs;

    internal abstract class ReleaseAction {
        protected ILog Log =>
            _Log ?? (
            _Log = Logging.For(this));
        private ILog _Log;

        protected CodeBase CodeBase =>
            _CodeBase ?? (
            _CodeBase = new CodeBase(Root));
        private CodeBase _CodeBase;

        protected Solution Solution =>
            _Solution ?? (
            _Solution = new Solution(CodeBase.Path));
        private Solution _Solution;

        protected string Process(string fileName, params string[] arguments) {
            var outp = "";
            var args = string.Join(" ", arguments);
            Log.Info($"{fileName} {args}");

            if (ProcessPath.TryGetValue(fileName, out string processPath)) {
                fileName = processPath;
                Log.Info($"Using {fileName}");
            }

            void errorDataReceived(object sender, DataReceivedEventArgs e) {
                var data = e?.Data;
                if (data != null) {
                    Log.Warn(data);
                }
            }

            void outputDataReceived(object sender, DataReceivedEventArgs e) {
                var data = e?.Data;
                if (data != null) {
                    outp += data;
                    Log.Info(data);
                }
            }

            using (var process = new Process()) {
                process.StartInfo.FileName = fileName;
                process.StartInfo.Arguments = args;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardError = true;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.WorkingDirectory = Solution.Parent;
                process.ErrorDataReceived += errorDataReceived;
                process.OutputDataReceived += outputDataReceived;
                process.Start();
                process.BeginErrorReadLine();
                process.BeginOutputReadLine();
                process.WaitForExit();

                var exitCode = process.ExitCode;
                if (exitCode != 0) throw new Exception("Process error (exit code '" + exitCode + "')");
            }

            return outp;
        }

        public string Root { get; set; }

        public IDictionary<string, string> ProcessPath {
            get => _ProcessPath ?? (_ProcessPath = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase));
            set => _ProcessPath = value;
        }
        private IDictionary<string, string> _ProcessPath;

        public abstract void Work();
    }
}
