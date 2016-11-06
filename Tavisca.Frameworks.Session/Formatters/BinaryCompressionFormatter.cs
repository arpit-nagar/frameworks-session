using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Runtime.Serialization.Formatters.Binary;
using Tavisca.Frameworks.Serialization;
using Tavisca.Frameworks.Serialization.Compression;
using Tavisca.Frameworks.Serialization.Configuration;

namespace Tavisca.Frameworks.Session.Formatters
{
    internal sealed class BinaryCompressionFormatter : BinaryFormatterBase
    {
        public BinaryCompressionFormatter() : base(new SerializationFactory()
                                        .GetSerializationFacade(SerializerType.Binary, CompressionTypeOptions.Deflate))
        { }
    }
}
