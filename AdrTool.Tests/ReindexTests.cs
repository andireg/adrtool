using AdrTool.Core;
using Moq;

namespace AdrTool.Tests
{
    public class ReindexTests : TestBase
    {
        [Fact]
        public async Task Verify_ScanRootFolder()
        {
            // arrange
            InputOutputUtilsMock
                .Setup(mock => mock.GetFiles("z:\\UnitTest\\docs"))
                .Returns(new[] { "z:\\UnitTest\\docs\\0001-document title.md" });
            InputOutputUtilsMock
                .Setup(mock => mock.GetDirectories("z:\\UnitTest\\docs"))
                .Returns(new[] { "z:\\UnitTest\\docs\\subfolder" });

            // act
            await RecordManager.ReindexAsync();

            // assert
            InputOutputUtilsMock.Verify(
                mock => mock.GetFiles("z:\\UnitTest\\docs"),
                Times.Once);
            InputOutputUtilsMock.Verify(
                mock => mock.GetDirectories("z:\\UnitTest\\docs"),
                Times.Once);
            InputOutputUtilsMock.Verify(
                mock => mock.GetFiles("z:\\UnitTest\\docs\\subfolder"),
                Times.Once);
            InputOutputUtilsMock.Verify(
                mock => mock.GetDirectories("z:\\UnitTest\\docs\\subfolder"),
                Times.Once);
            InputOutputUtilsMock.Verify(
                mock => mock.WriteFileAsync(
                    "z:\\UnitTest\\docs\\index.md",
                    @"# Index

| Number | Title | Link |
| ---:| --- | --- |
| | subfolder | [Link](subfolder/index.md) |
| 0001 | document title | [Link](0001-document title.md) |"),
                Times.Once);
        }
    }
}