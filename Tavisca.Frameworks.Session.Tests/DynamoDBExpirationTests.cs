using System;
using System.Configuration;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tavisca.Frameworks.Session.Configuration;
using Tavisca.Frameworks.Session.Formatters;
using Tavisca.Frameworks.Session.Provider.DynamoDB;

namespace Tavisca.Frameworks.Session.Tests
{
    [TestClass]
    public class DynamoDBExpirationTests
    {
        private static readonly string DynamoDBManagerConnString = ConfigurationManager.AppSettings["dynamoMgrConnString"];
        private static readonly string DynamoDBConnString = ConfigurationManager.AppSettings["dynamoConnString"];
        private const string Category = "testCat";

        [TestMethod]
        public void DynamoDBExpirationTest()
        {
            ChangeConnString(DynamoDBConnString);
            ChangeProvider(ProviderTypeOptions.DynamoDB);
            ChangeFormatterType(FormatterTypeOptions.JsonCompressed);

            for (int i = 0; i < 20; i++)
            {
                PutItemToExpire(1024 * 100);
                //System.Threading.Thread.Sleep(10);
            }

            var expirer = new DynamoDBExpirer(DynamoDBManagerConnString);

            expirer.RemoveExpiredItems();
        }

        [TestMethod]
        public void DynamoDBBigDataAddTest()
        {
            ChangeConnString(DynamoDBConnString);
            ChangeProvider(ProviderTypeOptions.DynamoDB);
            ChangeFormatterType(FormatterTypeOptions.Binary);

            var key = PutItemToExpire(358400 * 3);

            var store = new SessionStore();

            var item = store.Get<TestData>(Category, key);

            Assert.IsNotNull(item);
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

        private static string PutItemToExpire(int sizeInBytes)
        {
            var data = new byte[sizeInBytes];

            for (var i = 0; i < sizeInBytes; i++)
            {
                data[i] = Convert.ToByte(i.ToString()[0]);
            }

            var store = new SessionStore();

            var key = Guid.NewGuid().ToString();

            store.Add(Category, key, new TestData()
            {
                FirstName = Encoding.UTF8.GetString(data)
            }, TimeSpan.FromMilliseconds(5));

            return key;
        }
    }
}
