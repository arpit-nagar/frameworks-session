using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tavisca.Frameworks.Session.Tests
{
    [TestClass]
    public class LoadTests
    {
        private const int DataSize = 20000;
        private const int RunCount = 10000;

        private static readonly List<TestData> Data = new List<TestData>(DataSize);
        private const string Category = "Tavisca";
        private static readonly ConcurrentDictionary<int, string> Keys = new ConcurrentDictionary<int, string>();
        private static readonly ConcurrentBag<string> Values = new ConcurrentBag<string>();

        private static readonly Random RandomWrite = new Random();

        static LoadTests()
        {
            for (int i = 0; i < DataSize; i++)
            {
                Data.Add(new TestData() { FirstName = "Phoenix" + i, LastName = "Core" + i });
            }

            for (var i = 0; i < RunCount; i++)
            {
                Keys.AddOrUpdate(i, Guid.NewGuid().ToString(), (x, y) => y);
            }
        }

        [TestMethod]
        public void TestWrite()
        {
            var number = RandomWrite.Next(0, RunCount - 1);

            var key = Keys[number];

            //Values.Add(key);

            SessionStore.Session[Category, key] = Data;

            if ((number % 7) == 0)
            {
                var data = SessionStore.Session[Category, key];

                if (data == null)
                    throw new Exception("Read failed");
            }
        }

        
    }
}
