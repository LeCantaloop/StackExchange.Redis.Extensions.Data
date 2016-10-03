using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StackExchange.Redis.Extensions.Core;
using StackExchange.Redis.Extensions.Newtonsoft;

namespace StackExchange.Redis.Extensions.Data.Tests
{
    [TestClass()]
    public class RedisRepositoryTests
    {
        [TestMethod()]
        public void AddOrUpdateTest()
        {
            var repository = RedisRepository.GetInstance(new MockCacheClient());

            //TODO: Request next key
            var u0 = new User
            {
                UserId = 1000,
                Age = 30,
                Email = "John.smith@example.com",
                Gender = "M"
            };
            repository.AddOrUpdate(u0);

            var u1 = repository.Get<int, User>(u0.UserId);

            Assert.AreEqual(u0.Age, u1.Age);
            Assert.AreEqual(u0.Email, u1.Email);
            Assert.AreEqual(u0.Gender, u1.Gender);
            Assert.AreEqual(u0.UserId, u1.UserId);
        }

        [TestMethod]
        public void AddBidirectionEdgeTest()
        {
            var graphRepo = RedisGraphRepository.GetInstance(new MockCacheClient(), "friends");

            var u0 = new User
            {
                UserId = 1000,
                Age = 30,
                Email = "John.smith@example.com",
                Gender = "M"
            };

            var u1 = new User
            {
                UserId = 1001,
                Age = 28,
                Email = "Jane.smith@example.com",
                Gender = "F"
            };

            graphRepo.AddBiDirectionalEdge(u0, u1);

            var k = graphRepo.Members(u0).First();
            Assert.AreEqual("1001", k);
            Assert.IsTrue( graphRepo.Member(u0, u1.UserId.ToString()));
            Assert.IsTrue(graphRepo.Member(u0,u1 ));

            k = graphRepo.Members(u1).First();
            Assert.AreEqual("1000", k);
            Assert.IsTrue(graphRepo.Member(u1, "1000"));
            Assert.IsTrue(graphRepo.Member(u1, u0));
        }

        private class User
        {
            [Key]
            public int UserId { get; set; }

            [EmailAddress]
            [Index]
            public string Email { get; set; }

            public int Age { get; set; }

            [MaxLength(1)]
            public string Gender { get; set; }
        }

        private class MockCacheClient : ICacheClient
        {
            private readonly MockDatabase _db;
            private readonly NewtonsoftSerializer _serializer;

            public MockCacheClient()
            {
                _db = new MockDatabase();
                _serializer = new NewtonsoftSerializer();
            }

            public void Dispose()
            {
            }

            public bool Exists(string key)
            {
                return false;
            }

            public Task<bool> ExistsAsync(string key)
            {
                return null;
            }

            public bool Remove(string key)
            {
                return false;
            }

            public Task<bool> RemoveAsync(string key)
            {
                return null;
            }

            public void RemoveAll(IEnumerable<string> keys)
            {
            }

            public Task RemoveAllAsync(IEnumerable<string> keys)
            {
                return null;
            }

            public T Get<T>(string key)
            {
                return default(T);
            }

            public Task<T> GetAsync<T>(string key)
            {
                return null;
            }

            public bool Add<T>(string key, T value)
            {
                Debug.WriteLine(key + " : " + value);
                Database.StringSet(key, Serializer.Serialize(value));

                return true;
            }

            public Task<bool> AddAsync<T>(string key, T value)
            {
                return null;
            }

            public bool Replace<T>(string key, T value)
            {
                return false;
            }

            public Task<bool> ReplaceAsync<T>(string key, T value)
            {
                return null;
            }

            public bool Add<T>(string key, T value, DateTimeOffset expiresAt)
            {
                return false;
            }

            public Task<bool> AddAsync<T>(string key, T value, DateTimeOffset expiresAt)
            {
                return null;
            }

            public bool Replace<T>(string key, T value, DateTimeOffset expiresAt)
            {
                return false;
            }

            public Task<bool> ReplaceAsync<T>(string key, T value, DateTimeOffset expiresAt)
            {
                return null;
            }

            public bool Add<T>(string key, T value, TimeSpan expiresIn)
            {
                return false;
            }

            public Task<bool> AddAsync<T>(string key, T value, TimeSpan expiresIn)
            {
                return null;
            }

            public bool Replace<T>(string key, T value, TimeSpan expiresIn)
            {
                return false;
            }

            public Task<bool> ReplaceAsync<T>(string key, T value, TimeSpan expiresIn)
            {
                return null;
            }

            public IDictionary<string, T> GetAll<T>(IEnumerable<string> keys)
            {
                return null;
            }

            public Task<IDictionary<string, T>> GetAllAsync<T>(IEnumerable<string> keys)
            {
                return null;
            }

            public bool AddAll<T>(IList<Tuple<string, T>> items)
            {
                return false;
            }

            public Task<bool> AddAllAsync<T>(IList<Tuple<string, T>> items)
            {
                return null;
            }



            public bool SetAdd(string memberName, string key)
            {
                Database.SetAdd(memberName, key);

                return true;
            }

            public Task<bool> SetAddAsync(string memberName, string key)
            {
                return null;
            }

            public bool SetAdd<T>(string key, T item) where T : class
            {
                Database.SetAdd(key, item.ToString());
                return true;
            }

            public Task<bool> SetAddAsync<T>(string key, T item) where T : class
            {
                throw new NotImplementedException();
            }

            public string[] SetMember(string memberName)
            {
                return Database.SetMembers(memberName).Select(s=>s.ToString()).ToArray();
            }

            public Task<string[]> SetMemberAsync(string memberName)
            {
                return null;
            }

            public Task<IEnumerable<T>> SetMembersAsync<T>(string key)
            {
                return null;
            }

            public IEnumerable<string> SearchKeys(string pattern)
            {
                yield break;
            }

            public Task<IEnumerable<string>> SearchKeysAsync(string pattern)
            {
                return null;
            }

            public void FlushDb()
            {
            }

            public Task FlushDbAsync()
            {
                return null;
            }

            public void Save(SaveType saveType)
            {
                throw new NotImplementedException();
            }

            public void SaveAsync(SaveType saveType)
            {
                throw new NotImplementedException();
            }

            public Dictionary<string, string> GetInfo()
            {
                return null;
            }

            public Task<Dictionary<string, string>> GetInfoAsync()
            {
                return null;
            }

            public long Publish<T>(RedisChannel channel, T message, CommandFlags flags = CommandFlags.None)
            {
                throw new NotImplementedException();
            }

            public Task<long> PublishAsync<T>(RedisChannel channel, T message, CommandFlags flags = CommandFlags.None)
            {
                throw new NotImplementedException();
            }

            public void Subscribe<T>(RedisChannel channel, Action<T> handler, CommandFlags flags = CommandFlags.None)
            {
                throw new NotImplementedException();
            }

            public Task SubscribeAsync<T>(RedisChannel channel, Func<T, Task> handler, CommandFlags flags = CommandFlags.None)
            {
                throw new NotImplementedException();
            }

            public void Unsubscribe<T>(RedisChannel channel, Action<T> handler, CommandFlags flags = CommandFlags.None)
            {
                throw new NotImplementedException();
            }

            public Task UnsubscribeAsync<T>(RedisChannel channel, Func<T, Task> handler, CommandFlags flags = CommandFlags.None)
            {
                throw new NotImplementedException();
            }

            public void UnsubscribeAll(CommandFlags flags = CommandFlags.None)
            {
                throw new NotImplementedException();
            }

            public Task UnsubscribeAllAsync(CommandFlags flags = CommandFlags.None)
            {
                throw new NotImplementedException();
            }

            public long ListAddToLeft<T>(string key, T item) where T : class
            {
                throw new NotImplementedException();
            }

            public Task<long> ListAddToLeftAsync<T>(string key, T item) where T : class
            {
                throw new NotImplementedException();
            }

            public T ListGetFromRight<T>(string key) where T : class
            {
                throw new NotImplementedException();
            }

            public Task<T> ListGetFromRightAsync<T>(string key) where T : class
            {
                throw new NotImplementedException();
            }

            public bool HashDelete(string hashKey, string key, CommandFlags commandFlags = CommandFlags.None)
            {
                return false;
            }

            public long HashDelete(string hashKey, IEnumerable<string> keys, CommandFlags commandFlags = CommandFlags.None)
            {
                return 0;
            }

            public bool HashExists(string hashKey, string key, CommandFlags commandFlags = CommandFlags.None)
            {
                return false;
            }

            public T HashGet<T>(string hashKey, string key, CommandFlags commandFlags = CommandFlags.None)
            {
                return default(T);
            }

            public Dictionary<string, T> HashGet<T>(string hashKey, IEnumerable<string> keys, CommandFlags commandFlags = CommandFlags.None)
            {
                return null;
            }

            public Dictionary<string, T> HashGetAll<T>(string hashKey, CommandFlags commandFlags = CommandFlags.None)
            {
                return null;
            }

            public long HashIncerementBy(string hashKey, string key, long value, CommandFlags commandFlags = CommandFlags.None)
            {
                return 0;
            }

            public double HashIncerementBy(string hashKey, string key, double value, CommandFlags commandFlags = CommandFlags.None)
            {
                return 0;
            }

            public IEnumerable<string> HashKeys(string hashKey, CommandFlags commandFlags = CommandFlags.None)
            {
                yield break;
            }

            public long HashLength(string hashKey, CommandFlags commandFlags = CommandFlags.None)
            {
                return 0;
            }

            public bool HashSet<T>(string hashKey, string key, T value, bool nx = false, CommandFlags commandFlags = CommandFlags.None)
            {
                return false;
            }

            public void HashSet<T>(string hashKey, Dictionary<string, T> values, CommandFlags commandFlags = CommandFlags.None)
            {
            }

            public IEnumerable<T> HashValues<T>(string hashKey, CommandFlags commandFlags = CommandFlags.None)
            {
                yield break;
            }

            public Dictionary<string, T> HashScan<T>(string hashKey, string pattern, int pageSize = 10, CommandFlags commandFlags = CommandFlags.None)
            {
                return null;
            }

            public Task<bool> HashDeleteAsync(string hashKey, string key, CommandFlags commandFlags = CommandFlags.None)
            {
                return null;
            }

            public Task<long> HashDeleteAsync(string hashKey, IEnumerable<string> keys, CommandFlags commandFlags = CommandFlags.None)
            {
                return null;
            }

            public Task<bool> HashExistsAsync(string hashKey, string key, CommandFlags commandFlags = CommandFlags.None)
            {
                return null;
            }

            public Task<T> HashGetAsync<T>(string hashKey, string key, CommandFlags commandFlags = CommandFlags.None)
            {
                return null;
            }

            public Task<Dictionary<string, T>> HashGetAsync<T>(string hashKey, IEnumerable<string> keys, CommandFlags commandFlags = CommandFlags.None)
            {
                return null;
            }

            public Task<Dictionary<string, T>> HashGetAllAsync<T>(string hashKey, CommandFlags commandFlags = CommandFlags.None)
            {
                return null;
            }

            public Task<long> HashIncerementByAsync(string hashKey, string key, long value, CommandFlags commandFlags = CommandFlags.None)
            {
                return null;
            }

            public Task<double> HashIncerementByAsync(string hashKey, string key, double value, CommandFlags commandFlags = CommandFlags.None)
            {
                return null;
            }

            public Task<IEnumerable<string>> HashKeysAsync(string hashKey, CommandFlags commandFlags = CommandFlags.None)
            {
                return null;
            }

            public Task<long> HashLengthAsync(string hashKey, CommandFlags commandFlags = CommandFlags.None)
            {
                return null;
            }

            public Task<bool> HashSetAsync<T>(string hashKey, string key, T value, bool nx = false, CommandFlags commandFlags = CommandFlags.None)
            {
                return null;
            }

            public Task HashSetAsync<T>(string hashKey, IDictionary<string, T> values, CommandFlags commandFlags = CommandFlags.None)
            {
                return null;
            }

            public Task<IEnumerable<T>> HashValuesAsync<T>(string hashKey, CommandFlags commandFlags = CommandFlags.None)
            {
                return null;
            }

            public Task<Dictionary<string, T>> HashScanAsync<T>(string hashKey, string pattern, int pageSize = 10, CommandFlags commandFlags = CommandFlags.None)
            {
                return null;
            }

            public StackExchange.Redis.IDatabase Database
            {
                get { return _db; }
            }

            public ISerializer Serializer
            {
                get { return _serializer; }
            }

            private class MockDatabase : IDatabase
            {
                public Task<TimeSpan> PingAsync(CommandFlags flags = CommandFlags.None)
                {
                    return null;
                }

                public bool TryWait(Task task)
                {
                    return false;
                }

                public void Wait(Task task)
                {
                }

                public T Wait<T>(Task<T> task)
                {
                    return default(T);
                }

                public void WaitAll(params Task[] tasks)
                {
                }

                public ConnectionMultiplexer Multiplexer
                {
                    get { return null; }
                }

                public TimeSpan Ping(CommandFlags flags = CommandFlags.None)
                {
                    return new TimeSpan();
                }

                public Task<RedisValue> DebugObjectAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
                {
                    return null;
                }

                public Task<long> HashDecrementAsync(RedisKey key, RedisValue hashField, long value = 1, CommandFlags flags = CommandFlags.None)
                {
                    return null;
                }

                public Task<double> HashDecrementAsync(RedisKey key, RedisValue hashField, double value, CommandFlags flags = CommandFlags.None)
                {
                    return null;
                }

                public Task<bool> HashDeleteAsync(RedisKey key, RedisValue hashField, CommandFlags flags = CommandFlags.None)
                {
                    return null;
                }

                public Task<long> HashDeleteAsync(RedisKey key, RedisValue[] hashFields, CommandFlags flags = CommandFlags.None)
                {
                    return null;
                }

                public Task<bool> HashExistsAsync(RedisKey key, RedisValue hashField, CommandFlags flags = CommandFlags.None)
                {
                    return null;
                }

                public Task<HashEntry[]> HashGetAllAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
                {
                    return null;
                }

                public Task<RedisValue> HashGetAsync(RedisKey key, RedisValue hashField, CommandFlags flags = CommandFlags.None)
                {
                    return null;
                }

                public Task<RedisValue[]> HashGetAsync(RedisKey key, RedisValue[] hashFields, CommandFlags flags = CommandFlags.None)
                {
                    return null;
                }

                public Task<long> HashIncrementAsync(RedisKey key, RedisValue hashField, long value = 1, CommandFlags flags = CommandFlags.None)
                {
                    return null;
                }

                public Task<double> HashIncrementAsync(RedisKey key, RedisValue hashField, double value, CommandFlags flags = CommandFlags.None)
                {
                    return null;
                }

                public Task<RedisValue[]> HashKeysAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
                {
                    return null;
                }

                public Task<long> HashLengthAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
                {
                    return null;
                }

                public Task HashSetAsync(RedisKey key, HashEntry[] hashFields, CommandFlags flags = CommandFlags.None)
                {
                    return null;
                }

                public Task<bool> HashSetAsync(RedisKey key, RedisValue hashField, RedisValue value, When when = When.Always, CommandFlags flags = CommandFlags.None)
                {
                    return null;
                }

                public Task<RedisValue[]> HashValuesAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
                {
                    return null;
                }

                public Task<bool> HyperLogLogAddAsync(RedisKey key, RedisValue value, CommandFlags flags = CommandFlags.None)
                {
                    return null;
                }

                public Task<bool> HyperLogLogAddAsync(RedisKey key, RedisValue[] values, CommandFlags flags = CommandFlags.None)
                {
                    return null;
                }

                public Task<long> HyperLogLogLengthAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
                {
                    return null;
                }

                public Task<long> HyperLogLogLengthAsync(RedisKey[] keys, CommandFlags flags = CommandFlags.None)
                {
                    return null;
                }

                public Task HyperLogLogMergeAsync(RedisKey destination, RedisKey first, RedisKey second, CommandFlags flags = CommandFlags.None)
                {
                    return null;
                }

                public Task HyperLogLogMergeAsync(RedisKey destination, RedisKey[] sourceKeys, CommandFlags flags = CommandFlags.None)
                {
                    return null;
                }

                public Task<EndPoint> IdentifyEndpointAsync(RedisKey key = new RedisKey(), CommandFlags flags = CommandFlags.None)
                {
                    return null;
                }

                public bool IsConnected(RedisKey key, CommandFlags flags = CommandFlags.None)
                {
                    return false;
                }

                public Task<bool> KeyDeleteAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
                {
                    return null;
                }

                public Task<long> KeyDeleteAsync(RedisKey[] keys, CommandFlags flags = CommandFlags.None)
                {
                    return null;
                }

                public Task<byte[]> KeyDumpAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
                {
                    return null;
                }

                public Task<bool> KeyExistsAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
                {
                    return null;
                }

                public Task<bool> KeyExpireAsync(RedisKey key, TimeSpan? expiry, CommandFlags flags = CommandFlags.None)
                {
                    return null;
                }

                public Task<bool> KeyExpireAsync(RedisKey key, DateTime? expiry, CommandFlags flags = CommandFlags.None)
                {
                    return null;
                }

                public Task KeyMigrateAsync(RedisKey key, EndPoint toServer, int toDatabase = 0, int timeoutMilliseconds = 0,
                    MigrateOptions migrateOptions = MigrateOptions.None, CommandFlags flags = CommandFlags.None)
                {
                    return null;
                }

                public Task<bool> KeyMoveAsync(RedisKey key, int database, CommandFlags flags = CommandFlags.None)
                {
                    return null;
                }

                public Task<bool> KeyPersistAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
                {
                    return null;
                }

                public Task<RedisKey> KeyRandomAsync(CommandFlags flags = CommandFlags.None)
                {
                    return null;
                }

                public Task<bool> KeyRenameAsync(RedisKey key, RedisKey newKey, When when = When.Always, CommandFlags flags = CommandFlags.None)
                {
                    return null;
                }

                public Task KeyRestoreAsync(RedisKey key, byte[] value, TimeSpan? expiry = null, CommandFlags flags = CommandFlags.None)
                {
                    return null;
                }

                public Task<TimeSpan?> KeyTimeToLiveAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
                {
                    return null;
                }

                public Task<RedisType> KeyTypeAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
                {
                    return null;
                }

                public Task<RedisValue> ListGetByIndexAsync(RedisKey key, long index, CommandFlags flags = CommandFlags.None)
                {
                    return null;
                }

                public Task<long> ListInsertAfterAsync(RedisKey key, RedisValue pivot, RedisValue value, CommandFlags flags = CommandFlags.None)
                {
                    return null;
                }

                public Task<long> ListInsertBeforeAsync(RedisKey key, RedisValue pivot, RedisValue value, CommandFlags flags = CommandFlags.None)
                {
                    return null;
                }

                public Task<RedisValue> ListLeftPopAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
                {
                    return null;
                }

                public Task<long> ListLeftPushAsync(RedisKey key, RedisValue value, When when = When.Always, CommandFlags flags = CommandFlags.None)
                {
                    return null;
                }

                public Task<long> ListLeftPushAsync(RedisKey key, RedisValue[] values, CommandFlags flags = CommandFlags.None)
                {
                    return null;
                }

                public Task<long> ListLengthAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
                {
                    return null;
                }

                public Task<RedisValue[]> ListRangeAsync(RedisKey key, long start = 0, long stop = -1, CommandFlags flags = CommandFlags.None)
                {
                    return null;
                }

                public Task<long> ListRemoveAsync(RedisKey key, RedisValue value, long count = 0, CommandFlags flags = CommandFlags.None)
                {
                    return null;
                }

                public Task<RedisValue> ListRightPopAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
                {
                    return null;
                }

                public Task<RedisValue> ListRightPopLeftPushAsync(RedisKey source, RedisKey destination, CommandFlags flags = CommandFlags.None)
                {
                    return null;
                }

                public Task<long> ListRightPushAsync(RedisKey key, RedisValue value, When when = When.Always, CommandFlags flags = CommandFlags.None)
                {
                    return null;
                }

                public Task<long> ListRightPushAsync(RedisKey key, RedisValue[] values, CommandFlags flags = CommandFlags.None)
                {
                    return null;
                }

                public Task ListSetByIndexAsync(RedisKey key, long index, RedisValue value, CommandFlags flags = CommandFlags.None)
                {
                    return null;
                }

                public Task ListTrimAsync(RedisKey key, long start, long stop, CommandFlags flags = CommandFlags.None)
                {
                    return null;
                }

                public Task<bool> LockExtendAsync(RedisKey key, RedisValue value, TimeSpan expiry, CommandFlags flags = CommandFlags.None)
                {
                    return null;
                }

                public Task<RedisValue> LockQueryAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
                {
                    return null;
                }

                public Task<bool> LockReleaseAsync(RedisKey key, RedisValue value, CommandFlags flags = CommandFlags.None)
                {
                    return null;
                }

                public Task<bool> LockTakeAsync(RedisKey key, RedisValue value, TimeSpan expiry, CommandFlags flags = CommandFlags.None)
                {
                    return null;
                }

                public Task<long> PublishAsync(RedisChannel channel, RedisValue message, CommandFlags flags = CommandFlags.None)
                {
                    return null;
                }

                public Task<RedisResult> ScriptEvaluateAsync(string script, RedisKey[] keys = null, RedisValue[] values = null, CommandFlags flags = CommandFlags.None)
                {
                    return null;
                }

                public Task<RedisResult> ScriptEvaluateAsync(byte[] hash, RedisKey[] keys = null, RedisValue[] values = null, CommandFlags flags = CommandFlags.None)
                {
                    return null;
                }

                public Task<RedisResult> ScriptEvaluateAsync(LuaScript script, object parameters = null, CommandFlags flags = CommandFlags.None)
                {
                    throw new NotImplementedException();
                }

                public Task<RedisResult> ScriptEvaluateAsync(LoadedLuaScript script, object parameters = null, CommandFlags flags = CommandFlags.None)
                {
                    throw new NotImplementedException();
                }

                public Task<bool> SetAddAsync(RedisKey key, RedisValue value, CommandFlags flags = CommandFlags.None)
                {
                    return null;
                }

                public Task<long> SetAddAsync(RedisKey key, RedisValue[] values, CommandFlags flags = CommandFlags.None)
                {
                    return null;
                }

                public Task<long> SetCombineAndStoreAsync(SetOperation operation, RedisKey destination, RedisKey first, RedisKey second,
                    CommandFlags flags = CommandFlags.None)
                {
                    return null;
                }

                public Task<long> SetCombineAndStoreAsync(SetOperation operation, RedisKey destination, RedisKey[] keys, CommandFlags flags = CommandFlags.None)
                {
                    return null;
                }

                public Task<RedisValue[]> SetCombineAsync(SetOperation operation, RedisKey first, RedisKey second, CommandFlags flags = CommandFlags.None)
                {
                    return null;
                }

                public Task<RedisValue[]> SetCombineAsync(SetOperation operation, RedisKey[] keys, CommandFlags flags = CommandFlags.None)
                {
                    return null;
                }

                public Task<bool> SetContainsAsync(RedisKey key, RedisValue value, CommandFlags flags = CommandFlags.None)
                {
                    return null;
                }

                public Task<long> SetLengthAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
                {
                    return null;
                }

                public Task<RedisValue[]> SetMembersAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
                {
                    return null;
                }

                public Task<bool> SetMoveAsync(RedisKey source, RedisKey destination, RedisValue value, CommandFlags flags = CommandFlags.None)
                {
                    return null;
                }

                public Task<RedisValue> SetPopAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
                {
                    return null;
                }

                public Task<RedisValue> SetRandomMemberAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
                {
                    return null;
                }

                public Task<RedisValue[]> SetRandomMembersAsync(RedisKey key, long count, CommandFlags flags = CommandFlags.None)
                {
                    return null;
                }

                public Task<bool> SetRemoveAsync(RedisKey key, RedisValue value, CommandFlags flags = CommandFlags.None)
                {
                    return null;
                }

                public Task<long> SetRemoveAsync(RedisKey key, RedisValue[] values, CommandFlags flags = CommandFlags.None)
                {
                    return null;
                }

                public Task<long> SortAndStoreAsync(RedisKey destination, RedisKey key, long skip = 0, long take = -1, Order order = Order.Ascending,
                    SortType sortType = SortType.Numeric, RedisValue @by = new RedisValue(), RedisValue[] get = null, CommandFlags flags = CommandFlags.None)
                {
                    return null;
                }

                public Task<RedisValue[]> SortAsync(RedisKey key, long skip = 0, long take = -1, Order order = Order.Ascending, SortType sortType = SortType.Numeric,
                    RedisValue @by = new RedisValue(), RedisValue[] get = null, CommandFlags flags = CommandFlags.None)
                {
                    return null;
                }

                public Task<bool> SortedSetAddAsync(RedisKey key, RedisValue member, double score, CommandFlags flags = CommandFlags.None)
                {
                    return null;
                }

                public Task<long> SortedSetAddAsync(RedisKey key, SortedSetEntry[] values, CommandFlags flags = CommandFlags.None)
                {
                    return null;
                }

                public Task<long> SortedSetCombineAndStoreAsync(SetOperation operation, RedisKey destination, RedisKey first, RedisKey second,
                    Aggregate aggregate = Aggregate.Sum, CommandFlags flags = CommandFlags.None)
                {
                    return null;
                }

                public Task<long> SortedSetCombineAndStoreAsync(SetOperation operation, RedisKey destination, RedisKey[] keys,
                    double[] weights = null, Aggregate aggregate = Aggregate.Sum, CommandFlags flags = CommandFlags.None)
                {
                    return null;
                }

                public Task<double> SortedSetDecrementAsync(RedisKey key, RedisValue member, double value, CommandFlags flags = CommandFlags.None)
                {
                    return null;
                }

                public Task<double> SortedSetIncrementAsync(RedisKey key, RedisValue member, double value, CommandFlags flags = CommandFlags.None)
                {
                    return null;
                }

                public Task<long> SortedSetLengthAsync(RedisKey key, double min = double.NegativeInfinity, double max = double.PositiveInfinity, Exclude exclude = Exclude.None,
                    CommandFlags flags = CommandFlags.None)
                {
                    return null;
                }

                public Task<long> SortedSetLengthByValueAsync(RedisKey key, RedisValue min, RedisValue max, Exclude exclude = Exclude.None,
                    CommandFlags flags = CommandFlags.None)
                {
                    return null;
                }

                public Task<RedisValue[]> SortedSetRangeByRankAsync(RedisKey key, long start = 0, long stop = -1, Order order = Order.Ascending,
                    CommandFlags flags = CommandFlags.None)
                {
                    return null;
                }

                public Task<SortedSetEntry[]> SortedSetRangeByRankWithScoresAsync(RedisKey key, long start = 0, long stop = -1, Order order = Order.Ascending,
                    CommandFlags flags = CommandFlags.None)
                {
                    return null;
                }

                public Task<RedisValue[]> SortedSetRangeByScoreAsync(RedisKey key, double start = double.NegativeInfinity, double stop = double.PositiveInfinity, Exclude exclude = Exclude.None,
                    Order order = Order.Ascending, long skip = 0, long take = -1, CommandFlags flags = CommandFlags.None)
                {
                    return null;
                }

                public Task<SortedSetEntry[]> SortedSetRangeByScoreWithScoresAsync(RedisKey key, double start = -double.NegativeInfinity, double stop = double.PositiveInfinity,
                    Exclude exclude = Exclude.None, Order order = Order.Ascending, long skip = 0, long take = -1, CommandFlags flags = CommandFlags.None)
                {
                    return null;
                }

                public Task<RedisValue[]> SortedSetRangeByValueAsync(RedisKey key, RedisValue min = new RedisValue(), RedisValue max = new RedisValue(),
                    Exclude exclude = Exclude.None, long skip = 0, long take = -1, CommandFlags flags = CommandFlags.None)
                {
                    return null;
                }

                public Task<long?> SortedSetRankAsync(RedisKey key, RedisValue member, Order order = Order.Ascending, CommandFlags flags = CommandFlags.None)
                {
                    return null;
                }

                public Task<bool> SortedSetRemoveAsync(RedisKey key, RedisValue member, CommandFlags flags = CommandFlags.None)
                {
                    return null;
                }

                public Task<long> SortedSetRemoveAsync(RedisKey key, RedisValue[] members, CommandFlags flags = CommandFlags.None)
                {
                    return null;
                }

                public Task<long> SortedSetRemoveRangeByRankAsync(RedisKey key, long start, long stop, CommandFlags flags = CommandFlags.None)
                {
                    return null;
                }

                public Task<long> SortedSetRemoveRangeByScoreAsync(RedisKey key, double start, double stop, Exclude exclude = Exclude.None,
                    CommandFlags flags = CommandFlags.None)
                {
                    return null;
                }

                public Task<long> SortedSetRemoveRangeByValueAsync(RedisKey key, RedisValue min, RedisValue max, Exclude exclude = Exclude.None,
                    CommandFlags flags = CommandFlags.None)
                {
                    return null;
                }

                public Task<double?> SortedSetScoreAsync(RedisKey key, RedisValue member, CommandFlags flags = CommandFlags.None)
                {
                    return null;
                }

                public Task<long> StringAppendAsync(RedisKey key, RedisValue value, CommandFlags flags = CommandFlags.None)
                {
                    return null;
                }

                public Task<long> StringBitCountAsync(RedisKey key, long start = 0, long end = -1, CommandFlags flags = CommandFlags.None)
                {
                    return null;
                }

                public Task<long> StringBitOperationAsync(Bitwise operation, RedisKey destination, RedisKey first, RedisKey second = new RedisKey(),
                    CommandFlags flags = CommandFlags.None)
                {
                    return null;
                }

                public Task<long> StringBitOperationAsync(Bitwise operation, RedisKey destination, RedisKey[] keys, CommandFlags flags = CommandFlags.None)
                {
                    return null;
                }

                public Task<long> StringBitPositionAsync(RedisKey key, bool bit, long start = 0, long end = -1, CommandFlags flags = CommandFlags.None)
                {
                    return null;
                }

                public Task<long> StringDecrementAsync(RedisKey key, long value = 1, CommandFlags flags = CommandFlags.None)
                {
                    return null;
                }

                public Task<double> StringDecrementAsync(RedisKey key, double value, CommandFlags flags = CommandFlags.None)
                {
                    return null;
                }

                public Task<RedisValue> StringGetAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
                {
                    return null;
                }

                public Task<RedisValue[]> StringGetAsync(RedisKey[] keys, CommandFlags flags = CommandFlags.None)
                {
                    return null;
                }

                public Task<bool> StringGetBitAsync(RedisKey key, long offset, CommandFlags flags = CommandFlags.None)
                {
                    return null;
                }

                public Task<RedisValue> StringGetRangeAsync(RedisKey key, long start, long end, CommandFlags flags = CommandFlags.None)
                {
                    return null;
                }

                public Task<RedisValue> StringGetSetAsync(RedisKey key, RedisValue value, CommandFlags flags = CommandFlags.None)
                {
                    return null;
                }

                public Task<RedisValueWithExpiry> StringGetWithExpiryAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
                {
                    return null;
                }

                public Task<long> StringIncrementAsync(RedisKey key, long value = 1, CommandFlags flags = CommandFlags.None)
                {
                    return null;
                }

                public Task<double> StringIncrementAsync(RedisKey key, double value, CommandFlags flags = CommandFlags.None)
                {
                    return null;
                }

                public Task<long> StringLengthAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
                {
                    return null;
                }

                public Task<bool> StringSetAsync(RedisKey key, RedisValue value, TimeSpan? expiry = null, When when = When.Always,
                    CommandFlags flags = CommandFlags.None)
                {
                    return null;
                }

                public Task<bool> StringSetAsync(KeyValuePair<RedisKey, RedisValue>[] values, When when = When.Always, CommandFlags flags = CommandFlags.None)
                {
                    return null;
                }

                public Task<bool> StringSetBitAsync(RedisKey key, long offset, bool bit, CommandFlags flags = CommandFlags.None)
                {
                    return null;
                }

                public Task<RedisValue> StringSetRangeAsync(RedisKey key, long offset, RedisValue value, CommandFlags flags = CommandFlags.None)
                {
                    return null;
                }

                public IBatch CreateBatch(object asyncState = null)
                {
                    return null;
                }

                public void KeyMigrate(RedisKey key, EndPoint toServer, int toDatabase = 0, int timeoutMilliseconds = 0,
                    MigrateOptions migrateOptions = MigrateOptions.None, CommandFlags flags = CommandFlags.None)
                {
                }

                public ITransaction CreateTransaction(object asyncState = null)
                {
                    return null;
                }

                public RedisValue DebugObject(RedisKey key, CommandFlags flags = CommandFlags.None)
                {
                    return new RedisValue();
                }

                public long HashDecrement(RedisKey key, RedisValue hashField, long value = 1, CommandFlags flags = CommandFlags.None)
                {
                    return 0;
                }

                public double HashDecrement(RedisKey key, RedisValue hashField, double value, CommandFlags flags = CommandFlags.None)
                {
                    return 0;
                }

                public bool HashDelete(RedisKey key, RedisValue hashField, CommandFlags flags = CommandFlags.None)
                {
                    return false;
                }

                public long HashDelete(RedisKey key, RedisValue[] hashFields, CommandFlags flags = CommandFlags.None)
                {
                    return 0;
                }

                public bool HashExists(RedisKey key, RedisValue hashField, CommandFlags flags = CommandFlags.None)
                {
                    return false;
                }

                public RedisValue HashGet(RedisKey key, RedisValue hashField, CommandFlags flags = CommandFlags.None)
                {
                    return new RedisValue();
                }

                public RedisValue[] HashGet(RedisKey key, RedisValue[] hashFields, CommandFlags flags = CommandFlags.None)
                {
                    return new RedisValue[] { };
                }

                public HashEntry[] HashGetAll(RedisKey key, CommandFlags flags = CommandFlags.None)
                {
                    return _hashEntriies[key.ToString()];
                }

                public long HashIncrement(RedisKey key, RedisValue hashField, long value = 1, CommandFlags flags = CommandFlags.None)
                {
                    return 0;
                }

                public double HashIncrement(RedisKey key, RedisValue hashField, double value, CommandFlags flags = CommandFlags.None)
                {
                    return 0;
                }

                public RedisValue[] HashKeys(RedisKey key, CommandFlags flags = CommandFlags.None)
                {
                    return new RedisValue[] { };
                }

                public long HashLength(RedisKey key, CommandFlags flags = CommandFlags.None)
                {
                    return 0;
                }

                public IEnumerable<HashEntry> HashScan(RedisKey key, RedisValue pattern, int pageSize, CommandFlags flags)
                {
                    yield break;
                }

                public IEnumerable<HashEntry> HashScan(RedisKey key, RedisValue pattern = new RedisValue(), int pageSize = 10, long cursor = 0,
                    int pageOffset = 0, CommandFlags flags = CommandFlags.None)
                {
                    yield break;
                }

                private Dictionary<RedisKey, HashEntry[]> _hashEntriies = new Dictionary<RedisKey, HashEntry[]>();

                public void HashSet(RedisKey key, HashEntry[] hashFields, CommandFlags flags = CommandFlags.None)
                {
                    _hashEntriies.Add(key.ToString(), hashFields);
                }

                public bool HashSet(RedisKey key, RedisValue hashField, RedisValue value, When when = When.Always, CommandFlags flags = CommandFlags.None)
                {
                    return false;
                }

                public RedisValue[] HashValues(RedisKey key, CommandFlags flags = CommandFlags.None)
                {
                    return new RedisValue[] { };
                }

                public bool HyperLogLogAdd(RedisKey key, RedisValue value, CommandFlags flags = CommandFlags.None)
                {
                    return false;
                }

                public bool HyperLogLogAdd(RedisKey key, RedisValue[] values, CommandFlags flags = CommandFlags.None)
                {
                    return false;
                }

                public long HyperLogLogLength(RedisKey key, CommandFlags flags = CommandFlags.None)
                {
                    return 0;
                }

                public long HyperLogLogLength(RedisKey[] keys, CommandFlags flags = CommandFlags.None)
                {
                    return 0;
                }

                public void HyperLogLogMerge(RedisKey destination, RedisKey first, RedisKey second, CommandFlags flags = CommandFlags.None)
                {
                }

                public void HyperLogLogMerge(RedisKey destination, RedisKey[] sourceKeys, CommandFlags flags = CommandFlags.None)
                {
                }

                public EndPoint IdentifyEndpoint(RedisKey key = new RedisKey(), CommandFlags flags = CommandFlags.None)
                {
                    return null;
                }

                public bool KeyDelete(RedisKey key, CommandFlags flags = CommandFlags.None)
                {
                    return false;
                }

                public long KeyDelete(RedisKey[] keys, CommandFlags flags = CommandFlags.None)
                {
                    return 0;
                }

                public byte[] KeyDump(RedisKey key, CommandFlags flags = CommandFlags.None)
                {
                    return new byte[] { };
                }

                public bool KeyExists(RedisKey key, CommandFlags flags = CommandFlags.None)
                {
                    return false;
                }

                public bool KeyExpire(RedisKey key, TimeSpan? expiry, CommandFlags flags = CommandFlags.None)
                {
                    return false;
                }

                public bool KeyExpire(RedisKey key, DateTime? expiry, CommandFlags flags = CommandFlags.None)
                {
                    return false;
                }

                public bool KeyMove(RedisKey key, int database, CommandFlags flags = CommandFlags.None)
                {
                    return false;
                }

                public bool KeyPersist(RedisKey key, CommandFlags flags = CommandFlags.None)
                {
                    return false;
                }

                public RedisKey KeyRandom(CommandFlags flags = CommandFlags.None)
                {
                    return new RedisKey();
                }

                public bool KeyRename(RedisKey key, RedisKey newKey, When when = When.Always, CommandFlags flags = CommandFlags.None)
                {
                    return false;
                }

                public void KeyRestore(RedisKey key, byte[] value, TimeSpan? expiry = null, CommandFlags flags = CommandFlags.None)
                {
                }

                public TimeSpan? KeyTimeToLive(RedisKey key, CommandFlags flags = CommandFlags.None)
                {
                    return null;
                }

                public RedisType KeyType(RedisKey key, CommandFlags flags = CommandFlags.None)
                {
                    return RedisType.None;
                }

                public RedisValue ListGetByIndex(RedisKey key, long index, CommandFlags flags = CommandFlags.None)
                {
                    return new RedisValue();
                }

                public long ListInsertAfter(RedisKey key, RedisValue pivot, RedisValue value, CommandFlags flags = CommandFlags.None)
                {
                    return 0;
                }

                public long ListInsertBefore(RedisKey key, RedisValue pivot, RedisValue value, CommandFlags flags = CommandFlags.None)
                {
                    return 0;
                }

                public RedisValue ListLeftPop(RedisKey key, CommandFlags flags = CommandFlags.None)
                {
                    return new RedisValue();
                }

                public long ListLeftPush(RedisKey key, RedisValue value, When when = When.Always, CommandFlags flags = CommandFlags.None)
                {
                    return 0;
                }

                public long ListLeftPush(RedisKey key, RedisValue[] values, CommandFlags flags = CommandFlags.None)
                {
                    return 0;
                }

                public long ListLength(RedisKey key, CommandFlags flags = CommandFlags.None)
                {
                    return 0;
                }

                public RedisValue[] ListRange(RedisKey key, long start = 0, long stop = -1, CommandFlags flags = CommandFlags.None)
                {
                    return new RedisValue[] { };
                }

                public long ListRemove(RedisKey key, RedisValue value, long count = 0, CommandFlags flags = CommandFlags.None)
                {
                    return 0;
                }

                public RedisValue ListRightPop(RedisKey key, CommandFlags flags = CommandFlags.None)
                {
                    return new RedisValue();
                }

                public RedisValue ListRightPopLeftPush(RedisKey source, RedisKey destination, CommandFlags flags = CommandFlags.None)
                {
                    return new RedisValue();
                }

                public long ListRightPush(RedisKey key, RedisValue value, When when = When.Always, CommandFlags flags = CommandFlags.None)
                {
                    return 0;
                }

                public long ListRightPush(RedisKey key, RedisValue[] values, CommandFlags flags = CommandFlags.None)
                {
                    return 0;
                }

                public void ListSetByIndex(RedisKey key, long index, RedisValue value, CommandFlags flags = CommandFlags.None)
                {
                }

                public void ListTrim(RedisKey key, long start, long stop, CommandFlags flags = CommandFlags.None)
                {
                }

                public bool LockExtend(RedisKey key, RedisValue value, TimeSpan expiry, CommandFlags flags = CommandFlags.None)
                {
                    return false;
                }

                public RedisValue LockQuery(RedisKey key, CommandFlags flags = CommandFlags.None)
                {
                    return new RedisValue();
                }

                public bool LockRelease(RedisKey key, RedisValue value, CommandFlags flags = CommandFlags.None)
                {
                    return false;
                }

                public bool LockTake(RedisKey key, RedisValue value, TimeSpan expiry, CommandFlags flags = CommandFlags.None)
                {
                    return false;
                }

                public long Publish(RedisChannel channel, RedisValue message, CommandFlags flags = CommandFlags.None)
                {
                    return 0;
                }

                public RedisResult ScriptEvaluate(string script, RedisKey[] keys = null, RedisValue[] values = null, CommandFlags flags = CommandFlags.None)
                {
                    return null;
                }

                public RedisResult ScriptEvaluate(byte[] hash, RedisKey[] keys = null, RedisValue[] values = null, CommandFlags flags = CommandFlags.None)
                {
                    return null;
                }

                public RedisResult ScriptEvaluate(LuaScript script, object parameters = null, CommandFlags flags = CommandFlags.None)
                {
                    throw new NotImplementedException();
                }

                public RedisResult ScriptEvaluate(LoadedLuaScript script, object parameters = null, CommandFlags flags = CommandFlags.None)
                {
                    throw new NotImplementedException();
                }

                public bool SetAdd(RedisKey key, RedisValue value, CommandFlags flags = CommandFlags.None)
                {
                    Debug.WriteLine(key.ToString() + " : " + value);
                    _set.Add(key, new[]{value});
                    return false;
                }

                public long SetAdd(RedisKey key, RedisValue[] values, CommandFlags flags = CommandFlags.None)
                {
                    return 0;
                }

                public RedisValue[] SetCombine(SetOperation operation, RedisKey first, RedisKey second, CommandFlags flags = CommandFlags.None)
                {
                    return new RedisValue[] { };
                }

                public RedisValue[] SetCombine(SetOperation operation, RedisKey[] keys, CommandFlags flags = CommandFlags.None)
                {
                    return new RedisValue[] { };
                }

                public long SetCombineAndStore(SetOperation operation, RedisKey destination, RedisKey first, RedisKey second,
                    CommandFlags flags = CommandFlags.None)
                {
                    return 0;
                }

                public long SetCombineAndStore(SetOperation operation, RedisKey destination, RedisKey[] keys, CommandFlags flags = CommandFlags.None)
                {
                    return 0;
                }

                Dictionary<RedisKey,RedisValue[]> _set = new Dictionary<RedisKey, RedisValue[]>();

                public bool SetContains(RedisKey key, RedisValue value, CommandFlags flags = CommandFlags.None)
                {
                    Debug.WriteLine(key);
                    var v = _set[key.ToString()];
                    return v.Any(p=>p == value);
                }

                public long SetLength(RedisKey key, CommandFlags flags = CommandFlags.None)
                {
                    return 0;
                }

                public RedisValue[] SetMembers(RedisKey key, CommandFlags flags = CommandFlags.None)
                {
                    return _set[key];
                }

                public bool SetMove(RedisKey source, RedisKey destination, RedisValue value, CommandFlags flags = CommandFlags.None)
                {
                    return false;
                }

                public RedisValue SetPop(RedisKey key, CommandFlags flags = CommandFlags.None)
                {
                    return new RedisValue();
                }

                public RedisValue SetRandomMember(RedisKey key, CommandFlags flags = CommandFlags.None)
                {
                    return new RedisValue();
                }

                public RedisValue[] SetRandomMembers(RedisKey key, long count, CommandFlags flags = CommandFlags.None)
                {
                    return new RedisValue[] { };
                }

                public bool SetRemove(RedisKey key, RedisValue value, CommandFlags flags = CommandFlags.None)
                {
                    return false;
                }

                public long SetRemove(RedisKey key, RedisValue[] values, CommandFlags flags = CommandFlags.None)
                {
                    return 0;
                }

                public IEnumerable<RedisValue> SetScan(RedisKey key, RedisValue pattern, int pageSize, CommandFlags flags)
                {
                    yield break;
                }

                public IEnumerable<RedisValue> SetScan(RedisKey key, RedisValue pattern = new RedisValue(), int pageSize = 10, long cursor = 0,
                    int pageOffset = 0, CommandFlags flags = CommandFlags.None)
                {
                    yield break;
                }

                public RedisValue[] Sort(RedisKey key, long skip = 0, long take = -1, Order order = Order.Ascending, SortType sortType = SortType.Numeric,
                    RedisValue @by = new RedisValue(), RedisValue[] get = null, CommandFlags flags = CommandFlags.None)
                {
                    return new RedisValue[] { };
                }

                public long SortAndStore(RedisKey destination, RedisKey key, long skip = 0, long take = -1, Order order = Order.Ascending,
                    SortType sortType = SortType.Numeric, RedisValue @by = new RedisValue(), RedisValue[] get = null, CommandFlags flags = CommandFlags.None)
                {
                    return 0;
                }

                public bool SortedSetAdd(RedisKey key, RedisValue member, double score, CommandFlags flags = CommandFlags.None)
                {
                    return false;
                }

                public long SortedSetAdd(RedisKey key, SortedSetEntry[] values, CommandFlags flags = CommandFlags.None)
                {
                    return 0;
                }

                public long SortedSetCombineAndStore(SetOperation operation, RedisKey destination, RedisKey first, RedisKey second,
                    Aggregate aggregate = Aggregate.Sum, CommandFlags flags = CommandFlags.None)
                {
                    return 0;
                }

                public long SortedSetCombineAndStore(SetOperation operation, RedisKey destination, RedisKey[] keys, double[] weights = null,
                    Aggregate aggregate = Aggregate.Sum, CommandFlags flags = CommandFlags.None)
                {
                    return 0;
                }

                public double SortedSetDecrement(RedisKey key, RedisValue member, double value, CommandFlags flags = CommandFlags.None)
                {
                    return 0;
                }

                public double SortedSetIncrement(RedisKey key, RedisValue member, double value, CommandFlags flags = CommandFlags.None)
                {
                    return 0;
                }

                public long SortedSetLength(RedisKey key, double min = double.NegativeInfinity, double max = double.PositiveInfinity, Exclude exclude = Exclude.None,
                    CommandFlags flags = CommandFlags.None)
                {
                    return 0;
                }

                public long SortedSetLengthByValue(RedisKey key, RedisValue min, RedisValue max, Exclude exclude = Exclude.None,
                    CommandFlags flags = CommandFlags.None)
                {
                    return 0;
                }

                public RedisValue[] SortedSetRangeByRank(RedisKey key, long start = 0, long stop = -1, Order order = Order.Ascending,
                    CommandFlags flags = CommandFlags.None)
                {
                    return new RedisValue[] { };
                }

                public SortedSetEntry[] SortedSetRangeByRankWithScores(RedisKey key, long start = 0, long stop = -1, Order order = Order.Ascending,
                    CommandFlags flags = CommandFlags.None)
                {
                    return new SortedSetEntry[] { };
                }

                public RedisValue[] SortedSetRangeByScore(RedisKey key, double start = double.NegativeInfinity, double stop = double.PositiveInfinity,
                    Exclude exclude = Exclude.None, Order order = Order.Ascending, long skip = 0, long take = -1, CommandFlags flags = CommandFlags.None)
                {
                    return new RedisValue[] { };
                }

                public SortedSetEntry[] SortedSetRangeByScoreWithScores(RedisKey key, double start = double.NegativeInfinity, double stop = double.PositiveInfinity,
                    Exclude exclude = Exclude.None, Order order = Order.Ascending, long skip = 0, long take = -1, CommandFlags flags = CommandFlags.None)
                {
                    return new SortedSetEntry[] { };
                }

                public RedisValue[] SortedSetRangeByValue(RedisKey key, RedisValue min = new RedisValue(), RedisValue max = new RedisValue(),
                    Exclude exclude = Exclude.None, long skip = 0, long take = -1, CommandFlags flags = CommandFlags.None)
                {
                    return new RedisValue[] { };
                }

                public long? SortedSetRank(RedisKey key, RedisValue member, Order order = Order.Ascending, CommandFlags flags = CommandFlags.None)
                {
                    return null;
                }

                public bool SortedSetRemove(RedisKey key, RedisValue member, CommandFlags flags = CommandFlags.None)
                {
                    return false;
                }

                public long SortedSetRemove(RedisKey key, RedisValue[] members, CommandFlags flags = CommandFlags.None)
                {
                    return 0;
                }

                public long SortedSetRemoveRangeByRank(RedisKey key, long start, long stop, CommandFlags flags = CommandFlags.None)
                {
                    return 0;
                }

                public long SortedSetRemoveRangeByScore(RedisKey key, double start, double stop, Exclude exclude = Exclude.None,
                    CommandFlags flags = CommandFlags.None)
                {
                    return 0;
                }

                public long SortedSetRemoveRangeByValue(RedisKey key, RedisValue min, RedisValue max, Exclude exclude = Exclude.None,
                    CommandFlags flags = CommandFlags.None)
                {
                    return 0;
                }

                public IEnumerable<SortedSetEntry> SortedSetScan(RedisKey key, RedisValue pattern, int pageSize, CommandFlags flags)
                {
                    yield break;
                }

                public IEnumerable<SortedSetEntry> SortedSetScan(RedisKey key, RedisValue pattern = new RedisValue(), int pageSize = 10, long cursor = 0,
                    int pageOffset = 0, CommandFlags flags = CommandFlags.None)
                {
                    yield break;
                }

                public double? SortedSetScore(RedisKey key, RedisValue member, CommandFlags flags = CommandFlags.None)
                {
                    return null;
                }

                public long StringAppend(RedisKey key, RedisValue value, CommandFlags flags = CommandFlags.None)
                {
                    return 0;
                }

                public long StringBitCount(RedisKey key, long start = 0, long end = -1, CommandFlags flags = CommandFlags.None)
                {
                    return 0;
                }

                public long StringBitOperation(Bitwise operation, RedisKey destination, RedisKey first, RedisKey second = new RedisKey(),
                    CommandFlags flags = CommandFlags.None)
                {
                    return 0;
                }

                public long StringBitOperation(Bitwise operation, RedisKey destination, RedisKey[] keys, CommandFlags flags = CommandFlags.None)
                {
                    return 0;
                }

                public long StringBitPosition(RedisKey key, bool bit, long start = 0, long end = -1, CommandFlags flags = CommandFlags.None)
                {
                    return 0;
                }

                public long StringDecrement(RedisKey key, long value = 1, CommandFlags flags = CommandFlags.None)
                {
                    return 0;
                }

                public double StringDecrement(RedisKey key, double value, CommandFlags flags = CommandFlags.None)
                {
                    return 0;
                }

                public RedisValue StringGet(RedisKey key, CommandFlags flags = CommandFlags.None)
                {
                    return new RedisValue();
                }

                public RedisValue[] StringGet(RedisKey[] keys, CommandFlags flags = CommandFlags.None)
                {
                    return new RedisValue[] { };
                }

                public bool StringGetBit(RedisKey key, long offset, CommandFlags flags = CommandFlags.None)
                {
                    return false;
                }

                public RedisValue StringGetRange(RedisKey key, long start, long end, CommandFlags flags = CommandFlags.None)
                {
                    return new RedisValue();
                }

                public RedisValue StringGetSet(RedisKey key, RedisValue value, CommandFlags flags = CommandFlags.None)
                {
                    return new RedisValue();
                }

                public RedisValueWithExpiry StringGetWithExpiry(RedisKey key, CommandFlags flags = CommandFlags.None)
                {
                    return new RedisValueWithExpiry();
                }

                public long StringIncrement(RedisKey key, long value = 1, CommandFlags flags = CommandFlags.None)
                {
                    return 0;
                }

                public double StringIncrement(RedisKey key, double value, CommandFlags flags = CommandFlags.None)
                {
                    return 0;
                }

                public long StringLength(RedisKey key, CommandFlags flags = CommandFlags.None)
                {
                    return 0;
                }

                public bool StringSet(RedisKey key, RedisValue value, TimeSpan? expiry = null, When when = When.Always, CommandFlags flags = CommandFlags.None)
                {
                    return false;
                }

                public bool StringSet(KeyValuePair<RedisKey, RedisValue>[] values, When when = When.Always, CommandFlags flags = CommandFlags.None)
                {
                    return false;
                }

                public bool StringSetBit(RedisKey key, long offset, bool bit, CommandFlags flags = CommandFlags.None)
                {
                    return false;
                }

                public RedisValue StringSetRange(RedisKey key, long offset, RedisValue value, CommandFlags flags = CommandFlags.None)
                {
                    return new RedisValue();
                }

                public int Database
                {
                    get { return 0; }
                }
            }
        }
    }
}