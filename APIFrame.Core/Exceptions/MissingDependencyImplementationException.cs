using System;

namespace APIFrame.Core.Exceptions
{
    public class MissingDependencyImplementationException : Exception
    {
        public MissingDependencyImplementationException(string interfaceFullName, string searchNamespace) : base($"No implementation was found for '{interfaceFullName}' in namespace '{searchNamespace}'.")
        {
        }
    }
}
