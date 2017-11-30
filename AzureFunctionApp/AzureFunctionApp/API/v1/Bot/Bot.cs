using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Configuration;
using Newtonsoft.Json;
using Microsoft.Bot.Builder.Azure;
using System.Threading;
using System.Collections.Generic;
using AzureFunctionApp.Models.v1;
using AdaptiveCards;
using System.Collections;
using System.Text;

namespace AzureFunctionApp.API.v1.Bot
{
    public static class Bot
    {
        [FunctionName("Bot")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "v1/bot")]HttpRequestMessage req, TraceWriter log)
        {
            var botId = ConfigurationManager.AppSettings["BotId"];
            var botSecret = ConfigurationManager.AppSettings["BotSecret"];
            Activity reply = null;
            log.Info(await req.Content.ReadAsStringAsync());
            var msg = await req.Content.ReadAsAsync<Activity>();
            var cred = new MicrosoftAppCredentials(botId, botSecret);

            log.Info(JsonConvert.SerializeObject(cred));
            if (!await BotService.Authenticator.TryAuthenticateAsync(req, new[] { msg }, CancellationToken.None))
            {
                return BotAuthenticator.GenerateUnauthorizedResponse(req);
            }
            log.Info("1");
            MicrosoftAppCredentials.TrustServiceUrl(msg.ServiceUrl, DateTime.MaxValue);
            log.Info("2");
            var connector = new ConnectorClient(new Uri(msg.ServiceUrl), cred);
            log.Info("3");
            if (msg.Type == "message")
            {
                log.Info("4");
                string text = null;
                if (msg.Text.ToLower().Contains("introduce"))
                {

                    text = "I am an Azure Function created as a demo for this Experts Live event. Pleased to meet you all! What trick do you want me to perform first?";
                    reply = msg.CreateReply(text);
                    reply.Speak = text;
                }
                else if(msg.Text.ToLower().Contains("show") && msg.Text.ToLower().Contains("people"))
                {
                    using(var client = new HttpClient())
                    {
                        var baseUrl = $"https://{req.RequestUri.Host}/api/v1/eventbrite";
                        log.Info(baseUrl);
                        var uri = new Uri(baseUrl);
                        log.Info(uri.ToString());
                        var resp = await client.GetStringAsync(uri.AbsoluteUri);
                        log.Info(resp);
                        List<EventBriteAttendee> attendees = JsonConvert.DeserializeObject<List<EventBriteAttendee>>(resp);
                        log.Info(attendees.Count.ToString());
                        var speak = $"There are {attendees.Count} attendees for this event. ";
                        reply = msg.CreateReply($"There are {attendees.Count} attendees for this event. ");
                        var card = new ReceiptCard();
                        var i = 0;
                        foreach (var attendee in attendees)
                        {
                            card.Items.Add(new ReceiptItem(attendee.FullName));
                            if(i == attendees.Count - 1)
                            {
                                speak = speak.Substring(0, speak.Length - 2);
                                speak += $" and {attendee.FirstName}.";
                            } else
                            {
                                speak += $"{attendee.FirstName}, ";
                            }
                            i++;
                        }
                        reply.Attachments.Add(card.ToAttachment());
                        reply.Speak = speak;
                        

                    }
                }
                else if (msg.Text.ToLower().Contains("mail"))
                {
                    using (var client = new HttpClient())
                    {
                        var baseUrl = $"https://{req.RequestUri.Host}/api/v1/eventbrite";
                        log.Info(baseUrl);
                        var uri = new Uri(baseUrl);
                        log.Info(uri.ToString());
                        var resp = await client.GetStringAsync(uri.AbsoluteUri);
                        log.Info(resp);
                        List<EventBriteAttendee> attendees = JsonConvert.DeserializeObject<List<EventBriteAttendee>>(resp);
                        log.Info(attendees.Count.ToString());
                        var topicEndPoint = ConfigurationManager.AppSettings["SendMailTopicEndpoint"];
                        var topicKey = ConfigurationManager.AppSettings["SendMailTopicKey"];
                        client.DefaultRequestHeaders.Add("aeg-sas-key", topicKey);
                        client.DefaultRequestHeaders.UserAgent.ParseAdd("serverlessdemo");
                        IList<GridEvent<SendMail>> mailItems = new List<GridEvent<SendMail>>();
                        foreach(var attendee in attendees)
                        {
                            mailItems.Add(new GridEvent<SendMail>()
                            {
                                EventType = "SendMail",
                                Subject = "serverless",
                                Data = new SendMail()
                                {
                                    Body = "serverless",
                                    Subject = $"Experts Live Cafe: Serverless Presentation",
                                    To = attendee.EMail
                                }
                            });

                        }

                        string json = JsonConvert.SerializeObject(mailItems);
                        log.Info(json);
                        HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, topicEndPoint)
                        {
                            Content = new StringContent(json,Encoding.UTF8,"application/json")
                        };
                        var response = await client.SendAsync(request);
                        log.Info(JsonConvert.SerializeObject(response));
                        text = "Ok, I've sent a mail to every attendee! Enjoy!";
                        reply = msg.CreateReply(text);
                        reply.Speak = text;

                        


                    }
                }
                else
                {
                    text = "Sorry, I was not programmed to be smart enough to handle your gibberish";
                    reply = msg.CreateReply(text);
                    reply.Speak = text;

                }
                //reply = msg.CreateReply(text);
                //reply.Speak = text;
                







            }
            else
            {
                reply = HandleSystemMessage(msg);
              
            }
            if (reply != null)
            {
                connector.Conversations.ReplyToActivity(reply);
            }
            return req.CreateResponse(HttpStatusCode.OK);
        }

        private static Activity HandleSystemMessage(Activity message)
        {
            if (message.Type == "BotAddedToConversation")
            {
                return message.CreateReply($"Welcome {message.From?.Name}!");
            }
            else if (message.Type == "BotRemovedFromConversation")
            {
                return message.CreateReply($"Bye {message.From?.Name}!");
            }

            return null;
        }

    }
}
