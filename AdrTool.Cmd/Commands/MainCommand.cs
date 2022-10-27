using System.CommandLine;

namespace SwissLife.Slkv.Tools.ArchitectureDecisionRecord.Commands
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