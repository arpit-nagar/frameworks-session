using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tavisca.Frameworks.Session.Formatters
{
    public enum FormatterTypeOptions
    {
        BinaryCompressed = 0,
        Binary = 1,
        ProtoBuf = 2,
        ProtoBufCompressed = 3,
        Json = 4,
        JsonCompressed = 5,
        Custom = 6,
        ProtoBufLz4Compressed = 7,
        BinaryLz4Compressed = 8,
        JsonLz4Compressed = 9
    }
}
