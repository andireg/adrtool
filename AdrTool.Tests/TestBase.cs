using AdrTool.Core;
using Moq;

namespace AdrTool.Tests
{
    public abstract class TestBase
    {
        private RecordManager? recordManager;

        protected TestBase()
        {
            InputOutputUtilsMock
                .Setup(mock => mock.Combine(It.IsAny<string>(), It.IsAny<string>()))
                .Returns((string p1, string p2) => $"{p1}\\{p2}".Replace("\\\\", "\\"));

            InputOutputUtilsMock
                .Setup(mock => mock.GetFileName(It.IsAny<string>()))
                .Returns((string filename) => filename.Split("\\", StringSplitOptions.RemoveEmptyEntries).Last());

            InputOutputUtilsMock
                .Setup(mock => mock.GetFiles(It.IsAny<string>()))
                .Returns(Enumerable.Empty<string>());

            InputOutputUtilsMock
                .Setup(mock => mock.GetDirectories(It.IsAny<string>()))
                .Returns(Enumerable.Empty<string>());
        }

        protected Mock<IInputOutputUtils> InputOutputUtilsMock { get; } = new ();

        protected RecordManager RecordManager => recordManager ??= new RecordManager(InputOutputUtilsMock.Object, "z:\\UnitTest");
    }
}