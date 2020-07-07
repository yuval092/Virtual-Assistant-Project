using AssistantSolution.Responses;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace AssistantSolution
{
    class CommunicationHandler
    {
        private static readonly string HOST = "http://bac6701c.ngrok.io";
        private static readonly string INTENT_SLOTS_PATH = "/intent_and_slots";


        public static IntentAndSlotsResponse getIntentAndSlots(string req)
        {
            string address = HOST + INTENT_SLOTS_PATH;

            var httpWebRequest = (HttpWebRequest)WebRequest.Create(address);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";

            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                streamWriter.Write(req);
            }

            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            string result = "";
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                result = streamReader.ReadToEnd();
            }

            return JsonConvert.DeserializeObject<IntentAndSlotsResponse>(result);
        }
    }
}
