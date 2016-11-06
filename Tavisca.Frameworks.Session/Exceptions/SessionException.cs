using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tavisca.Frameworks.Session.Exceptions
{
    [Serializable]
    public class SessionException : Exception
    {
        public SessionException() { }

        public SessionException(string message) : base(message) { }

        public SessionException(string message, Exception innerException) : base(message, innerException) { }
    }
}
