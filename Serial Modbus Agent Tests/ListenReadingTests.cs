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
            var listener = new SerialModbusChamberListener(null);
            var receiverMock = new Mock<IValueReceiver<ChamberSensors>>();

            var checking = It.Is<ChamberSensors>(s 
                => Math.Abs(s.Humidity - 66.855F) < 0.01F
                && Math.Abs(s.Temperature - 1.899F) < 0.01F
            );

            receiverMock.Setup(r => r.ValueReceived(checking));
            listener.Add(20, receiverMock.Object);
            listener.ReadValues(this.normalData.Skip(2*23).Take(23));
            receiverMock.Verify(r => r.ValueReceived(checking));
        }
    }
}
