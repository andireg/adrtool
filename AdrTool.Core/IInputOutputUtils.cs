namespace AdrTool.Core;

public interface IInputOutputUtils
{
    string Combine(string path1, string path2);

    void CopyFile(string sourceFilename, string targetFilename);

    void RenameFile(string sourceFilename, string targetFilename);

    void DeleteFile(string filename);

    void EnsureFolderExistence(string path);

    bool FileExists(string filename);
    
    IEnumerable<string> GetAllFiles(string path);

    IEnumerable<string> GetDirectories(string path);

    string GetFileName(string path);

    IEnumerable<string> GetFiles(string path);

    Task<string> ReadFileAsync(string filename);
    
    Task WriteFileAsync(string filename, string content);

    Task WriteFileAsync(string filename, Stream content);
}