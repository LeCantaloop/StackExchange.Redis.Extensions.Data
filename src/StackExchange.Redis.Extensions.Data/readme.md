Usage

 public static class Redis
    {
        private static ICacheClient _clientManager;

        public static ICacheClient ClientManager
        {
            get
            {
                if (_clientManager == null)
                {
                    var s = new JsonSerializer();
                    try
                    {
                        var configurationOptions = new ConfigurationOptions
                        {
                            EndPoints =
                            {
                                { "server", 6380 }
                            },
                            KeepAlive = 180,
                            Password = "",
                            DefaultVersion = new Version("2.8"),
                            // Needed for cache clear
                            AllowAdmin = true,
                            AbortOnConnectFail = false,
                            ConnectTimeout = 10000,
                            Ssl = true
                        };

                        var cmp =
                            ConnectionMultiplexer.Connect(configurationOptions
                                );

                        _clientManager = new StackExchangeRedisCacheClient(cmp, s);
                    }
                    catch (Exception)
                    {
                        //TODO: Log Failure

                    }
                }
                return _clientManager;
            }
        }
    }