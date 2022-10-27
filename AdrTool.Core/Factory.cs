namespace AdrTool.Core
{
    public static class Factory
    {
        public static IRecordManager CreateManager(string baseFolder)
            => new RecordManager(new InputOutputUtils(), baseFolder);
    }
}