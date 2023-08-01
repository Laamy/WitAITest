using System.Net.Http;
using System;
using System.Collections.Generic;
using System.Web.Script.Serialization;
using System.Linq;

namespace TEstThignydsg
{
    internal class Program
    {
        // dictionary used to store respones for each intent category
        static Dictionary<string, List<string>> responses = new Dictionary<string, List<string>>
        {
            // you can add more intents here
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

            { "hooks_detours",
                new List<string> {
                    "Hooks are basically like a path with a sign that tells you what to do and you just change that info with ur own, detours on the other hand are like erasing part of the path and detouring it to ur own"
                }
            },

            { "how_are_you",
                new List<string> {
                    "Im doing great! you?",
                    "Im doing good! you?",
                    "Im doing fine! you?",
                    "Im doing great! how about you?",
                    "Great! how about you?",
                    "Good! how about you?",
                    "Fine! how about you?",
                    "Im doing great! how are you?",
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
            // TODO: if the intent category doesnt exist in our dictionary
            return responses[intentCategory][ran.Next(0, responses[intentCategory].Count)];
        }

        static void Main(string[] args)
        {
            // YOUR_API_KEY_HERE
            string accessToken = "YOUR_API_KEY_HERE";

            // TODO: setup C# action parsing on response before sending for stuff like date

            // you'll have to setup the intents yourself on wit.ai (as i wont be providing mine)
            // aswell as train it with your own test prompts so it understands the intentions
            while (true)
            {
                // get userinput
                Console.Write("YOU: ");
                string userInput = Console.ReadLine();

                // setup http request to wit.ai
                using (HttpClient httpClient = new HttpClient())
                {
                    // api url & auth headers
                    httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
                    string apiUrl = "https://api.wit.ai/message?q=" + Uri.EscapeDataString(userInput);

                    // send request & get response
                    HttpResponseMessage response = httpClient.GetAsync(apiUrl).GetAwaiter().GetResult();
                    string jsonResponse = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();

                    // deserialize json response
                    JavaScriptSerializer jss = new JavaScriptSerializer();
                    IntentRoot intentRoot = jss.Deserialize<IntentRoot>(jsonResponse);

                    // check if we got any intents back else respond with unknown intent
                    // (not defined in ai intents btw)
                    if (intentRoot.intents.Count < 1)
                    {
                        Console.WriteLine("AI: [ERROR] " + GetResponseFor("unknown"));
                        continue;
                    }

                    // get the most likely intent
                    Intent likelyIntent = intentRoot.intents.First();

                    foreach (Intent intent in intentRoot.intents)
                    {
                        // if the current intent is more likely then the previous one
                        if (intent.confidence > likelyIntent.confidence)
                        {
                            likelyIntent = intent;
                        }
                    }

                    // respond with the most likely intent response from our dictionary
                    Console.WriteLine("AI: " + GetResponseFor(likelyIntent.name));
                }
            }
        }
    }
}
