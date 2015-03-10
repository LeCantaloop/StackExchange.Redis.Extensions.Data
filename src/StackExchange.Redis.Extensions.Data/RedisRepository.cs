using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using StackExchange.Redis.Extensions.Core;

namespace StackExchange.Redis.Extensions.Data
{
    public class RedisRepository
    {
        private readonly ICacheClient _cacheClient;
        private readonly ISerializer _serializer;
        private readonly IKeyGenerator _keyGenerator;
        private readonly IMultipleKeyGenerator _multipleKeyGenerator;

        private RedisRepository(ICacheClient cacheClient, ISerializer serializer, IKeyGenerator keyGenerator, IMultipleKeyGenerator multipleKeyGenerator)
        {
            _keyGenerator = keyGenerator;
            _multipleKeyGenerator = multipleKeyGenerator;
            _cacheClient = cacheClient;
            _serializer = serializer;
        }

        private ICacheClient CacheClient
        {
            get { return _cacheClient; }
        }

        public static RedisRepository GetInstance(ICacheClient cacheClient, ISerializer serializer, IKeyGenerator keyGenerator, IMultipleKeyGenerator multipleKeyGenerator)
        {
            return new RedisRepository(cacheClient, serializer, keyGenerator, multipleKeyGenerator);
        }

        public static RedisRepository GetInstance(ICacheClient cacheClient)
        {
            var kg = new IdentityKeyGenerator();
            return GetInstance(cacheClient, cacheClient.Serializer, kg, new IndexKeyGenerator(kg));
        }

        public void AddOrUpdate<T>(T u) where T : class
        {
            // keyFormat + index property name
            // index property name + index property value + key property name
            foreach (var i in _multipleKeyGenerator.Generate(u))
            {
                CacheClient.Add(i.PropertyName, i.PropertyValue);
            }

            // Generate the hash key and hash entries
            var hashKey = KeyFormatter.Format(_keyGenerator.Generate(u).ToString(), "info");

            CacheClient.Database.HashSet(hashKey, GenerateHashEntries(u));
        }

        private HashEntry[] GenerateHashEntries<T>(T value) where T : class
        {
            return GenerateHashEntriesImpl(value).ToArray();
        }

        private IEnumerable<HashEntry> GenerateHashEntriesImpl<T>(T value)
            where T : class
        {
            var props = value.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);
            foreach (var p in props)
            {
                var propName = p.Name.ToLowerInvariant();
                var propValue = p.GetValue(value);
                var serializedPropValue = _serializer.Serialize(propValue);
                yield return new HashEntry(propName, serializedPropValue);
            }
        }

        public TReturnType Get<TLookup, TReturnType>(TLookup id)
        {
            var key = _keyGenerator.Generate(default(TReturnType));
            key.PropertyValue = id.ToString().ToLowerInvariant();

            var hashKey = KeyFormatter.Format(key.ToString(), "info");
            var he = CacheClient.Database.HashGetAll(hashKey);

            // Build the object from the Name/Value of the HE
            var o = Activator.CreateInstance<TReturnType>();
            var p = typeof(TReturnType).GetProperties(BindingFlags.Public | BindingFlags.Instance).ToDictionary(p1 => p1.Name, e => e, StringComparer.OrdinalIgnoreCase);

            foreach (var h in he)
            {
                var p1 = p[h.Name];
                if (!p1.CanWrite) continue;

                var pt = p1.PropertyType;
                var deserializedValue = _serializer.Deserialize(h.Value);
                var convertedValue = Convert.ChangeType(deserializedValue, pt);
                p1.SetValue(o, convertedValue);
            }

            return o;
        }
    }
}