using System.CommandLine;

namespace AdrTool.Cmd.Commands
{
    internal class MainCommand : RootCommand
    {
        public MainCommand()
        {
            Description = "Manages youd architecure decision records";

            AddCommand(new InitCommand());
            AddCommand(new AddCommand());
            AddCommand(new ReindexCommand());
            AddCommand(new TemplateCommand());
        }
    }
}