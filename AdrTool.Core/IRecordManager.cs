namespace AdrTool.Core;

public interface IRecordManager
{
    Task AddRecordAsync(string title, string? template);

    void AddTemplate(string name);

    Task InitAsync();

    IEnumerable<string> ListTemplates();

    Task ReindexAsync();

    void RemoveTemplate(string name);

    Task SetConfigAsync(string key, string? value);

    void SetDefaultTemplate(string name);
}