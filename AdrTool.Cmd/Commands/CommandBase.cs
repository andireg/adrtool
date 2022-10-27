using System.CommandLine;
using System.Diagnostics;
using System.Runtime.CompilerServices;
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

        protected static void Process<T>(
            DirectoryInfo? directory,
            Action<IRecordManager, T> action,
            T payload,
            [CallerFilePath] string? callerFilePath = null)
        {
            Debug.WriteLine($"Start process ({callerFilePath})");
            IRecordManager adrUtils = Factory.CreateManager(directory?.FullName ?? Directory.GetCurrentDirectory());
            try
            {
                action.Invoke(adrUtils, payload);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            Debug.WriteLine($"Finished process ({callerFilePath})");
        }

        protected static void Process(DirectoryInfo? directory, Action<IRecordManager> action)
            => Process(directory, static (mgr, act) => act.Invoke(mgr), action);
    }
}