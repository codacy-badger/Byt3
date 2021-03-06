﻿using Byt3.ADL;
using Byt3.CommandRunner;
using Byt3.Utilities.DotNet;

namespace Byt3.AssemblyGenerator.CLI.Commands
{
    public class SetTargetRuntimeCommand : AbstractCommand
    {
        public SetTargetRuntimeCommand() : base(new[] {"--set-target-runtime", "-sruntime"},
            "Sets the Assembly name. Default: TestAssembly")
        {
            CommandAction = SetTargetRuntime;
        }

        private void SetTargetRuntime(StartupArgumentInfo argumentInfo, string[] args)
        {
            if (!Program.HasTarget)
            {
                Logger.Log(LogType.Error, "You need to specify a target config");
                return;
            }

            if (args.Length != 1)
            {
                Logger.Log(LogType.Error, "Only 1 argument allowed.");
                return;
            }

            AssemblyDefinition definition = AssemblyDefinition.Load(Program.Target);

            Logger.Log(LogType.Log, "Setting Target Runtime: " + args[0]);
            definition.NoTargetRuntime = args[0].ToLower() == "none";
            definition.BuildTargetRuntime = args[0];

            AssemblyDefinition.Save(Program.Target, definition);
        }
    }
}