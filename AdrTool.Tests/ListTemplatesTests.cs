namespace AdrTool.Tests
{
    public class ListTemplatesTests : TestBase
    {
        [Fact]
        public void Verify_List()
        {
            // arrange
            InputOutputUtilsMock
                .Setup(mock => mock.GetFiles("z:\\UnitTest\\templates"))
                .Returns(new[] { "z:\\UnitTest\\templates\\simple-template.md" });

            // act
            IEnumerable<string> actual = RecordManager.ListTemplates();

            // assert
            Assert.NotNull(actual);
            Assert.Single(actual);
            Assert.Equal("simple-template", actual.First());
        }
    }
}
