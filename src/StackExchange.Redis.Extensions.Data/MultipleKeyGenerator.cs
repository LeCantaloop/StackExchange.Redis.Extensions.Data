using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace StackExchange.Redis.Extensions.Data
{
    public class MultipleKeyGenerator : IMultipleKeyGenerator
    {
        private readonly IKeyGenerator _keyGenerator;

        public MultipleKeyGenerator(IKeyGenerator keyGenerator)
        {
            _keyGenerator = keyGenerator;
        }

        public IEnumerable<Key> Generate<T>(T o)
        {
            if (o == null) yield return null;
            var props = o.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);
            if (props == null || !props.Any()) yield return null;
            foreach (var prop in props)
            {
                var attr = prop.GetCustomAttributes<IndexAttribute>(true);
                if (attr.Any())
                {
                    var tkg = new PropertyKeyGenerator(prop.Name);
                    var index = _keyGenerator.Generate(o);
                    var v1 = new Key(index + ":" + prop.Name.ToLowerInvariant(), prop.GetValue(o).ToString().ToLowerInvariant());
                    var v2 = new Key(tkg.Generate(o) + ":" + index.PropertyName, index.PropertyValue);

                    yield return v1;
                    yield return v2;
                }
            }
        }
    }
}