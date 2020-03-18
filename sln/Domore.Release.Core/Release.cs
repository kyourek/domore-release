using System;
using System.Collections.Generic;


namespace Domore {
    using ReleaseActions;
    using CONF = Conf.Conf;

    public class Release {
        public Release(IEnumerable<string> input) {
            var command = new ReleaseCommand(input);
            var actions = new ReleaseAction[] {
                new Clone(),
                new Bump(),
                new Restore(),
                new Build(),
                new Pack(),
                new Tag(),
                new Push()
            };

            foreach (var action in actions) {
                CONF.Configure(action, "");
                CONF.Configure(action, "ReleaseAction");
                CONF.Configure(action);

                command.Configure(action, "");
                command.Configure(action, "ReleaseAction");
                command.Configure(action);

                action.Work();
            }
        }
    }
}
