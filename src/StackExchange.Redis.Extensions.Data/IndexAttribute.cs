using System;

namespace StackExchange.Redis.Extensions.Data
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class IndexAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new <see cref="IndexAttribute"/> for an index that will be named by convention
        /// </summary>
        public IndexAttribute()
        {
        }
    }
}