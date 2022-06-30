using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LibraryBotUtn.Services.BotConfig.Repositories
{

    public class StoreRepository:IStoreRepositori
    {
        private readonly string baseUrl;
        private IDictionary<string, (JObject, string)> _store = new Dictionary<string, (JObject, string)>();
        private SemaphoreSlim _semaphoreSlim = new SemaphoreSlim(1, 1);
        public StoreRepository(string baseUrl)
        {
            this.baseUrl = baseUrl;
        }

        public async Task<(JObject content, string etag)> LoadAsync(string key)
        {
            try
            {
                await _semaphoreSlim.WaitAsync();

                if (_store.TryGetValue(key, out ValueTuple<JObject, string> value))
                {
                    return value;
                }

                return new ValueTuple<JObject, string>(null, null);
            }
            finally
            {
                _semaphoreSlim.Release();
            }
        }

        public async Task<bool> SaveAsync(string key, JObject content, string eTag)
        {
            try
            {
                await _semaphoreSlim.WaitAsync();

                if (eTag != null && _store.TryGetValue(key, out ValueTuple<JObject, string> value))
                {
                    if (eTag != value.Item2)
                    {
                        return false;
                    }
                }

                _store[key] = (content, Guid.NewGuid().ToString());
                return true;
            }
            finally
            {
                _semaphoreSlim.Release();
            }
        }
    }
}
