using System.CommandLine;

namespace SwissLife.Slkv.Tools.ArchitectureDecisionRecord.Commands
{
    internal class ListTemplateCommand : CommandBase
    {
        public ListTemplateCommand()
            : base("list", "Lists all templates")
        {
            this.SetHandler(Execute, FolderOption);
        }

        private void Execute(DirectoryInfo? directory)
            => Process(
                directory,
                static mgr =>
                {
                    foreach (string template in mgr.ListTemplates())
                    {
                        Console.WriteLine(template);
                    }
                });
    }
}