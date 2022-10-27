using AdrTool.Core;
using Moq;

namespace AdrTool.Tests
{
    public class AddTemplateTests : TestBase
    {
        [Fact]
        public void Verify_EmptyNameThrowsException()
        {
            // act
            InvalidOperationException exception = Assert.Throws<InvalidOperationException>(() => RecordManager.AddTemplate(string.Empty));

            // assert
            Assert.Equal("Template name needs to be defined.", exception.Message);
        }

        [Fact]
        public void Verify_DefaultNameThrowsException()
        {
            // act
            InvalidOperationException exception = Assert.Throws<InvalidOperationException>(() => RecordManager.AddTemplate("default"));

            // assert
            Assert.Equal("Template name default is not allowed.", exception.Message);
        }

        [Fact]
        public void Verify_FileExistsThrowsException()
        {
            // arrange
            InputOutputUtilsMock
                .Setup(mock => mock.FileExists("z:\\UnitTest\\templates\\new-template.md"))
                .Returns(true);

            // act
            InvalidOperationException exception = Assert.Throws<InvalidOperationException>(() => RecordManager.AddTemplate("new-template"));

            // assert
            Assert.Equal("Template new-template does already exist.", exception.Message);
        }

        [Fact]
        public void Verify_DefaultIsCopied()
        {
            // act
            RecordManager.AddTemplate("new-template");

            // assert
            InputOutputUtilsMock.Verify(
                mock => mock.CopyFile("z:\\UnitTest\\templates\\default.md", "z:\\UnitTest\\templates\\new-template.md"),
                Times.Once);
        }
    }
}