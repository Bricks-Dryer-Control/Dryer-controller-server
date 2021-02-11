using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Dryer_Server.Serial_Modbus_Agent;
using System.IO.Ports;

namespace Serial_Modbus_Agent_Tests
{
    [TestClass]
    public partial class ListenReadingTests
    {
        private Mock<SerialPort> portMock;
        private SerialModbusChamberListener listener;

        [TestInitialize]
        public void Init()
        {
            portMock = new Mock<SerialPort>();
        }

        [TestMethod]
        public void ShouldDecodeFirst()
        {
            
        }
    }
}
