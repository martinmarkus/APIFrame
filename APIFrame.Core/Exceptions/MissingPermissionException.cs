using System;

namespace APIFrame.Core.Exceptions
{
    public class MissingPermissionException : Exception
    {
        public MissingPermissionException(string subject, string operation) 
            : base($"Subject '{subject}' does not have enough permission for operation {operation}.")
        {
        }
    }
}
