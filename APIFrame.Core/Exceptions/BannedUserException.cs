using System;

namespace APIFrame.Core.Exceptions
{
    public class BannedUserException : Exception
    {
        public BannedUserException(string subject)
            : base ($"Subject '{subject}' is banned.")
        {
        }
    }
}
