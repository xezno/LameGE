using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Ulaid.Craft.Types.Tests
{
    [TestClass]
    public class VarLongTests
    {
        // Test data taken from "Sample VarLongs" at https://wiki.vg/index.php?title=Data_types&oldid=14256
        private Dictionary<long, byte[]> testData = new Dictionary<long, byte[]>
        {
            { 0, new byte[] { 0x00 } },
            { 1, new byte[] { 0x01 } },
            { 2, new byte[] { 0x02 } },
            { 127, new byte[] { 0x7f } },
            { 128, new byte[] { 0x80, 0x01 } },
            { 255, new byte[] { 0xff, 0x01 } },
            { 2147483647, new byte[] { 0xff, 0xff, 0xff, 0xff, 0x07 } },
            { 9223372036854775807, new byte[] { 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0x7f } },
            { -1, new byte[] { 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0x01 } },
            { -2147483648, new byte[] { 0x80, 0x80, 0x80, 0x80, 0xf8, 0xff, 0xff, 0xff, 0xff, 0x01 } },
            { -9223372036854775808, new byte[] { 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x01 } },
        };

        private void CheckSingleFromLong(VarLong varLong, byte[] b)
        {
            Assert.IsTrue(Enumerable.SequenceEqual(varLong.RawValue, b));
        }

        private void CheckSingleFromStream(long intendedValue, byte[] b)
        {
            var memoryStream = new MemoryStream(b);
            var varLong = new VarLong(memoryStream);

            Assert.IsTrue(varLong.Value == intendedValue);
        }

        [TestMethod]
        public void FromLong()
        {
            foreach (var pair in testData)
            {
                CheckSingleFromLong(new VarLong(pair.Key), pair.Value);
            }
        }

        [TestMethod]
        public void FromStream()
        {
            foreach (var pair in testData)
            {
                CheckSingleFromStream(pair.Key, pair.Value);
            }
        }
    }
}
