using System;
using System.Reflection;

namespace StackExchange.Redis.Extensions.Data
{
    public class PropertyKeyGenerator : IKeyGenerator
    {
        private readonly string _propertyName;

        public PropertyKeyGenerator(string propertyName)
        {
            _propertyName = propertyName;
        }

        private static Key Generate<T>(T o, string propertyName)
        {
            return GenerateImpl(typeof(T), o, propertyName);
        }

        private static Key GenerateImpl(Type o, object i, string propertyName)
        {
            var props = o.GetProperty(propertyName, BindingFlags.Public | BindingFlags.IgnoreCase | BindingFlags.Instance);

            if (props == null) return null;

            if (props.Name.StartsWith(o.Name, StringComparison.OrdinalIgnoreCase) || !propertyName.Equals("Id", StringComparison.OrdinalIgnoreCase))
            {
                return new Key(props.Name, i == null ? null : props.GetValue(i)?.ToString());
            }
            return new Key((o.Name + props.Name), i == null ? null : props.GetValue(i)?.ToString());
        }

        public Key Generate<T>(T o)
        {
            return Generate(o, _propertyName);
        }
    }
}