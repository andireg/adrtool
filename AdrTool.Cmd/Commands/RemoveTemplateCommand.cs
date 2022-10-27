using System.CommandLine;

namespace AdrTool.Cmd.Commands
{
    internal class RemoveTemplateCommand : CommandBase
    {
        private readonly Argument<string> nameArgument = new (null, description: "Name of the template");

        public RemoveTemplateCommand()
            : base("remove", "Removes a template")
        {
            AddArgument(nameArgument);
            this.SetHandler(Execute, nameArgument, FolderOption);
        }

        private void Execute(string name, DirectoryInfo? directory)
            => Process(directory, static (mgr, nameParam) => mgr.RemoveTemplate(nameParam), name);
    }
}