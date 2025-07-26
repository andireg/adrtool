using AdrTool.Core;
using Moq;

namespace AdrTool.Tests;

public class FactoryTests : TestBase
{
    [Fact]
    public void Verify_CreateManager()
    {
        // act
        IRecordManager recordManager = Factory.CreateManager("z:\\UnitTest");

        // assert
        Assert.NotNull(recordManager);
    }
}