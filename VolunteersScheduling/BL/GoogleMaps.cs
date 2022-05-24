using Google.Protobuf.WellKnownTypes;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace BL
{
    public class DistanceBase
    {
        public string[] destination_addresses { get; set; }
        public string[] origin_addresses { get; set; }
        public Row[] rows { get; set; }
        public string status { get; set; }

    }
    public class Row
    {
        public Element[] elements { get; set; }
    }
    public class Element
    {
        public Distance distance { get; set; }
        public Duration duration { get; set; }
        public string status { get; set; }
    }

    public class Distance
    {
        public string text { get; set; }
        public int value { get; set; }
    }

    public class Duration
    {
        public string text { get; set; }
        public int value { get; set; }
    }

    public class RootLocationBase
    {
        public Result[] results { get; set; }
    }

    public class Result
    {
        public string place_id { get; set; }
    }

    public class GoogleMaps
    {
        public static async Task<double> GetDistanceInMinutes(string origin, string destination)
        {
            string[] locationUrls = { BuildUrlForLocationId(origin), BuildUrlForLocationId(destination) },
            
            idLoction = new string[2];
            HttpClient http = new HttpClient();
            for (int i = 0; i < idLoction.Length; i++)
            {
                var responseId = Task.Run(() => http.GetAsync(locationUrls[i]));
                if (responseId.Result != null)
                {
                    var result = await responseId.Result.Content.ReadAsStringAsync();
                    RootLocationBase root = JsonConvert.DeserializeObject<RootLocationBase>(result);
                    idLoction[i] = root.results[0].place_id;
                }
            }
            var responseDistance = Task.Run(() => http.GetAsync(BuildUrlForDistance(idLoction[0], idLoction[1])));
            if (responseDistance.Result != null)
            {
                var result = await responseDistance.Result.Content.ReadAsStringAsync();
                DistanceBase root = JsonConvert.DeserializeObject<DistanceBase>(result);
                double f = root.rows[0].elements[0].duration.value;
                double minutes = f / 60;
                return minutes;
            }
            return 0.0;
        }

        //בניית כתובת עבור הגוגל מפס
        static string BuildUrlForLocationId(string address)
        {
            return "https://maps.googleapis.com/maps/api/place/textsearch/json?key=AIzaSyAyEYq8aMTFp3eNcRxWj3z4rFPLx7BamYo&query=" + address;
        }
        //בניית פונקציית המרחק
        static string BuildUrlForDistance(string path1, string path2)
        {
            string url = "https://maps.googleapis.com/maps/api/distancematrix/json?destinations=place_id:";
            url += path1 + "&origins=place_id:" + path2 + "&key=AIzaSyAyEYq8aMTFp3eNcRxWj3z4rFPLx7BamYo";
            return url;
        }
    }
}
