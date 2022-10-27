namespace AdrTool.Core
{
    internal static class Defaults
    {
        internal const string TemplateFolder = "templates";
        internal const string DocsFolder = "docs";
        internal const string DefaultTemplateName = "default";
        internal const string DateFormat = "yyyy-MM-dd";
        internal const string DocFilename = "{%number}-{%title}.md";

        internal const string IndexTemplate = @"# Index

| Number | Title | Link |
| ---:| --- | --- |
{%folders}{%files}";

        internal const string IndexFolderLineTemplate = "| | {%folderName} | [Link]({%folderName}/index.md) |";
        internal const string IndexFileLineTemplate = "| {%number} | {%title} | [Link]({%filename}) |";
    }
}