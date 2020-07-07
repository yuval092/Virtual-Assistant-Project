using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Newtonsoft.Json;
using System.Device.Location;


namespace AssistantSolution
{
    class WebServices
    {
        private static string GetWeatherURL = "http://api.weatherstack.com/current?" +
            "access_key=0c7d8a533a7ccf48be7f4154facef39c&query=";
        

        public static string GetWeather(string req, string intent, List<string> slots)
        {
            try
            {
                dynamic obj = GetWeatherHandler(ExtractSlots(req, intent, slots));

                string result = "The current temperature is {0} and the weather description is {1}.";
                return string.Format(result, obj.current["temperature"], obj.current["weather_descriptions"][0]);
            }
            catch(Exception e)
            {
                return e.Message;
            }
        }

        public static Dictionary<string, string> ExtractSlots(string req, string intent, List<string> slots)
        {
            string[] words = req.Split(' ');
            string searchSlot = "B-country";

            Dictionary<string, string> result = new Dictionary<string, string>()
            {
                {"country", "" },
                {"city", "" },
                {"current_location", "" },
                {"time_range", "" }
            };

            if(intent == "GetWeather")
            {

                while (slots.IndexOf(searchSlot) != -1)
                {
                    result["country"] += words[slots.IndexOf(searchSlot)] + " ";
                    slots[slots.IndexOf(searchSlot)] = "O";
                    searchSlot = "I-country";
                }

                searchSlot = "B-city";
                while (slots.IndexOf(searchSlot) != -1)
                {
                    result["city"] += words[slots.IndexOf(searchSlot)] + " ";
                    slots[slots.IndexOf(searchSlot)] = "O";
                    searchSlot = "I-city";
                }

                if (slots.IndexOf("B-current_location") != -1)
                    result["current_location"] += "here";

                searchSlot = "B-timeRange";
                while (slots.IndexOf(searchSlot) != -1)
                {
                    result["time_range"] += words[slots.IndexOf(searchSlot)] + " ";
                    slots[slots.IndexOf(searchSlot)] = "O";
                    searchSlot = "I-timeRange";
                }
            }

            return result;
        }

        private static dynamic GetWeatherHandler(Dictionary<string, string> values)
        {
            string location = "";

            if (values["current_location"] != "")
            {
                var resp = WebRequest.Create("https://api.ipify.org/").GetResponse();
                location = new StreamReader(resp.GetResponseStream()).ReadToEnd();
            }
            else if (values["city"] != "")
                location = values["city"];
            else if (values["country"] != "")
                location = values["country"];

            if(values["time_range"] != "")
            {
                string value = values["time_range"].ToLower();
                //if (value == "next day" || value == "tomorrow")
                //    time = 1;
                //else if (value == "next week")
                //    time = 7;
                if (value != "today" && value != "now" && value != "right now" && value != "currently")
                    throw new Exception("Sorry, the current api plan can't get forecasts.");
            }

            var httpWebRequest = (HttpWebRequest)WebRequest.Create(GetWeatherURL + location);
            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();

            var streamReader = new StreamReader(httpResponse.GetResponseStream());
            string result = streamReader.ReadToEnd();
            streamReader.Close();

            return JsonConvert.DeserializeObject(result);
        }
    }
}
