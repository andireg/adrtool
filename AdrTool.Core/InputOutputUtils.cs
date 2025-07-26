using System.Diagnostics.CodeAnalysis;

namespace AdrTool.Core;

[ExcludeFromCodeCoverage]
public class InputOutputUtils : IInputOutputUtils
{
    public string Combine(string path1, string path2)
        => Path.Combine(path1, path2);

    public void CopyFile(string sourceFilename, string targetFilename)
        => File.Copy(sourceFilename, targetFilename);

    public void DeleteFile(string filename)
    {
        if (File.Exists(filename))
        {
            File.Delete(filename);
        }
    }

    public void EnsureFolderExistence(string path)
    {
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
    }

    public bool FileExists(string filename)
        => File.Exists(filename);

    public IEnumerable<string> GetAllFiles(string path)
    {
        List<string> result = new();
        GetAllFiles(path, result);
        return result;
    }

    private void GetAllFiles(string path, List<string> files)
    {
        files.AddRange(Directory.GetFiles(path));
        string[] dirs = Directory.GetDirectories(path);
        foreach (string dir in dirs)
        {
            GetAllFiles(dir, files);
        }
    }

    public IEnumerable<string> GetDirectories(string path)
        => Directory.GetDirectories(path);

    public string GetFileName(string path)
        => Path.GetFileName(path);

    public IEnumerable<string> GetFiles(string path)
        => Directory.GetFiles(path);

    public Task<string> ReadFileAsync(string filename)
        => File.ReadAllTextAsync(filename);

    public Task WriteFileAsync(string filename, string content)
        => File.WriteAllTextAsync(filename, content);

    public async Task WriteFileAsync(string filename, Stream content)
    {
        using FileStream fileStream = File.Create(filename);
        await content.CopyToAsync(fileStream);
    }
}