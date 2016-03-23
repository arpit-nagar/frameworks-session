namespace Tavisca.Frameworks.Session.Provider.DynamoDB
{
    internal static class DynamoDBMeta
    {
        public const string IdColumn = "id";
        public const string ExpiryStaticColumn = "e";
        public const string ExpiryEpochColumn = "exEpochSec";
        public const string DataColumn = "data";
        public const string ItemCountColumn = "i";

        public const int MaxSecondaryIndexVal = 100;
    }
}