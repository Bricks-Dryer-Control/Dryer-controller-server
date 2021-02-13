using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Dryer_Server.Serial_Modbus_Agent;
using System.IO.Ports;
using Dryer_Server.Interfaces;
using System;

namespace Serial_Modbus_Agent_Tests
{
    [TestClass]
    public partial class ListenReadingTests
    {
        Mock<SerialModbusChamberListener> listenerMock = new Mock<SerialModbusChamberListener>();

        [TestInitialize]
        public void Init()
        {
            
        }

        [TestMethod]
        public void ShouldDecodeFirst()
        {
            var listener = listenerMock.Object;
            var receiverMock = new Mock<IValueReceiver<ChamberSensors>>();
            listener.Add(18, receiverMock.Object);
            listener.ReadValues(this.normalData);
            receiverMock.Verify(r => r.ValueReceived(It.Is<ChamberSensors>(s => s.Humidity == 0D)));
        }
    }
}
