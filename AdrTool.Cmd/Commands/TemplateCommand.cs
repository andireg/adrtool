using System.CommandLine;

namespace AdrTool.Cmd.Commands
{
    internal class TemplateCommand : Command
    {
        public TemplateCommand()
            : base("template", "Manages templates")
        {
            AddCommand(new ListTemplateCommand());
            AddCommand(new DefaultTemplateCommand());
            AddCommand(new AddTemplateCommand());
            AddCommand(new RemoveTemplateCommand());
        }
    }
}