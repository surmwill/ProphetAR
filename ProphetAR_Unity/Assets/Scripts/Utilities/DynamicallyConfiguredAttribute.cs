using System;

namespace ProphetAR
{
    /// <summary>
    /// Indicates that a field in a configuration is added dynamically by something in the level, rather than being passed into the level and known beforehand
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class DynamicallyConfiguredAttribute : Attribute
    {
    }
}