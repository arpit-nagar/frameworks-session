using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tavisca.Frameworks.Session.Exceptions
{
    [Serializable]
    public class SessionConfigurationException : SessionException
    {
        public SessionConfigurationException() { }

        public SessionConfigurationException(string message) : base(message) { }

        public SessionConfigurationException(string message, Exception innerException) : base(message, innerException) { }
    }
}
