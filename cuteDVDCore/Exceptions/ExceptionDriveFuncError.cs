using System;
using System.Collections.Generic;
using System.Text;

namespace cuteDVDCore.Exceptions
{
    public class ExceptionDriveFuncError : Exception
    {
        public ExceptionDriveFuncError()
        {
        }

        public ExceptionDriveFuncError(string? message) : base(message)
        {
        }
    }
}
