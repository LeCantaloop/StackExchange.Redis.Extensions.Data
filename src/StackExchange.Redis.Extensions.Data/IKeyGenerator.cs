using System.Collections.Generic;

namespace StackExchange.Redis.Extensions.Data
{
    public interface IKeyGenerator
    {
        Key Generate<T>(T value);
    }

    public interface IMultipleKeyGenerator
    {
        IEnumerable<Key> Generate<T>(T value);
    }
}