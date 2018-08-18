using MyApp.Data;
using Newtonsoft.Json;
using Shared;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace MyApp.Services
{
    public class SyncService
    {
        DatabaseRepository DatabaseRepository { get; }

        public DateTime LastSync
        {
            get
            {
                if (App.Current.Properties.ContainsKey("LastSync"))
                    return (DateTime)App.Current.Properties["LastSync"];

                return DateTime.MinValue;
            }
            set
            {
                App.Current.Properties["LastSync"] = value;
            }
        }

        public SyncService(DatabaseRepository databaseRepository)
        {
            DatabaseRepository = databaseRepository;
        }

        public async Task Sync()
        {
            var lastSync = LastSync;

            var changed = await DatabaseRepository.FindAsync(x => x.LastUpdated >= lastSync || (x.Deleted != null && x.Deleted >= lastSync));

            foreach (var item in await PutToServerAsync(changed))
            {
                if (!await DatabaseRepository.Exists(item.Id)) // Does not exist, hence insert
                    await DatabaseRepository.InsertAsync(item);
                else if (item.Deleted.HasValue)
                    await DatabaseRepository.DeleteAsync(item);
                else
                    await DatabaseRepository.UpdateAsync(item);
            }
            LastSync = DateTime.UtcNow;
        }

        public async Task<List<Customer>> PutToServerAsync(List<Customer> customers)
        {
            string url = "http://localhost:50643/api/customers/sync?since=" + System.Net.WebUtility.UrlEncode(LastSync.ToString("z"));

            using (var httpClient = new HttpClient())
            {
                var request = new HttpRequestMessage(HttpMethod.Put, url)
                {
                    Content = new StringContent(JsonConvert.SerializeObject(customers), Encoding.UTF8, "application/json")
                };

                var response = await httpClient.SendAsync(request);

                if(!response.IsSuccessStatusCode)
                {
                    //TODO: handle unsuccess request;
                    throw new NotImplementedException();
                }

                return JsonConvert.DeserializeObject<List<Customer>>(await response.Content.ReadAsStringAsync());
            }
        }
    }
}
