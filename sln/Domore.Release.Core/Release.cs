using System;
using System.Collections.Generic;

namespace Domore {
    using Conventions;
    using ReleaseActions;
    using CONF = Conf.Conf;

    public class Release {
        public ReleaseCommand Command { get; }
        public string Repository { get; set; }

        public IEnumerable<ReleaseAction> Actions =>
            _Actions ?? (
            _Actions = new ReleaseAction[] {
                new Clone(),
                new Bump(),
                new Restore(),
                new Build(),
                new Pack(),
                new Tag(),
                new Push() });
        private IEnumerable<ReleaseAction> _Actions;

        public Release(ReleaseCommand command) {
            Command = command ?? throw new ArgumentNullException(nameof(command));

            T configure<T>(T obj) {
                CONF.Configure(obj, "");
                CONF.Configure(obj);

                Command.Configure(obj, "");
                Command.Configure(obj);

                return obj;
            }

            var input = configure(this);
            var codeBase = new CodeBase(input.Repository);
            var solution = codeBase.Solution;

            foreach (var action in Actions) {
                configure(action);
                action.CodeBase = codeBase;
                action.Solution = solution;
                action.Work();
            }
        }
    }
}
