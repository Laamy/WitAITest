using System.Net.Http;
using System;
using System.Collections.Generic;
using System.Web.Script.Serialization;
using System.Linq;

namespace TEstThignydsg
{
    internal class Program
    {
        static Dictionary<string, List<string>> responses = new Dictionary<string, List<string>>
        {
            { "say_hello",
                new List<string> {
                    "Hello",
                    "Hi",
                    "Hey",
                    "Heyo"
                }
            },

            { "goodbye",
                new List<string> {
                    "cya",
                    "goodbye",
                    "later then",
                    "bye"
                }
            },

            { "thanks",
                new List<string> {
                    "your welcome",
                    "your very welcome"
                }
            }
        };

        static Random ran = new Random();

        static string GetResponseFor(string intentCategory)
        {
            return responses[intentCategory][ran.Next(0, responses[intentCategory].Count)];
        }

        static void Main(string[] args)
        {
            string accessToken = "x";

            while (true)
            {
                Console.Write("YOU: ");
                string userInput = Console.ReadLine();

                using (HttpClient httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
                    string apiUrl = "https://api.wit.ai/message?q=" + Uri.EscapeDataString(userInput);

                    HttpResponseMessage response = httpClient.GetAsync(apiUrl).GetAwaiter().GetResult();
                    string jsonResponse = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();

                    JavaScriptSerializer jss = new JavaScriptSerializer();

                    IntentRoot intentRoot = jss.Deserialize<IntentRoot>(jsonResponse);

                    Intent likelyIntent = intentRoot.intents.First();

                    foreach (Intent intent in intentRoot.intents)
                    {
                        if (intent.confidence > likelyIntent.confidence)
                        {
                            likelyIntent = intent;
                        }
                    }

                    Console.WriteLine("AI: " + GetResponseFor(likelyIntent.name));
                }
            }

        }
    }
}
