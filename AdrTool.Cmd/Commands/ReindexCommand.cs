using System.CommandLine;

namespace AdrTool.Cmd.Commands
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