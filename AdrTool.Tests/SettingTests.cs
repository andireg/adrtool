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

        // act
        await RecordManager.SetConfigAsync("Scope", "VALUE-SCOPY");

        // assert
        InputOutputUtilsMock.Verify(
            mock => mock.WriteFileAsync(
                "z:\\UnitTest\\settings.json",
                "{\"Scope\":\"VALUE-SCOPY\",\"UniqueNumbers\":false}"),
            Times.Once);
    }

    [Fact]
    public async Task When_SetConfigBoolValue_ThenSetValue()
    {
        // arrange
        InputOutputUtilsMock
            .Setup(mock => mock.ReadFileAsync("z:\\UnitTest\\settings.json"))
            .ReturnsAsync("{\"uniqueNumbers\": false}");
        InputOutputUtilsMock
            .Setup(mock => mock.FileExists("z:\\UnitTest\\settings.json"))
            .Returns(true);

        // act
        await RecordManager.SetConfigAsync("UniqueNumbers", "true");

        // assert
        InputOutputUtilsMock.Verify(
            mock => mock.WriteFileAsync(
                "z:\\UnitTest\\settings.json",
                "{\"Scope\":\"\",\"UniqueNumbers\":true}"),
            Times.Once);
    }
}