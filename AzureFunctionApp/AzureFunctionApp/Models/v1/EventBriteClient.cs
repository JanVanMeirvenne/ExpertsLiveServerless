using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace AzureFunctionApp.Models.v1
{
    public class EventBriteClient
    {
        string _token = null;
        HttpClient _client = null;
        const string _baseUrl = "https://www.eventbriteapi.com/v3";

        public EventBriteClient(string token)
        {
            this._token = token;
        }

        private async Task<string> getResource(string url)
        {
            if (_client == null)
            {
                _client = new HttpClient();
                //_client.BaseAddress = new Uri(_baseUrl);
            }
            string getUrl = $"{_baseUrl}/{url}?token={_token}";
            var uri = new Uri(getUrl);
            var response = await _client.GetStringAsync(uri.AbsoluteUri);
            return response;


        }

        public async Task<List<EventBriteAttendee>> getAttendees()
        {
            JObject parsed = null;
            try
            {
                var list = new List<EventBriteAttendee>();
                string json = await this.getResource("events/38275794933/attendees/");
                parsed = JObject.Parse(json);
                foreach (var item in (dynamic)parsed["attendees"])
                {
                    var attendee = new EventBriteAttendee()
                    {
                        FirstName = item.profile.first_name,
                        LastName = item.profile.last_name,
                        EMail = item.profile.email,
                        FullName = item.profile.name
                    };
                    list.Add(attendee);
                }
                return list;
            } catch(Exception ex)
            {
                throw new Exception($"{ex.Message}: {JsonConvert.SerializeObject(parsed)}");
            }

            
        }
    }
}
