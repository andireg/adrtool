using System;
using Moq;

namespace AdrTool.Tests;

public class SettingTests : TestBase
{
    [Fact]
    public async Task When_SetConfigWithWrongKey_ThenThrowException()
    {
        // arrange
        InputOutputUtilsMock
    .Setup(mock => mock.ReadFileAsync("z:\\UnitTest\\settings.json"))
    .ReturnsAsync("{\"scope\": \"MY_SCOPE\"}");
        InputOutputUtilsMock
            .Setup(mock => mock.FileExists("z:\\UnitTest\\settings.json"))
            .Returns(true);
        InputOutputUtilsMock
            .Setup(mock => mock.GetFiles("z:\\UnitTest\\docs"))
            .Returns(new[] { "z:\\UnitTest\\docs\\0001-document title.md" });
        InputOutputUtilsMock
            .Setup(mock => mock.GetDirectories("z:\\UnitTest\\docs"))
            .Returns(new[] { "z:\\UnitTest\\docs\\subfolder" });

        // act
        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(
            async () => await RecordManager.SetConfigAsync("KEY", "VALUE"));
    }

    [Fact]
    public async Task When_SetConfigWithValidKey_ThenSetValue()
    {
        // arrange
        InputOutputUtilsMock
            .Setup(mock => mock.ReadFileAsync("z:\\UnitTest\\settings.json"))
            .ReturnsAsync("{\"scope\": \"MY_SCOPE\"}");
        InputOutputUtilsMock
            .Setup(mock => mock.FileExists("z:\\UnitTest\\settings.json"))
                .Returns(true);
        InputOutputUtilsMock
            .Setup(mock => mock.GetFiles("z:\\UnitTest\\docs"))
            .Returns(new[] { "z:\\UnitTest\\docs\\0001-document title.md" });
        InputOutputUtilsMock
            .Setup(mock => mock.GetDirectories("z:\\UnitTest\\docs"))
            .Returns(new[] { "z:\\UnitTest\\docs\\subfolder" });

        // act
        await RecordManager.SetConfigAsync("Scope", "VALUE-SCOPY");

        // assert
        InputOutputUtilsMock.Verify(
            mock => mock.WriteFileAsync(
                "z:\\UnitTest\\settings.json",
                "{\"Scope\":\"VALUE-SCOPY\",\"UniqueNumbers\":false}"),
            Times.Once);
    }
}