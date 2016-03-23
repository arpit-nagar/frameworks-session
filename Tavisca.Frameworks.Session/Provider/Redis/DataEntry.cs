using System;

namespace Tavisca.Frameworks.Session.Provider.Redis
{
    public class DataEntry
    {
        public Guid Id { get; set; }
        public string ObjectName { get; set; }
        public byte[] ObjectValue { get; set; }
        public DateTime? AddedOn { get; set; }
        public DateTime? ExpiresOn { get; set; }
    }
}
