using AdrTool.Core;

namespace AdrTool.Tests
{
    public class StringFormatterTests
    {
        [Fact]
        public void Verify_SimpleFormat()
        {
            // act
            string actual = "Number {%text}".FormatWithObject(new { text = "FooBar" });

            // assert
            Assert.Equal("Number FooBar", actual);
        }

        [Fact]
        public void Verify_FormatWithFormat()
        {
            // act
            string actual = "Number {%number:0000}".FormatWithObject(new { number = 42 });

            // assert
            Assert.Equal("Number 0042", actual);
        }
    }
}
