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
    internal sealed class BinaryLz4CompressionFormatter: BinaryFormatterBase
    {
        public BinaryLz4CompressionFormatter()
            : base(new SerializationFactory()
                                        .GetSerializationFacade(SerializerType.Binary,
                                        CompressionTypeOptions.Lz4))
        { }
    }
}
