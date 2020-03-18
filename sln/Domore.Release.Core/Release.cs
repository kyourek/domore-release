using System;
using System.Collections.Generic;


namespace Domore {
    using ReleaseActions;
    using CONF = Conf.Conf;

    public class Release {
        public Release(IEnumerable<string> input) {
            var cmd = new ReleaseCommand(input);
            var stage = cmd.Arg("Stage");
            var release = cmd.Arg("Release") == "yes";
            if (release == false && string.IsNullOrWhiteSpace(stage)) {
                throw new InvalidOperationException("Must either stage or release");
            }

            var solutionDirectory = cmd.Arg("SolutionDirectory");
            var context = new ReleaseContext();
            var actions = new ReleaseAction[] {
                new Bump(),
                new Build(),
                new Tag(),
                new Push()
            };

            foreach (var action in actions) {
                CONF.Configure(action, "ReleaseAction");
                CONF.Configure(action);

                if (cmd.Arg(action.GetType().Name) != "no") {
                    action.Context = context;
                    action.Stage = stage;
                    action.SolutionDirectory = solutionDirectory;
                    action.Work();
                }
            }
        }
    }
}
