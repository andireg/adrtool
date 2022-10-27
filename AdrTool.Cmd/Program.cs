using SwissLife.Slkv.Tools.ArchitectureDecisionRecord.Commands;
using System.CommandLine;

await new MainCommand().InvokeAsync(Environment.GetCommandLineArgs().Skip(1).ToArray());