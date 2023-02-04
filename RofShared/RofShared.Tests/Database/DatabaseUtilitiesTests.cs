using NUnit.Framework;
using RofShared.Database;
using System;

namespace RofShared.Tests.Database
{
    [TestFixture]
    public class DatabaseUtilitiesTests
    {
        [Test]
        public void GetTotalPageTest()
        {
            var totalPage = DatabaseUtilities.GetTotalPages(23, 10, 1);
            Assert.AreEqual(3, totalPage);

            totalPage = DatabaseUtilities.GetTotalPages(20, 10, 1);
            Assert.AreEqual(2, totalPage);

            Assert.Throws<Exception>(() => DatabaseUtilities.GetTotalPages(20, 10, 3));
        }
    }
}
