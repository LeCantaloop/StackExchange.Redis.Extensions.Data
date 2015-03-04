# StackExchange.Redis.Extensions.Data

Extending [StackExchange.Redis.Extensions](https://github.com/imperugo/StackExchange.Redis.Extensions) allowing implementation of object relationships, storage of object properties as hashes, and implementing a ```Repository``` pattern.

##How to install it
StackExchange.Redis.Extensions.Data is an extension of StackExchange.Redis.Extensions, which has a Core and the Serializer implementation.

##Install Core [![NuGet Status](http://img.shields.io/nuget/v/StackExchange.Redis.Extensions.Core.svg?style=flat)](https://www.nuget.org/packages/StackExchange.Redis.Extensions.Core/)

```
PM> Install-Package StackExchange.Redis.Extensions.Core
```

## Install Json.NET implementation [![NuGet Status](http://img.shields.io/nuget/v/StackExchange.Redis.Extensions.Newtonsoft.svg?style=flat)](https://www.nuget.org/packages/StackExchange.Redis.Extensions.Newtonsoft/)

```
PM> Install-Package StackExchange.Redis.Extensions.Newtonsoft
```

## How to use it
The repositories use an instance of ICacheClient, which is provided by StackExchange.Redis.Extensions

```csharp
var client = new StackExchangeRedisCacheClient();
var repo = RedisRepository.GetInstance(client);
var user = repo.Get&lt;int, User&gt;(1000);
```

## Object storage into Redis
Object properties are stored as HashEntry items in Redis allowing for update of specific properties without incurring overhead for pulling, deserializing, updating, then serializing the entire object.

```csharp
public class User
{
	[Key]
	public int UserId {get;set;}

	[Index]
	public string Email {get;set;}

	public string FirstName {get;set;}
}



```

The class is decorated with optional attributes to designate a specific field should be used as the identity and that another field can be used for lookup (like an index in RDBMS). In the absence of the ```[Key]``` attribute the system will attempt to locate a suitable property matching either "Id" or "Class" + "Id".

All of these classes are equivilent to the system

```csharp
public class User
{
	[Key]
	public int UserId {get; set;}
}

public class User
{
	public int UserId{get;set;}
}

public class User
{
	public int Id{get;set;}
}
```

When the system encounters a ```Key``` attribute or a property that makes for a suitable identity, a string used for all future key storage is created. The above classes would generate ```userid:{value}``` where "{value}" is the value of the property.

```csharp
var u = new User { UserId = 1000, FirstName = "Richard"};
```
The above would have keys generated as ```userid:1000```

The ```[Index]``` attribute is useful in providing alternative ways to access an identifier or a specific property. For example, if you had a site where a user could login to the system with their email address, the [Index] could provide an easy way to look up the user identifier for an email address, or lookup an email address for a given user identifier. A generated key for this scenario would look like ```userid:1000:email``` where the value is the email address, and ```email:{value}:userid``` where "{value}" is the actual email address and the value of the key is the userid value.

