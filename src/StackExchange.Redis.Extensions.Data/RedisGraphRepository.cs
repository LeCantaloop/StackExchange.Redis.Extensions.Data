using System.Collections.Generic;
using StackExchange.Redis.Extensions.Core;

namespace StackExchange.Redis.Extensions.Data
{
    public class RedisGraphRepository
    {
        private readonly ICacheClient _cacheClient;
        private readonly IKeyGenerator _keyGenerator;
        private readonly string _relationshipName;

        private RedisGraphRepository(ICacheClient cacheClient, IKeyGenerator keyGenerator, string relationshipName)
        {
            _keyGenerator = keyGenerator;
            _relationshipName = relationshipName;
            _cacheClient = cacheClient;
        }

        private ICacheClient CacheClient
        {
            get { return _cacheClient; }
        }

        public static RedisGraphRepository GetInstance(ICacheClient cacheClient, IKeyGenerator keyGenerator, string relationshipName)
        {
            return new RedisGraphRepository(cacheClient, keyGenerator, relationshipName);
        }

        public static RedisGraphRepository GetInstance(ICacheClient cacheClient, string relationshipName)
        {
            return GetInstance(cacheClient, new IdentityKeyGenerator(), relationshipName);
        }

        public void AddUnidirectionalEdge<T>(T a, T b) where T : class
        {
            var edgeKey = GenerateEdgeKey(a);
            var k2 = _keyGenerator.Generate(b);
            CacheClient.SetAdd<string>(edgeKey, k2.PropertyValue);
        }

        private string GenerateEdgeKey<T>(T a)
        {
            var k = _keyGenerator.Generate(a);
            return KeyFormatter.Format(k.ToString(), _relationshipName)
            ;
        }

        public void AddBiDirectionalEdge<T>(T a, T b) where T : class
        {
            AddUnidirectionalEdge(a, b);
            AddUnidirectionalEdge(b, a);
        }

        public void RemoveUnidirectionalEdge<T>(T a, T b)
        {
            var edgeKey = GenerateEdgeKey(a);
            var k2 = _keyGenerator.Generate(b);
            CacheClient.Database.SetRemove(edgeKey, k2.PropertyValue);
        }

        public IEnumerable<string> Members<T>(T a)
        {
            var edgeKey = GenerateEdgeKey(a);
            return CacheClient.SetMember(edgeKey);
        }

        public bool Member<T>(T a, string key)
        {
            var edgeKey = GenerateEdgeKey(a);
            return CacheClient.Database.SetContains(edgeKey, key);
        }

        public bool Member<T>(T a, T b)
        {
            var k2 = _keyGenerator.Generate(b);
            return Member(a, k2.PropertyValue);
        }
    }
}