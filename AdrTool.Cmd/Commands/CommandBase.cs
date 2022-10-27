using System.CommandLine;
using AdrTool.Core;

namespace AdrTool.Cmd.Commands
{
    internal abstract class CommandBase : Command
    {
        protected CommandBase(string name, string? description)
            : base(name, description)
        {
            AddOption(FolderOption);
        }

        protected Option<DirectoryInfo?> FolderOption { get; } = new ("--folder", "Root-folder of the architecture decision record.");

        protected static void Process<T>(DirectoryInfo? directory, Action<IRecordManager, T> action, T payload)
        {
            IRecordManager adrUtils = Factory.CreateManager(directory?.FullName ?? Directory.GetCurrentDirectory());
            try
            {
                action.Invoke(adrUtils, payload);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        protected static void Process(DirectoryInfo? directory, Action<IRecordManager> action)
            => Process(directory, static (mgr, act) => act.Invoke(mgr), action);
    }
}