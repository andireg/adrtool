# dotnet ADR Tool

## Description

This solution contains a tiny dotnet tool to manage architecture decision records.

## Usage

### Installation

`dotnet tool install AdrTool.Cmd -g`

### Using

Just type the `adr` command in your prompt. There are this commands:

## Initialization

`adr init [--folder {root-path}]`

This command creates the default folder structure and places an initial template to the template-folder.

## Add new record

`adr add {title} [--folder {root-path}] [--template {template}]`

ThHis command adds a new record file to the docs-folder. 
If the title contains slashes (or backslashes), the record file will be placed to subfolders.

## Reindexing

`adr reindex [--folder {root-path}]`

This commands regenerates the index files.

## List existing templates

`adr template list [--folder {root-path}]`

This command lists all installed templates.

## Set default template

`adr template default {name-of-template} [--folder {root-path}]`

This command sets the default template to a specific one.

## Add new template

`adr template add {name-of-template} [--folder {root-path}]`

This command copies the current default template. You need to modify it manually.

## Remove template

`adr template remove {name-of-template} [--folder {root-path}]`

This command removes a template.