using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using System.Configuration;
using AzureFunctionApp.Models.v1;
using System.Collections.Generic;

namespace AzureFunctionApp.API.v1.EventBrite
{
    public static class EventBrite
    {
        [FunctionName("EventBrite")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "v1/eventbrite")]HttpRequestMessage req, TraceWriter log)
        {
            var token = ConfigurationManager.AppSettings["EventBritetoken"];
            var client = new EventBriteClient(token);
            var resp  = req.CreateResponse<List<EventBriteAttendee>>(await client.getAttendees());
            return resp;
        }
    }
}
