using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tavisca.Frameworks.Session.Formatters
{
    public interface ISessionDataFormatter
    {
        byte[] Format(object obj);
        T FromFormatted<T>(byte[] array);
    }
}