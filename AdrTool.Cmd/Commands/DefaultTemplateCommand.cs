using System.CommandLine;

namespace AdrTool.Cmd.Commands
{
    internal class DefaultTemplateCommand : CommandBase
    {
        private readonly Argument<string> nameArgument = new Argument<string>(null, description: "Name of the template");

        public DefaultTemplateCommand()
            : base("default", "Sets a new default template")
        {
            AddArgument(nameArgument);
            this.SetHandler(Execute, nameArgument, FolderOption);
        }

        private void Execute(string name, DirectoryInfo? directory)
            => Process(directory, static (mgr, nameParam) => mgr.SetDefaultTemplate(nameParam), name);
    }
}