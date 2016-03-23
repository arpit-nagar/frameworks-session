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
    internal sealed class ProtoBufCompressionFormatter : BinaryFormatterBase
    {
        public ProtoBufCompressionFormatter()
            : base(new SerializationFactory()
                                        .GetSerializationFacade(SerializerType.ProtoBuf, CompressionTypeOptions.Deflate))
        { }
    }
}
