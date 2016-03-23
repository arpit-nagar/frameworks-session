using System;
using System.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using ProtoBuf;
using Tavisca.Frameworks.Session.Configuration;
using Tavisca.Frameworks.Session.Formatters;

namespace Tavisca.Frameworks.Session.Tests
{
    [TestClass]
    public class HighLevelApiTests
    {
        private const string Category = "testCat";
        private const string AerospikeNamespace = "bar";
        private const string SqlConnString = "Tavisca.Frameworks.Session.Properties.Settings.dScepterDBConnectionString";
        private const string RedisConnString = "192.168.2.56";
        private const string AeroSpikeConnString = "192.168.2.15:3000";
        private static readonly string DynamoDBConnString = ConfigurationManager.AppSettings["dynamoConnString"];
        
        [TestMethod, Priority(10)]
        public void TestWithAppConfigValues()
        {
            ResetToAppConfigDefault();

            TestBasicOptions();
        }

        #region Sql Tests

        [TestMethod, Priority(2)]
        public void TestSqlWithBinaryFormatter()
        {
            ChangeConnString(SqlConnString);
            ChangeProvider(ProviderTypeOptions.Sql);
            ChangeFormatterType(FormatterTypeOptions.Binary);

            TestBasicOptions();
        }

        [TestMethod, Priority(2)]
        public void TestSqlWithBinaryCompressedFormatter()
        {
            ChangeConnString(SqlConnString);
            ChangeProvider(ProviderTypeOptions.Sql);
            ChangeFormatterType(FormatterTypeOptions.BinaryCompressed);

            TestBasicOptions();
        }

        [TestMethod, Priority(2)]
        public void TestSqlWithProtoFormatter()
        {
            ChangeConnString(SqlConnString);
            ChangeProvider(ProviderTypeOptions.Sql);
            ChangeFormatterType(FormatterTypeOptions.ProtoBuf);

            TestBasicOptions();
        }

        [TestMethod, Priority(2)]
        public void TestSqlWithProtoCompressedFormatter()
        {
            ChangeConnString(SqlConnString);
            ChangeProvider(ProviderTypeOptions.Sql);
            ChangeFormatterType(FormatterTypeOptions.ProtoBufCompressed);

            TestBasicOptions();
        }

        [TestMethod, Priority(2)]
        public void TestSqlWithJsonFormatter()
        {
            ChangeConnString(SqlConnString);
            ChangeProvider(ProviderTypeOptions.Sql);
            ChangeFormatterType(FormatterTypeOptions.Json);

            TestBasicOptions();
        }

        [TestMethod, Priority(2)]
        public void TestSqlWithJsonCompressedFormatter()
        {
            ChangeConnString(SqlConnString);
            ChangeProvider(ProviderTypeOptions.Sql);
            ChangeFormatterType(FormatterTypeOptions.JsonCompressed);

            TestBasicOptions();
        }

        #endregion

        #region Redis Tests

        [TestMethod, Priority(2)]
        public void TestRedisWithBinaryFormatter()
        {
            ChangeConnString(RedisConnString);
            ChangeProvider(ProviderTypeOptions.Redis);
            ChangeFormatterType(FormatterTypeOptions.Binary);

            TestBasicOptions();
        }

        [TestMethod, Priority(2)]
        public void TestRedisWithBinaryCompressedFormatter()
        {
            ChangeConnString(RedisConnString);
            ChangeProvider(ProviderTypeOptions.Redis);
            ChangeFormatterType(FormatterTypeOptions.BinaryCompressed);

            TestBasicOptions();
        }

        [TestMethod, Priority(2)]
        public void TestRedisWithProtoFormatter()
        {
            ChangeConnString(RedisConnString);
            ChangeProvider(ProviderTypeOptions.Redis);
            ChangeFormatterType(FormatterTypeOptions.ProtoBuf);

            TestBasicOptions();
        }

        [TestMethod, Priority(2)]
        public void TestRedisWithProtoCompressedFormatter()
        {
            ChangeConnString(RedisConnString);
            ChangeProvider(ProviderTypeOptions.Redis);
            ChangeFormatterType(FormatterTypeOptions.ProtoBufCompressed);

            TestBasicOptions();
        }

        [TestMethod, Priority(2)]
        public void TestRedisWithJsonCompressedFormatter()
        {
            ChangeConnString(RedisConnString);
            ChangeProvider(ProviderTypeOptions.Redis);
            ChangeFormatterType(FormatterTypeOptions.JsonCompressed);

            TestBasicOptions();
        }

        [TestMethod, Priority(2)]
        public void TestRedisWithJsonFormatter()
        {
            ChangeConnString(RedisConnString);
            ChangeProvider(ProviderTypeOptions.Redis);
            ChangeFormatterType(FormatterTypeOptions.Json);

            TestBasicOptions();
        }

        [TestMethod, Priority(2)]
        public void TestAsyncAdd()
        {
            const string fName = "test_fname";
            const string lName = "test_lname";

            var store = new SessionStore();

            var key = Guid.NewGuid().ToString();

            store.AddAsync(Category, key, new TestData()
            {
                FirstName = fName,
                LastName = lName
            });



            TestData storedVal = null;
            WaitTillCondition(() =>
                {
                    storedVal = store.Get<TestData>(Category, key);

                    if (storedVal == null)
                        return false;
                    return true;
                }, 3000);
            

            Assert.IsNotNull(storedVal, "The stored value could not be retrieved, either the async add did not work or retrieve failed within the given time frame.");
            Assert.IsTrue(fName.Equals(storedVal.FirstName), "First name did not match.");
            Assert.IsTrue(lName.Equals(storedVal.LastName), "Last name did not match.");

            store.Remove(Category, key);

            storedVal = store.Get<TestData>(Category, key);

            Assert.IsNull(storedVal);
        }

        #endregion

        #region AeroSpike Tests

        [TestMethod, Priority(2)]
        public void TestAeroSpikeWithBinaryFormatter()
        {
            ChangeConnString(AeroSpikeConnString);
            ChangeProvider(ProviderTypeOptions.AeroSpike);
            ChangeFormatterType(FormatterTypeOptions.Binary);
            ChangeApplicationKey(AerospikeNamespace);

            TestBasicOptions();
        }

        [TestMethod, Priority(2)]
        public void TestAeroSpikeWithBinaryCompressedFormatter()
        {
            ChangeConnString(AeroSpikeConnString);
            ChangeProvider(ProviderTypeOptions.AeroSpike);
            ChangeFormatterType(FormatterTypeOptions.BinaryCompressed);
            ChangeApplicationKey(AerospikeNamespace);

            TestBasicOptions();
        }

        [TestMethod, Priority(2)]
        public void TestAeroSpikeWithProtoFormatter()
        {
            ChangeConnString(AeroSpikeConnString);
            ChangeProvider(ProviderTypeOptions.AeroSpike);
            ChangeFormatterType(FormatterTypeOptions.ProtoBuf);
            ChangeApplicationKey(AerospikeNamespace);

            TestBasicOptions();
        }

        [TestMethod, Priority(2)]
        public void TestAeroSpikeWithProtoCompressedFormatter()
        {
            ChangeConnString(AeroSpikeConnString);
            ChangeProvider(ProviderTypeOptions.AeroSpike);
            ChangeFormatterType(FormatterTypeOptions.ProtoBufCompressed);
            ChangeApplicationKey(AerospikeNamespace);

            TestBasicOptions();
        }

        [TestMethod, Priority(2)]
        public void TestAeroSpikeWithJsonCompressedFormatter()
        {
            ChangeConnString(AeroSpikeConnString);
            ChangeProvider(ProviderTypeOptions.AeroSpike);
            ChangeFormatterType(FormatterTypeOptions.JsonCompressed);
            ChangeApplicationKey(AerospikeNamespace);

            TestBasicOptions();
        }

        [TestMethod, Priority(2)]
        public void TestAeroSpikeWithJsonFormatter()
        {
            ChangeConnString(AeroSpikeConnString);
            ChangeProvider(ProviderTypeOptions.AeroSpike);
            ChangeFormatterType(FormatterTypeOptions.Json);
            ChangeApplicationKey(AerospikeNamespace);

            TestBasicOptions();
        }

        #endregion

        #region DynamoDB Tests

        [TestMethod, Priority(2)]
        public void TestDynamoDBWithBinaryFormatter()
        {
            ChangeConnString(DynamoDBConnString);
            ChangeProvider(ProviderTypeOptions.DynamoDB);
            ChangeFormatterType(FormatterTypeOptions.Binary);

            TestBasicOptions();
        }

        [TestMethod, Priority(2)]
        public void TestDynamoDBWithBinaryCompressedFormatter()
        {
            ChangeConnString(DynamoDBConnString);
            ChangeProvider(ProviderTypeOptions.DynamoDB);
            ChangeFormatterType(FormatterTypeOptions.BinaryCompressed);

            TestBasicOptions();
        }

        [TestMethod, Priority(2)]
        public void TestDynamoDBWithProtoFormatter()
        {
            ChangeConnString(DynamoDBConnString);
            ChangeProvider(ProviderTypeOptions.DynamoDB);
            ChangeFormatterType(FormatterTypeOptions.ProtoBuf);

            TestBasicOptions();
        }

        [TestMethod, Priority(2)]
        public void TestDynamoDBWithProtoCompressedFormatter()
        {
            ChangeConnString(DynamoDBConnString);
            ChangeProvider(ProviderTypeOptions.DynamoDB);
            ChangeFormatterType(FormatterTypeOptions.ProtoBufCompressed);

            TestBasicOptions();
        }

        [TestMethod, Priority(2)]
        public void TestDynamoDBWithJsonCompressedFormatter()
        {
            ChangeConnString(DynamoDBConnString);
            ChangeProvider(ProviderTypeOptions.DynamoDB);
            ChangeFormatterType(FormatterTypeOptions.JsonCompressed);

            TestBasicOptions();
        }

        [TestMethod, Priority(2)]
        public void TestDynamoDBWithJsonFormatter()
        {
            ChangeConnString(DynamoDBConnString);
            ChangeProvider(ProviderTypeOptions.DynamoDB);
            ChangeFormatterType(FormatterTypeOptions.Json);

            TestBasicOptions();
        }

        #endregion

        #region Helper Methods

        public static bool WaitTillCondition(Func<bool> condition, int timeoutInMilliseconds)
        {
            var time = Environment.TickCount;

            while (!condition.Invoke())
            {
                System.Threading.Thread.Sleep(100);

                if ((Environment.TickCount - time) > timeoutInMilliseconds)
                    return false;
            }
            return true;
        }

        private void ResetToAppConfigDefault()
        {
            var appConfigSettings = (ISessionConfiguration) ConfigurationManager.GetSection("DataStore");

            SessionConfigurationManager.SetConfiguration(appConfigSettings);
        }

        private void ChangeFormatterType(FormatterTypeOptions formatterType)
        {
            var config = SessionConfigurationManager.GetConfiguration().GetWritableCopy();

            config.Formatter = formatterType;

            SessionConfigurationManager.SetConfiguration(config);
        }

        private void ChangeConnString(string connString)
        {
            var config = SessionConfigurationManager.GetConfiguration().GetWritableCopy();

            config.ConnStringNameOrValue = connString;

            SessionConfigurationManager.SetConfiguration(config);
        }

        private void ChangeProvider(ProviderTypeOptions providerType)
        {
            var config = SessionConfigurationManager.GetConfiguration().GetWritableCopy();

            config.Provider = providerType;

            SessionConfigurationManager.SetConfiguration(config);
        }

        private void ChangeApplicationKey(string applicationKey)
        {
            var config = SessionConfigurationManager.GetConfiguration().GetWritableCopy();

            config.ApplicationKey = applicationKey;

            SessionConfigurationManager.SetConfiguration(config);
        }

        private void TestBasicOptions()
        {
            const string fName = "test_fname";
            const string lName = "test_lname";

            var store = new SessionStore();

            var key = Guid.NewGuid().ToString();

            store.Add(Category, key, new TestData()
            {
                FirstName = fName,
                LastName = lName
            });

            var storedVal = store.Get<TestData>(Category, key);

            Assert.IsNotNull(storedVal);
            Assert.IsTrue(fName.Equals(storedVal.FirstName), "First name did not match.");
            Assert.IsTrue(lName.Equals(storedVal.LastName), "Last name did not match.");

            store.Remove(Category, key);

            storedVal = store.Get<TestData>(Category, key);

            Assert.IsNull(storedVal);
        }

        //private static readonly List<TestData> TestDatas = new List<TestData>(10000);
        //private List<TestData> GetTestData()
        //{
        //    const string fName = "test_fname";
        //    const string lName = "test_lname";

        //    if (TestDatas.Count > 0)
        //        return TestDatas;

        //    for (int i = 0; i < 10000; i++)
        //    {
        //        TestDatas.Add(new TestData()
        //        {
        //            FirstName = fName,
        //            LastName = lName
        //        });
        //    }
        //    return TestDatas;
        //}

        #endregion
    }

    [Serializable]
    [ProtoContract(ImplicitFields = ImplicitFields.AllFields)]
    public class TestData
    {
        public string FirstName;
        public string LastName;
    }
}
