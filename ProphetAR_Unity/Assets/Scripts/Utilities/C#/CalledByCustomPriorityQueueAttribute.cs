using System;

namespace ProphetAR
{
    /// <summary>
    /// Indicates that a method is intended to be called by the internals of the CustomPriorityQueue, and not the user.
    /// The user need not worry about this method.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class CalledByCustomPriorityQueueAttribute : Attribute
    {
        
    }
}