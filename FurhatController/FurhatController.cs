using System;
using System.Collections.Generic;
using System.Composition;
using System.Text;
using Microsoft.Psi;
using FurhatGesture = OpenSense.Components.VirtualRapport.FurhatGesture;
using System.Web;


namespace OpenSense.Components.FurhatController {
    public sealed class Furhat {

        public static string furhatUri = "http://172.16.31.15:54321/furhat";
        public DateTime lastGestureTimestamp = new DateTime(DateTime.Now.Ticks);
        public int minGestureInterval = 5;
        public bool introFurhatPending = true;
        public Receiver<FurhatGesture> GestureIn { get; }
        public Furhat(Pipeline pipeline) {
            GestureIn = pipeline.CreateReceiver<FurhatGesture>(this, DoGesture, nameof(GestureIn));
        }

        private async void DoGesture(FurhatGesture data, Envelope envelope) {
            if (introFurhatPending) {
                Speak(new HttpClient(), "Hi, my name is Furhat. Can you tell me a little about yourself?");
                introFurhatPending = false;
            }
            string gesture = data.ToString();

            if ((DateTime.Now - lastGestureTimestamp).TotalSeconds < minGestureInterval) {
                return;
            }

            using (HttpClient client = new HttpClient()) {
                string uri = furhatUri + "/gesture";
                string encodedText = HttpUtility.UrlEncode(gesture);
                var uriBuilder = new UriBuilder(uri);
                uriBuilder.Query = $"name={encodedText}";

                HttpContent content = new StringContent("", Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PostAsync(uriBuilder.Uri, content);

                if (response.IsSuccessStatusCode) {
                    string responseBody = await response.Content.ReadAsStringAsync();
                    Console.WriteLine("Sent {gesture} command to Furhat");
                    lastGestureTimestamp = DateTime.Now;
                } else {
                    Console.WriteLine($"Error sending getsure command to Furhat: {response.StatusCode}");
                }

            }
        }
        private async Task TestConnection(HttpClient client) {

            HttpResponseMessage response = await client.GetAsync(furhatUri);

            if (response.IsSuccessStatusCode) {
                string responseBody = await response.Content.ReadAsStringAsync();
                Console.WriteLine("Connection to Furhat successful!");
            } else {
                Console.WriteLine($"Error in the connection: {response.StatusCode} {response.ReasonPhrase}");
            }
            
        }
        private async Task Speak(HttpClient client, string text) {
            string uri = furhatUri + "/say";

            //string text = "Hi, my name is Furhat. How are you doing today?";
            string encodedText = HttpUtility.UrlEncode(text);

            var uriBuilder = new UriBuilder(uri);
            uriBuilder.Query = $"text={encodedText}";

            HttpContent content = new StringContent("", Encoding.UTF8, "application/json");

            HttpResponseMessage response = await client.PostAsync(uriBuilder.Uri, content);


            if (response.IsSuccessStatusCode) {
                string responseBody = await response.Content.ReadAsStringAsync();
                Console.WriteLine("Sent text utterance to Furhat");
                //Console.WriteLine(responseBody);
            } else {
                Console.WriteLine($"Error trying to make Furhat speak: {response.StatusCode}");
            }
        }
    }
}
