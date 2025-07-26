using System.CommandLine;

namespace AdrTool.Cmd.Commands;

internal class ConfigCommand : CommandBase
{
    private readonly Argument<string> keyArgument = new (null, description: "Key of the config");
    private readonly Argument<string?> valueArgument = new (null, description: "Value of the config");

    public ConfigCommand()
        : base("config", "Set a config")
    {
        AddArgument(keyArgument);
        AddArgument(valueArgument);
        this.SetHandler(Execute, keyArgument, valueArgument, FolderOption);
    }

    private void Execute(string key, string? value, DirectoryInfo? directory)
        => Process(
            directory,
            static (mgr, param) => mgr.SetConfigAsync(param.key, param.value).GetAwaiter().GetResult(),
            new { key, value });
}