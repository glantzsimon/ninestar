using System;
using System.Reflection;

namespace K9.SharedLibrary.Exceptions
{
    public class FailedToMapPropertyException : ApplicationException
    {
        private PropertyInfo MapToProperty { get; }

        public FailedToMapPropertyException(PropertyInfo mapToProperty, Exception ex) : base(
            $"An error occurred whilst trying to map property '{mapToProperty.Name}'.", ex)
        {
            MapToProperty = mapToProperty;
        }
    }
}
