using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace StackExchange.Redis.Extensions.Data
{
    public class KeyAttributeKeyGenerator : IKeyGenerator
    {
        public Key Generate<T>(T o)
        {
            var t = typeof(T);

            var props = t.GetProperties(BindingFlags.Instance | BindingFlags.Public);

            foreach (var prop in props)
            {
                var attr = prop.GetCustomAttributes<KeyAttribute>(true);
                if (attr.Any())
                {
                    return new Key(prop.Name, o == null ? null : prop.GetValue(o).ToString());
                }
            }

            var k = new PropertyKeyGenerator("Id");
            var retval = k.Generate<T>(o);
            if (retval != null)
            {
                return retval;
            }

            k = new PropertyKeyGenerator(t.Name + "Id");
            return k.Generate(o);
        }
    }
}