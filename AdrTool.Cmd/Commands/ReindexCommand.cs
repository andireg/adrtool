﻿using System.CommandLine;

namespace SwissLife.Slkv.Tools.ArchitectureDecisionRecord.Commands
{
    internal class ReindexCommand : CommandBase
    {
        public ReindexCommand()
            : base("reindex", "Performs a re-index")
        {
            this.SetHandler(Execute, FolderOption);
        }

        private void Execute(DirectoryInfo? directory)
            => Process(directory, static mgr => mgr.ReindexAsync());
    }
}