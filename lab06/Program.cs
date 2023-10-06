﻿using System.Net;
using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace lab06{

    public class Weather
    {
        public string Country { get; set; }
        public string Name { get; set; }
        public double Temp { get; set; }
        public string Description { get; set; }

        public Weather(WeatherResponse weatherResponse)
        {
            this.Country = weatherResponse.Sys.Country;
            this.Name = weatherResponse.Name;
            this.Temp = weatherResponse.Main.Temp;
            this.Description = weatherResponse.Weather[0].Description;
        }
        
        public void Print()
        {
            Console.Write("country: " + Country + ", ");
            Console.Write("name of place: " + Name + ", ");
            Console.Write("description: " + Description + ", ");
            Console.Write("temperature: " + Temp + "\n");
        }
    }

    public class WeatherResponse
    {
        public TemperatureInfo Main { get; set; }
        public CountryInfo Sys { get; set; }
        public string Name { get; set; }
        public DescriptionInfo[] Weather { get; set; }

        public void Print()
        {
            Console.Write("country: " + Sys.Country + ", ");
            Console.Write("name of place: " + Name + ", ");
            Console.Write("description: " + Weather[0].Description + ", ");
            Console.Write("temperature: " + Main.Temp + "\n");
        }

        public class TemperatureInfo
        {
            public float Temp { get; set; }
        }

        public class DescriptionInfo
        {
            public string Description { get; set; }
        }

        public class CountryInfo
        {
            public string Country { get; set; }
        }
    }

    public class Program{
        public static async Task<string> Get(string url)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                try
                {
                    HttpResponseMessage response = await httpClient.GetAsync(url);

                    if (response.IsSuccessStatusCode)
                    {
                        return await response.Content.ReadAsStringAsync();
                    }
                    else
                    {
                        throw new HttpRequestException($"HTTP Error: {response.StatusCode} - {response.ReasonPhrase}");
                    }
                }
                catch (HttpRequestException e)
                {
                    throw new HttpRequestException($"Request error: {e.Message}");
                }
            }
        }


        static async Task Main(){
            const string API_KEY = "9b7551f0eb91104dcd252bd651ec2d08";

            Random RandomNumber = new Random();

            

            var WeatherArray = new Weather[50];

            int count = 0;

            while (count < 50){
                double lat = RandomNumber.Next(-90, 90);
                double lon = RandomNumber.Next(-180, 180);
                string url = $"https://api.openweathermap.org/data/2.5/weather?lat={lat}&lon={lon}&appid={API_KEY}";
                string content = await Get(url);

                WeatherResponse response = JsonConvert.DeserializeObject<WeatherResponse>(content);
                
                if (response.Name != ""){
                    Weather WeatherRecord = new Weather(response);
                    WeatherArray[count] = WeatherRecord;
                    count++;
                    continue;
                }
                
            }

            Console.Write("Country with maximal temperature: " + WeatherArray.MaxBy(w => w.Temp).Country + " ");

            Console.Write("(" + WeatherArray.MaxBy(w => w.Temp).Temp + " C)\n\n");

            Console.Write("Country with minimal temperature: " + WeatherArray.MinBy(w => w.Temp).Country);
            Console.Write("(" + WeatherArray.MinBy(w => w.Temp).Temp + " C)\n\n");

            Console.WriteLine("Average temperature in the world: " + WeatherArray.Average(w => w.Temp) + "\n");

            Console.WriteLine("Number of countries in collection: " + WeatherArray.Select(w => w.Country).Distinct().Count() + "\n");

            var firstMatch = WeatherArray.FirstOrDefault(w => w.Description == "clear sky" || w.Description == 
                "rain" || w.Description == "few clouds");

            Console.WriteLine($"The first found country and name of place with suitable description: " +
                            $"{firstMatch.Country}, {firstMatch.Name} with {firstMatch.Description}");
        }
    }
}