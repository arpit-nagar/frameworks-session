using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Tavisca.Frameworks.Session.Provider.DynamoDB;

namespace DynamoDBExpirationHost
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            RunExpirations();
        }

        private static void RunExpirations()
        {
            try
            {
                var connections = GetTargetConnections();

                foreach (var connection in connections)
                {
                    ExpireForConnection(connection);
                }
            }
            catch(Exception ex)
            {
                throw new Exception("DynamoDBExpirationHost run failed. See inner exception for more details.", ex);
            }
        }

        private static void ExpireForConnection(string connection)
        {
            var expirer = new DynamoDBExpirer(connection);

            expirer.RemoveExpiredItems();
        }

        private static IEnumerable<string> GetTargetConnections()
        {
            var targetKeyVal = ConfigurationManager.AppSettings["targetConnectionKeys"];

            if (string.IsNullOrWhiteSpace(targetKeyVal))
                throw new Exception("targetConnectionKeys (app.config) is not defined or empty in the configuration file.");

            var targetKeys = targetKeyVal.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            if (targetKeys.Length == 0)
                throw new Exception("targetConnectionKeys (app.config) does not have any valid entries.");


            foreach (var targetKey in targetKeys)
            {
                var connString = ConfigurationManager.AppSettings[targetKey];

                if (string.IsNullOrWhiteSpace(connString))
                    throw new Exception("targetConnectionKeys (app.config) has a reference key which yielded an empty result, fix this. The key was " + targetKey);

                yield return connString;
            }
        }
    }
}
