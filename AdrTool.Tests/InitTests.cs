using AdrTool.Core;
using Moq;

namespace AdrTool.Tests
{
    public class InitTests : TestBase
    {
        [Fact]
        public async Task Verify_FolderExitence()
        {
            // act
            await RecordManager.InitAsync();

            // assert
            InputOutputUtilsMock.Verify(
                mock => mock.EnsureFolderExistence("z:\\UnitTest"),
                Times.Once);
            InputOutputUtilsMock.Verify(
                mock => mock.EnsureFolderExistence("z:\\UnitTest\\templates"),
                Times.Once);
            InputOutputUtilsMock.Verify(
                mock => mock.EnsureFolderExistence("z:\\UnitTest\\docs"),
                Times.Once);
        }

        [Fact]
        public async Task Verify_CopyAllTemplates()
        {
            // act
            await RecordManager.InitAsync();

            // assert
            InputOutputUtilsMock.Verify(
                mock => mock.WriteFileAsync(It.IsAny<string>(), It.IsAny<Stream>()),
                Times.Exactly(1));
            InputOutputUtilsMock.Verify(
                mock => mock.WriteFileAsync("z:\\UnitTest\\templates\\simple-template.md", It.IsAny<Stream>()),
                Times.Once);
        }

        [Fact]
        public async Task Verify_SetDefaultTemplate()
        {
            // arrange
            InputOutputUtilsMock
                .Setup(svc => svc.GetFiles("z:\\UnitTest\\templates"))
                .Returns(new[] { "z:\\UnitTest\\templates\\simple-template.md" });

            // act
            await RecordManager.InitAsync();

            // assert
            InputOutputUtilsMock.Verify(
                mock => mock.CopyFile(It.IsAny<string>(), It.IsAny<string>()),
                Times.Exactly(1));
            InputOutputUtilsMock.Verify(
                mock => mock.CopyFile("z:\\UnitTest\\templates\\simple-template.md", "z:\\UnitTest\\templates\\default.md"),
                Times.Once);
        }
    }
}