namespace AdrTool.Core
{
    public interface IRecordManager
    {
        Task AddRecordAsync(string title, string? template);

        void AddTemplate(string name);

        Task InitAsync();

        IEnumerable<string> ListTemplates();

        Task ReindexAsync();

        void RemoveTemplate(string name);

        void SetDefaultTemplate(string name);
    }
}