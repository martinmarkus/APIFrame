using System;

namespace APIFrame.BackgroundService.Exceptions
{
    public class MissingDependencyException : Exception
    {
        public MissingDependencyException(string dependency) : base($"Missing dependency '{dependency}'.")
        {
        }
    }
}
