using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Dryer_Server.Serial_Modbus_Agent;
using System.IO.Ports;
using Dryer_Server.Interfaces;
using System;
using System.Linq;

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
        public void CheckCrc() 
        {
            foreach (var r in requests)
            {
                var crc = CrcModbus.Get(r.Take(r.Length - 2));
                var crcEnumerator = crc.GetEnumerator();

                Assert.IsTrue(crcEnumerator.MoveNext());
                Assert.AreEqual(crcEnumerator.Current, r[r.Length - 2]);
                Assert.IsTrue(crcEnumerator.MoveNext());
                Assert.AreEqual(crcEnumerator.Current, r[r.Length - 1]);
                Assert.IsFalse(crcEnumerator.MoveNext());
            }
        }

        [TestMethod]
        public void ShouldDecodeFirst()
        {
            ChamberSensors result = null;
            var listener = new SerialModbusChamberListener((string)null);
            var receiverMock = new Mock<IValueReceiver<ChamberSensors>>();

            receiverMock
                .Setup(r => r.ValueReceived(It.IsAny<ChamberSensors>()))
                .Callback<ChamberSensors>(s => result = s);
            listener.Add(20, receiverMock.Object);

            listener.ReadValues(this.normalData.Skip(2*23).Take(23));
            
            receiverMock.Verify(r => r.ValueReceived(It.IsAny<ChamberSensors>()), Times.Once);
            Assert.IsNotNull(result);
            Assert.AreEqual(66.855F, result.Humidity, 0.01F);
            Assert.AreEqual(1.899F, result.Temperature, 0.01F);
        }
    }
}
