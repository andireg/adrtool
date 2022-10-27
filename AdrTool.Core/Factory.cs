namespace AdrTool.Core
{
    /// <summary>
    /// Factory to create instance of `IRecordManager`.
    /// </summary>
    public static class Factory
    {
        /// <summary>
        /// Creates an instance of `IRecordManager`.
        /// </summary>
        /// <param name="baseFolder">The base folder of the ADR.</param>
        /// <returns>An instance of `IRecordManager`.</returns>
        public static IRecordManager CreateManager(string baseFolder)
            => new RecordManager(new InputOutputUtils(), baseFolder);
    }
}