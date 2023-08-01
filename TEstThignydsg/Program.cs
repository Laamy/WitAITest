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
            { "greeting",
                new List<string> {
                    "Hello",
                    "Hi",
                    "Hey",
                    "Heyo",
                    "Hiya",
                    "Hi there",
                    "Hello there",
                    "Hey there",
                    "Hi there, how are you?",
                    "Hello there, how are you?",
                    "Hey there, how are you?",
                }
            },

            { "goodbye",
                new List<string> {
                    "cya",
                    "goodbye",
                    "later then",
                    "bye",
                    "see ya",
                    "see you later",
                    "see you later then",
                    "see you later then, bye",
                }
            },

            { "thanks",
                new List<string> {
                    "your welcome",
                    "your very welcome",
                    "no problem",
                    "no problem, your welcome",
                    "no problem, your very welcome",
                    "no problem, glad i could help"
                }
            },

            { "unknown",
                new List<string> {
                    "I dont understand what your trying to say",
                    "Huh? please be more specific, im confused",
                    "I dont know what your trying to say",
                    "I dont understand",
                    "mind explaining? im confused",
                    "I dont understand what your trying to say, please be more specific",
                    "please be more specific",
                    "I dont understand, please be more specific",
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
            // YOUR_API_KEY_HERE
            string accessToken = "YOUR_API_KEY_HERE";

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

                    if (intentRoot.intents.Count < 1)
                    {
                        Console.WriteLine("AI: [ERROR] " + GetResponseFor("unknown"));
                        continue;
                    }

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
