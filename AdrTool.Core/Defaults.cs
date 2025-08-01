﻿namespace AdrTool.Core;

internal static class Defaults
{
    internal const string TemplateFolder = "templates";
    internal const string DocsFolder = "docs";
    internal const string SettingsFilename = "settings.json";
    internal const string DefaultTemplateName = "default";
    internal const string DateFormat = "yyyy-MM-dd";
    internal const string DocFilename = "{%scope}{%number:0000}-{%title}.md";

    internal const string IndexTemplate = @"# Index

| Number | Title | Link |
| ---:| --- | --- |
{%folders}{%files}";

    internal const string IndexFolderLineTemplate = "| | {%foldername} | [Link]({%filename}.md) |";
    internal const string IndexFileLineTemplate = "| {%scope}{%number:0000} | {%title} | [Link]({%filename}) |";
}