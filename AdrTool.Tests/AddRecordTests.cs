using AdrTool.Core;
using Moq;

namespace AdrTool.Tests
{
    public class AddRecordTests : TestBase
    {
        [Fact]
        public async Task Verify_UseDefaultTemplate()
        {
            // arrange
            InputOutputUtilsMock
                .Setup(mock => mock.GetFiles("z:\\UnitTest\\docs\\folder"))
                .Returns(new[] { "z:\\UnitTest\\docs\\folder\\0001-document title.md", "z:\\UnitTest\\docs\\folder\\0011-document title.md" });
            InputOutputUtilsMock
                .Setup(mock => mock.ReadFileAsync("z:\\UnitTest\\templates\\default.md"))
                .ReturnsAsync("# {%title} {%date:yyyy-MM-dd}");

            // act
            await RecordManager.AddRecordAsync("folder/new record", null);

            // assert
            InputOutputUtilsMock.Verify(
                mock => mock.WriteFileAsync(
                    "z:\\UnitTest\\docs\\folder\\12-new record.md",
                    $"# new record {DateTime.Now:yyyy-MM-dd}"),
                Times.Once);
        }

        [Fact]
        public async Task Verify_WhenUseTemplateWithDefaultNameThrowsException()
        {
            // act
            InvalidOperationException exception = await Assert.ThrowsAsync<InvalidOperationException>(
                () => RecordManager.AddRecordAsync("folder/new record", "default"));

            // assert
            Assert.Equal("Template name default is not allowed.", exception.Message);
        }

        [Fact]
        public async Task Verify_NonExistingThrowsException()
        {
            // act
            InvalidOperationException exception = await Assert.ThrowsAsync<InvalidOperationException>(
                () => RecordManager.AddRecordAsync("folder/new record", "non-existing-template"));

            // assert
            Assert.Equal("Template non-existing-template does not exist.", exception.Message);
        }

        [Fact]
        public async Task Verify_WhenUseTemplate()
        {
            // arrange
            InputOutputUtilsMock
                .Setup(mock => mock.FileExists("z:\\UnitTest\\templates\\existing-template.md"))
                .Returns(true);
            InputOutputUtilsMock
                .Setup(mock => mock.GetFiles("z:\\UnitTest\\docs\\folder"))
                .Returns(new[] { "z:\\UnitTest\\docs\\folder\\0001-document title.md", "z:\\UnitTest\\docs\\folder\\0011-document title.md" });
            InputOutputUtilsMock
                .Setup(mock => mock.ReadFileAsync("z:\\UnitTest\\templates\\existing-template.md"))
                .ReturnsAsync("# {%title} {%date:yyyy-MM-dd}");

            // act
            await RecordManager.AddRecordAsync("folder/new record", "existing-template");

            // assert
            InputOutputUtilsMock.Verify(
                mock => mock.WriteFileAsync(
                    "z:\\UnitTest\\docs\\folder\\12-new record.md",
                    $"# new record {DateTime.Now:yyyy-MM-dd}"),
                Times.Once);
        }
    }
}