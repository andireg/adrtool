using System.CommandLine;

namespace SwissLife.Slkv.Tools.ArchitectureDecisionRecord.Commands
{
    internal class AddTemplateCommand : CommandBase
    {
        private readonly Argument<string> nameArgument = new(null, description: "Name of the template");

        public AddTemplateCommand()
            : base("add", "Adds a template")
        {
            AddArgument(nameArgument);
            this.SetHandler(Execute, nameArgument, FolderOption);
        }

        private void Execute(string name, DirectoryInfo? directory)
            => Process(directory, static (mgr, nameParam) => mgr.AddTemplate(nameParam), name);
    }
}