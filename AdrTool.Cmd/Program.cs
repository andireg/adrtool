using System.CommandLine;
using AdrTool.Cmd.Commands;

await new MainCommand().InvokeAsync(Environment.GetCommandLineArgs().Skip(1).ToArray());