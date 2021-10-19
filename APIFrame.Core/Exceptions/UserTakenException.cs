using System;

namespace APIFrame.Core.Exceptions
{
    public class UserTakenException : Exception
    {
        public UserTakenException(string subject)
            : base($"Subject '{subject}' does is already registered.")
        {
        }
    }
}
