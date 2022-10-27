using AdrTool.Core;
using Moq;

namespace AdrTool.Tests
{
    public class RemoveTemplateTests : TestBase
    {
        [Fact]
        public void Verify_EmptyNameThrowsException()
        {
            // act
            InvalidOperationException exception = Assert.Throws<InvalidOperationException>(() => RecordManager.RemoveTemplate(string.Empty));

            // assert
            Assert.Equal("Template name needs to be defined.", exception.Message);
        }

        [Fact]
        public void Verify_DefaultNameThrowsException()
        {
            // act
            InvalidOperationException exception = Assert.Throws<InvalidOperationException>(() => RecordManager.RemoveTemplate("default"));

            // assert
            Assert.Equal("Template name default is not allowed.", exception.Message);
        }

        [Fact]
        public void Verify_NonExistingThrowsException()
        {
            // act
            InvalidOperationException exception = Assert.Throws<InvalidOperationException>(() => RecordManager.RemoveTemplate("non-existing-template"));

            // assert
            Assert.Equal("Template non-existing-template does not exist.", exception.Message);
        }

        [Fact]
        public void Verify_DefaultIsCopied()
        {
            // arrange
            InputOutputUtilsMock
                .Setup(mock => mock.FileExists("z:\\UnitTest\\templates\\existing-template.md"))
                .Returns(true);

            // act
            RecordManager.RemoveTemplate("existing-template");

            // assert
            InputOutputUtilsMock.Verify(
                mock => mock.DeleteFile("z:\\UnitTest\\templates\\existing-template.md"),
                Times.Once);
        }
    }
}