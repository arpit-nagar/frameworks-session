using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tavisca.Frameworks.Serialization;
using Tavisca.Frameworks.Serialization.Compression;
using Tavisca.Frameworks.Serialization.Configuration;

namespace Tavisca.Frameworks.Session.Formatters
{
    internal abstract class BinaryFormatterBase : ISessionDataFormatter
    {
        private readonly ISerializationFacade _serializationFacade;

        protected BinaryFormatterBase(ISerializationFacade serializationFacade)
        {
            _serializationFacade = serializationFacade;
        }

        public byte[] Format(object obj)
        {
            return _serializationFacade.Serialize(obj);
        }

        public T FromFormatted<T>(byte[] array)
        {
            return _serializationFacade.Deserialize<T>(array);
        }
    }
}
