using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tavisca.Frameworks.Session.Configuration;
using Tavisca.Frameworks.Session.Exceptions;
using Tavisca.Frameworks.Session.Infrastructure;
using Tavisca.Frameworks.Session.Resources;

namespace Tavisca.Frameworks.Session.Formatters
{
    public static class FormatterFactory
    {
        public static ISessionDataFormatter GetFormatter(ISessionConfiguration configuration)
        {
            var formatter = GetFormatterFromType(configuration.Formatter, configuration.CustomFormatterType);

            return formatter;
        }

        private static readonly ConcurrentDictionary<string, ISessionDataFormatter> CustomFormatters = new ConcurrentDictionary<string, ISessionDataFormatter>();

        public static ISessionDataFormatter GetFormatterFromType(FormatterTypeOptions formatterType, string customType)
        {
            switch (formatterType)
            {
                case FormatterTypeOptions.Binary:
                    return new BinaryFormatter();
                case FormatterTypeOptions.BinaryCompressed:
                    return new BinaryCompressionFormatter();
                case FormatterTypeOptions.BinaryLz4Compressed:
                    return new BinaryLz4CompressionFormatter();
                case FormatterTypeOptions.ProtoBuf:
                    return new ProtoBufFormatter();
                case FormatterTypeOptions.ProtoBufCompressed:
                    return new ProtoBufCompressionFormatter();
                case FormatterTypeOptions.ProtoBufLz4Compressed:
                    return new ProtoBufLz4CompressionFormatter();
                case FormatterTypeOptions.Json:
                    return new JsonFormatter();
                case FormatterTypeOptions.JsonCompressed:
                    return new JsonCompressionFormatter();
                case FormatterTypeOptions.JsonLz4Compressed:
                    return new JsonLz4CompressionFormatter();
                case FormatterTypeOptions.Custom:
                default:
                    if (string.IsNullOrWhiteSpace(customType))
                        throw new SessionConfigurationException(SessionResources.Formatter_Missing);

                    var formatter = CustomFormatters.GetOrAdd(customType, s =>
                        {
                            var provider = s.Construct();

                            var retVal = provider as ISessionDataFormatter;

                            if (retVal == null)
                                throw new SessionConfigurationException(
                                    string.Format(SessionResources.Formatter_InvalidType,
                                                  typeof (ISessionDataFormatter).FullName));

                            return retVal;
                        });

                    return formatter;
            }
        }
    }
}
