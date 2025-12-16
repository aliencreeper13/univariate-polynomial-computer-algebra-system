using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Polylib
{
    internal class DomainErrorException : Exception
    {
        public DomainErrorException() { }
        public DomainErrorException(string message) : base(message) { }
        public DomainErrorException(string message, Exception innerException) : base(message, innerException) { }
        public int ErrorCode { get; set; }
    }
}
