﻿using System.Diagnostics;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace AdrTool.Core;

public class RecordManager : IRecordManager
{
    private static readonly JsonSerializerOptions options = new()
    {
        PropertyNameCaseInsensitive = true,
    };

    private readonly Regex regex;
    private readonly Settings settings;
    private readonly IInputOutputUtils inputOutputUtils;
    private readonly string baseFolder;
    private readonly string settingsFile;
    private readonly string templatesFolder;
    private readonly string docsFolder;
    private readonly string defaultTemplateFilename;

    public RecordManager(IInputOutputUtils inputOutputUtils, string baseFolder)
    {
        this.inputOutputUtils = inputOutputUtils;
        this.baseFolder = baseFolder;
        settingsFile = inputOutputUtils.Combine(baseFolder, Defaults.SettingsFilename);
        templatesFolder = inputOutputUtils.Combine(baseFolder, Defaults.TemplateFolder);
        docsFolder = inputOutputUtils.Combine(baseFolder, Defaults.DocsFolder);
        defaultTemplateFilename = inputOutputUtils.Combine(templatesFolder, $"{Defaults.DefaultTemplateName}.md");
        settings = ReadSettingsAsync().GetAwaiter().GetResult();
        regex = new(Defaults.DocFilename.Replace(".", "\\.").FormatWithObject(
            new {
                scope = string.Empty,
                number = "([0-9]{1,})",
                title = "([^\\\\\\/]*)" }));

    }

    public async Task InitAsync()
    {
        inputOutputUtils.EnsureFolderExistence(baseFolder);
        inputOutputUtils.EnsureFolderExistence(templatesFolder);
        inputOutputUtils.EnsureFolderExistence(docsFolder);

        await CheckTemplatesAsync();
        CheckDefaultTemplate();
        await ReindexAsync();
    }

    public async Task AddRecordAsync(string title, string? template)
    {
        string templateFilename = defaultTemplateFilename;
        if (!string.IsNullOrWhiteSpace(template))
        {
            templateFilename = CheckTemplateName(template);
        }

        string[] titleParts = title.Split('/', '\\');
        string titleOnly = titleParts.Last();
        string thisFolder = docsFolder;
        if (titleParts.Length > 1)
        {
            thisFolder = inputOutputUtils.Combine(thisFolder, string.Join("/", titleParts.Take(titleParts.Length - 1)));
            inputOutputUtils.EnsureFolderExistence(thisFolder);
        }

        int newNumber = GetNextNumber(thisFolder);
        string filename = inputOutputUtils.Combine(
            thisFolder, 
            Defaults.DocFilename.FormatWithObject(
                new { scope = settings.Scope, number = newNumber, title = titleOnly }).Replace(" ", "-"));
        string templateContent = await inputOutputUtils.ReadFileAsync(templateFilename);
        object param = new { date = DateTime.Now, title = titleOnly };
        string content = templateContent.FormatWithObject(param);
        await inputOutputUtils.WriteFileAsync(filename, content);
        await ReindexAsync();
    }

    public Task ReindexAsync()
        => ReindexFolderAsync(docsFolder);

    private async Task CheckTemplatesAsync()
    {
        Assembly assembly = Assembly.GetExecutingAssembly();
        string assemblyName = $"{assembly.GetName().Name}.Templates.";
        string[] resources = assembly.GetManifestResourceNames();
        foreach (string resource in resources.Where(r => r.StartsWith(assemblyName)))
        {
            string filename = inputOutputUtils.Combine(templatesFolder, resource.Replace(assemblyName, string.Empty));
            if (!inputOutputUtils.FileExists(filename))
            {
                Stream? stream = assembly.GetManifestResourceStream(resource);
                if (stream != null)
                {
                    await inputOutputUtils.WriteFileAsync(filename, stream);
                }
            }
        }
    }

    private void CheckDefaultTemplate()
    {
        if (inputOutputUtils.FileExists(defaultTemplateFilename))
        {
            return;
        }

        string? filename = inputOutputUtils.GetFiles(templatesFolder)?.FirstOrDefault();
        if (string.IsNullOrWhiteSpace(filename))
        {
            return;
        }

        inputOutputUtils.CopyFile(filename, defaultTemplateFilename);
    }

    private int GetNextNumber(string folder)
    {
        IEnumerable<string> files = settings.UniqueNumbers ?
            inputOutputUtils.GetAllFiles(docsFolder) :
            inputOutputUtils.GetFiles(folder);

        int max = files
                .Select(filename => regex.Match(filename))
                .Where(match => match.Success)
                .Select(match => (int?)int.Parse(match.Groups[1].Value))
                .Max() ?? 0;
        return max + 1;
    }

    private string GetCheckedFilename(string filename, string folder)
    {
        Match match = regex.Match(filename);
        if (!match.Success)
        {
            return filename;
        }

        string newFilename = Defaults.DocFilename.FormatWithObject(
            new
            {
                scope = settings.Scope ?? string.Empty,
                number = match.Groups[1].Value,
                title = match.Groups[2].Value,
            });

        if (string.Equals(newFilename, inputOutputUtils.GetFileName(filename), StringComparison.Ordinal))
        {
            return filename;
        }

        inputOutputUtils.RenameFile(Path.Combine(folder, filename), Path.Combine(folder, newFilename));

        return newFilename;
    }

    private async Task ReindexFolderAsync(string folder)
    {
        Debug.WriteLine($"Reindex '{folder}'");
        string folderName = inputOutputUtils.GetFileName(folder);
        string contentFiles = string.Join(
            Environment.NewLine,
            inputOutputUtils.GetFiles(folder)
                .Select(filename => GetCheckedFilename(filename, folder))
                .Select(filename => new { filename, match = regex.Match(filename) })
                .Where(item => item.match.Success)
                .OrderBy(item => int.Parse(item.match.Groups[1].Value))
                .Select(item => Defaults.IndexFileLineTemplate.FormatWithObject(
                    new
                    {
                        scope = settings.Scope ?? string.Empty,
                        number = item.match.Groups[1].Value,
                        title = item.match.Groups[2].Value.Replace("-", " "),
                        filename = $"{folderName}/{inputOutputUtils.GetFileName(item.filename)}",
                    })));

        IEnumerable<string> subFolders = inputOutputUtils.GetDirectories(folder);

        string contentFolders = string.Join(
            Environment.NewLine,
            subFolders
                .OrderBy(filename => filename)
                .Select(filename => Defaults.IndexFolderLineTemplate.FormatWithObject(
                    new
                    {
                        filename = $"{folderName}/{inputOutputUtils.GetFileName(filename)}",
                        foldername = inputOutputUtils.GetFileName(filename),
                    })));

        if (!string.IsNullOrWhiteSpace(contentFolders))
        {
            contentFolders += Environment.NewLine;
        }

        string indexFilename = inputOutputUtils.Combine(folder, $"..\\{folderName}.md");
        string content = Defaults.IndexTemplate.FormatWithObject(new { folders = contentFolders, files = contentFiles });
        await inputOutputUtils.WriteFileAsync(indexFilename, content);
        await Task.WhenAll(subFolders.Select(ReindexFolderAsync));
    }

    public IEnumerable<string> ListTemplates()
        => inputOutputUtils.GetFiles(templatesFolder)
            .Select(t => inputOutputUtils.GetFileName(t)[..^3])
            .OrderBy(t => t);

    public void AddTemplate(string name)
    {
        string filename = CheckNonDefaultTemplateName(name);
        if (inputOutputUtils.FileExists(filename))
        {
            throw new InvalidOperationException($"Template {name} does already exist.");
        }

        inputOutputUtils.CopyFile(defaultTemplateFilename, filename);
    }

    public void RemoveTemplate(string name)
    {
        string filename = CheckTemplateName(name);
        inputOutputUtils.DeleteFile(filename);
    }

    public void SetDefaultTemplate(string name)
    {
        string filename = CheckTemplateName(name);
        inputOutputUtils.DeleteFile(defaultTemplateFilename);
        inputOutputUtils.CopyFile(filename, defaultTemplateFilename);
    }

    private string CheckTemplateName(string name)
    {
        string filename = CheckNonDefaultTemplateName(name);
        if (!inputOutputUtils.FileExists(filename))
        {
            throw new InvalidOperationException($"Template {name} does not exist.");
        }

        return filename;
    }

    private string CheckNonDefaultTemplateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new InvalidOperationException("Template name needs to be defined.");
        }

        if (string.Equals(name, Defaults.DefaultTemplateName, StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException($"Template name {name} is not allowed.");
        }

        return inputOutputUtils.Combine(templatesFolder, name + ".md");
    }

    public async Task SetConfigAsync(string key, string? value)
    {
        PropertyInfo[] properties = settings.GetType().GetProperties();
        PropertyInfo? property = properties.FirstOrDefault(prop => string.Equals(key, prop.Name, StringComparison.InvariantCultureIgnoreCase));
        if (property == null)
        {
            throw new ArgumentOutOfRangeException($"No config {key} found.");
        }

        object? valueToSet = value;
        if( property.PropertyType == typeof(bool))
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                valueToSet = false;
            }
            else if (bool.TryParse(value, out bool boolValue))
            {
                valueToSet = boolValue;
            }
            else
            {
                throw new ArgumentOutOfRangeException($"Invalid boolean value for {key}: {value}");
            }
        }

        property.SetValue(settings, valueToSet);

        await WriteSettingsAsync();
    }

    private async Task<Settings> ReadSettingsAsync()
    {
        if (inputOutputUtils.FileExists(settingsFile))
        {
            string text = await inputOutputUtils.ReadFileAsync(settingsFile);
            return JsonSerializer.Deserialize<Settings>(text, options) ?? new Settings();
        }

        return new Settings();
    }

    private async Task WriteSettingsAsync()
    {
        string text = JsonSerializer.Serialize(settings);
        await inputOutputUtils.WriteFileAsync(settingsFile, text);
    }
}
