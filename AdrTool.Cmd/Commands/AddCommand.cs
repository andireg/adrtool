using System.CommandLine;

namespace SwissLife.Slkv.Tools.ArchitectureDecisionRecord.Commands
{
    internal class AddCommand : CommandBase
    {
        private readonly Argument<string> titleArgument = new Argument<string>(null, description: "Title of the record");
        private readonly Option<string?> templateOption = new("--template", "Template to use. If not given, the default template is used.");

        public AddCommand()
            : base("add", "Adds a new record")
        {
            AddOption(templateOption);
            AddArgument(titleArgument);
            this.SetHandler(Execute, titleArgument, FolderOption, templateOption);
        }

        private void Execute(string title, DirectoryInfo? directory, string? template)
            => Process(directory, static (mgr, param) => mgr.AddRecordAsync(param.title, param.template), new { title, template });
    }
}