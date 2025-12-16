using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Polylib
{
    internal class BadModulusException : Exception
    {
        // Constructors
        public BadModulusException() : base() { }

        public BadModulusException(string message) : base(message) { }

        public BadModulusException(string message, Exception innerException) : base(message, innerException) { }

        // You can add additional properties or methods as needed
        public int ErrorCode { get; set; }
    }
}
