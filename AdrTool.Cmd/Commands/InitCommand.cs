using System.CommandLine;

namespace AdrTool.Cmd.Commands
{
    internal class InitCommand : CommandBase
    {
        public InitCommand()
            : base("init", "Initialize a directory")
        {
            this.SetHandler(Execute, FolderOption);
        }

        private void Execute(DirectoryInfo? directory)
            => Process(directory, static mgr => mgr.InitAsync().GetAwaiter().GetResult());
    }
}