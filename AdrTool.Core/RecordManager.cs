using System.Diagnostics;
using System.Reflection;
using System.Text.RegularExpressions;

namespace AdrTool.Core
{
    public class RecordManager : IRecordManager
    {
        private readonly Regex regex;
        private readonly IInputOutputUtils inputOutputUtils;
        private readonly string baseFolder;
        private readonly string templatesFolder;
        private readonly string docsFolder;
        private readonly string defaultTemplateFilename;

        public RecordManager(IInputOutputUtils inputOutputUtils, string baseFolder)
        {
            this.inputOutputUtils = inputOutputUtils;
            this.baseFolder = baseFolder;
            templatesFolder = inputOutputUtils.Combine(baseFolder, Defaults.TemplateFolder);
            docsFolder = inputOutputUtils.Combine(baseFolder, Defaults.DocsFolder);
            defaultTemplateFilename = inputOutputUtils.Combine(templatesFolder, $"{Defaults.DefaultTemplateName}.md");
            regex = new (Defaults.DocFilename.Replace(".", "\\.").FormatWithObject(new { number = "([0-9]*)", title = "(.*)" }));
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
            string filename = inputOutputUtils.Combine(thisFolder, Defaults.DocFilename.FormatWithObject(new { number = newNumber, title = titleOnly }));
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
            int max = inputOutputUtils.GetFiles(folder)
                .Select(filename => regex.Match(filename))
                .Where(match => match.Success)
                .Select(match => (int?)int.Parse(match.Groups[1].Value))
                .Max() ?? 0;
            return max + 1;
        }

        private async Task ReindexFolderAsync(string folder)
        {
            Debug.WriteLine($"Reindex '{folder}'");
            string contentFiles = string.Join(
                Environment.NewLine,
                inputOutputUtils.GetFiles(folder)
                    .Select(filename => new { filename, match = regex.Match(filename) })
                    .Where(x => x.match.Success)
                    .OrderBy(x => int.Parse(x.match.Groups[1].Value))
                    .Select(x => Defaults.IndexFileLineTemplate.FormatWithObject(
                        new { number = x.match.Groups[1].Value, title = x.match.Groups[2].Value, filename = inputOutputUtils.GetFileName(x.filename) })));

            IEnumerable<string> subFolders = inputOutputUtils.GetDirectories(folder);

            string contentFolders = string.Join(
                Environment.NewLine,
                subFolders
                    .OrderBy(filename => filename)
                    .Select(filename => Defaults.IndexFolderLineTemplate.FormatWithObject(new { folderName = inputOutputUtils.GetFileName(filename) })));

            if (!string.IsNullOrWhiteSpace(contentFolders))
            {
                contentFolders += Environment.NewLine;
            }

            string indexFilename = inputOutputUtils.Combine(folder, "index.md");
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
    }
}