using System;
using System.Collections.Generic;
using System.Text;

namespace cuteDVDCore.Exceptions
{
    public class ExceptionDriveNotReady : Exception
    {
        public ExceptionDriveNotReady()
        {
        }

        public ExceptionDriveNotReady(string? message) : base(message)
        {
        }
    }
}
