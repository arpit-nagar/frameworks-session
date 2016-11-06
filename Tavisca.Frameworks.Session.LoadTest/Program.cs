using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tavisca.Frameworks.Session.LoadTest
{
    public class Program
    {
        private const int DataSize = 20000;
        private const int RunCount = 1000;

        private static List<TestData> _data = new List<TestData>(DataSize);
        private const string Category = "Tavisca";
        private static Dictionary<int, string> _keys = new Dictionary<int, string>(RunCount);

        static Program()
        {
            for (int i = 0; i < DataSize; i++)
            {
                _data.Add(new TestData() { FirstName = "Phoenix" + i, LastName = "Core" + i });
            }

            for (var i = 0; i < RunCount; i++)
            {
                _keys.Add(i, Guid.NewGuid().ToString());
            }
        }
        
        static void Main(string[] args)
        {
            for (int i = 0; i < RunCount; i++)
            {
                Write(i);

                Console.WriteLine("write finished " + i);
            }

            Console.WriteLine("Finished writes...beginning reads");

            for (int i = 0; i < RunCount; i++)
            {
                Read(i);

                Console.WriteLine("read finished " + i);
            }
        }

        private static void Read(int x)
        {
            var data = SessionStore.Session[Category, _keys[x]];

            if (data == null)
                throw new Exception();
        }

        private static void Write(int x)
        {
            SessionStore.Session[Category, _keys[x]] = _data;
        }
    }

    [Serializable]
    public class TestData
    {
        public String FirstName;
        public String LastName;
    }

}
