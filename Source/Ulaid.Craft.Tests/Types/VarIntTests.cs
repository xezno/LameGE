using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Ulaid.Craft.Types.Tests
{
    [TestClass]
    public class VarIntTests
    {
        // Test data taken from "Sample VarInts" at https://wiki.vg/index.php?title=Data_types&oldid=14256
        private Dictionary<int, byte[]> testData = new Dictionary<int, byte[]>
        {
            { 0, new byte[] { 0x00 } },
            { 1, new byte[] { 0x01 } },
            { 2, new byte[] { 0x02 } },
            { 127, new byte[] { 0x7f } },
            { 128, new byte[] { 0x80, 0x01 } },
            { 255, new byte[] { 0xff, 0x01 } },
            { 2147483647, new byte[] { 0xff, 0xff, 0xff, 0xff, 0x07 } },
            { -1, new byte[] { 0xff, 0xff, 0xff, 0xff, 0x0f } },
            { -2147483648, new byte[] { 0x80, 0x80, 0x80, 0x80, 0x08 } },
        };

        private void CheckSingleFromInt(VarInt varInt, byte[] b)
        {
            Assert.IsTrue(Enumerable.SequenceEqual(varInt.RawValue, b));
        }

        private void CheckSingleFromStream(int intendedValue, byte[] b)
        {
            var memoryStream = new MemoryStream(b);
            var varInt = new VarInt(memoryStream);

            Assert.IsTrue(varInt.Value == intendedValue);
        }

        [TestMethod]
        public void FromInt()
        {
            foreach (var pair in testData)
            {
                CheckSingleFromInt(new VarInt(pair.Key), pair.Value);
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
