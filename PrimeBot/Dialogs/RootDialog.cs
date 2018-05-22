using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System.Net.Http;
using System.Text;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace PrimeBot.Dialogs
{
    [Serializable]
    public class RootDialog : IDialog<object>
    {
        // NOTE: Replace this with a valid host name.
        static string host = "https://aiigniteqnaservice.azurewebsites.net";

        // NOTE: Replace this with a valid endpoint key.
        // This is not your subscription key.
        // To get your endpoint keys, call the GET /endpointkeys method.
        static string endpoint_key = "bf770a83-940b-4372-85a8-557609a15bcf";

        // NOTE: Replace this with a valid knowledge base ID.
        // Make sure you have published the knowledge base with the
        // POST /knowledgebases/{knowledge base ID} method.
        static string kb = "f0b6b572-5eb3-4f65-be9b-169c67be43fa";

        static string service = "/qnamaker";
        static string method = "/knowledgebases/" + kb + "/generateAnswer/";
        
        public Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);

            return Task.CompletedTask;
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            var activity = await result as Activity;


            var question = $"{{\"question\": \"{activity.Text}\"}}";

            var res = await GetAnswers(question);
            await context.PostAsync(res);

            // calculate something for us to return
            //int length = (activity.Text ?? string.Empty).Length;

            // return our reply to the user
           // await context.PostAsync($"You sent {activity.Text} which was {length} characters");

            context.Wait(MessageReceivedAsync);
        }

        async static Task<string> Post(string uri, string body)
        {
            using (var client = new HttpClient())
            using (var request = new HttpRequestMessage())
            {
                request.Method = HttpMethod.Post;
                request.RequestUri = new Uri(uri);
                request.Content = new StringContent(body, Encoding.UTF8, "application/json");
                request.Headers.Add("Authorization", "EndpointKey " + endpoint_key);

                var response = await client.SendAsync(request);
                return await response.Content.ReadAsStringAsync();
            }
        }

        async static Task<string> GetAnswers(string question)
        {
            var uri = host + service + method;
            //Console.WriteLine("Calling " + uri + ".");
            var response = await Post(uri, question);
            //Console.WriteLine(response);
            //Console.WriteLine("Press any key to continue.");

            //var myDetails = JsonConvert.DeserializeObject<MyDetail>(jsonData);

            var result = JsonConvert.DeserializeObject<RootObject>(response);



            return result.answers[0].answer;
        }

    }

    public class RootObject
    {
        public List<Answer> answers { get; set; }
    }

    public class Answer
    {
        public List<string> questions { get; set; }
        public string answer { get; set; }
        public double score { get; set; }
        public int id { get; set; }
        public string source { get; set; }
        public List<object> metadata { get; set; }
    }

}