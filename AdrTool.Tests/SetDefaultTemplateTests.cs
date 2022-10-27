using AdrTool.Core;
using Moq;

namespace AdrTool.Tests
{
    public class SetDefaultTemplateTests : TestBase
    {
        [Fact]
        public void Verify_EmptyNameThrowsException()
        {
            // act
            InvalidOperationException exception = Assert.Throws<InvalidOperationException>(() => RecordManager.SetDefaultTemplate(string.Empty));

            // assert
            Assert.Equal("Template name needs to be defined.", exception.Message);
        }

        [Fact]
        public void Verify_DefaultNameThrowsException()
        {
            // act
            InvalidOperationException exception = Assert.Throws<InvalidOperationException>(() => RecordManager.SetDefaultTemplate("default"));

            // assert
            Assert.Equal("Template name default is not allowed.", exception.Message);
        }

        [Fact]
        public void Verify_NonExistingThrowsException()
        {
            // act
            InvalidOperationException exception = Assert.Throws<InvalidOperationException>(() => RecordManager.SetDefaultTemplate("non-existing-template"));

            // assert
            Assert.Equal("Template non-existing-template does not exist.", exception.Message);
        }

        [Fact]
        public void Verify_DefaultIsDeleted()
        {
            // arrange
            InputOutputUtilsMock
                .Setup(mock => mock.FileExists("z:\\UnitTest\\templates\\default.md"))
                .Returns(true);
            InputOutputUtilsMock
                .Setup(mock => mock.FileExists("z:\\UnitTest\\templates\\existing-template.md"))
                .Returns(true);

            // act
            RecordManager.SetDefaultTemplate("existing-template");

            // assert
            InputOutputUtilsMock.Verify(
                mock => mock.DeleteFile("z:\\UnitTest\\templates\\default.md"),
                Times.Once);
        }

        [Fact]
        public void Verify_TemplateIsCopied()
        {
            // arrange
            InputOutputUtilsMock
                .Setup(mock => mock.FileExists("z:\\UnitTest\\templates\\existing-template.md"))
                .Returns(true);

            // act
            RecordManager.SetDefaultTemplate("existing-template");

            // assert
            InputOutputUtilsMock.Verify(
                mock => mock.CopyFile("z:\\UnitTest\\templates\\existing-template.md", "z:\\UnitTest\\templates\\default.md"),
                Times.Once);
        }
    }
}