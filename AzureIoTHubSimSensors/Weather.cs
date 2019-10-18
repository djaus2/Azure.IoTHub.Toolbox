using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Reflection;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;

namespace Azure_IoTHub_Sensors
{
    public class TelemetryDataPoint
    {
        public const string Prefix = "TELEM";
        public string city { get; set; } = "";
        public double temperature { get; set; } = -123456; //Use these initialisers to indicate no value supplied
        public double pressure { get; set; } = -1;
        public double humidity { get; set; } = -1;
        public static int WeatherIndex { get; set; }

        public TelemetryDataPoint()
        {

        }

        public static TelemetryDataPoint Deserialize(string msg)
        {
            try
            {
                TelemetryDataPoint iMsg = JsonConvert.DeserializeObject<TelemetryDataPoint>(msg);
                return (TelemetryDataPoint)iMsg;
            }
            catch (Exception)
            {
                JObject obj = (JObject)JsonConvert.DeserializeObject(msg);

                
                if (obj != null)
                {
                    var to =  new TelemetryDataPoint();
                    ////https://stackoverflow.com/questions/4144778/get-properties-and-values-from-unknown-object
                    //Type myType = obj.GetType();
                    //IList<PropertyInfo> props = new List<PropertyInfo>(myType.GetProperties());
                    ////////////////////
                    foreach (var property in obj.Properties()) //(System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic))
                    {
                        string propertyName = property.Name;
                        var val = obj.GetValue(propertyName); // static classes cannot be instanced, so use null...
                        

                        Type type2 = typeof(TelemetryDataPoint); // IoTHubConnectionDetails is static class with public static properties
                        foreach (var property2 in type2.GetProperties())//System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static)) //(System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic))
                        {
                            string propertyName2 = property2.Name;
                            if (propertyName2.ToLower() == propertyName.ToLower())
                            {
                               
                                var propertyInfo = type2.GetProperty(propertyName2); //, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
                                string strngVal = val.ToString();

                                //Assume its a string, but try to convert to double and int
                                object obj2 = strngVal;
                                int i;
                                double d;
                                if (double.TryParse(strngVal, out d))
                                {
                                    obj2 = d;
                                }
                                else if (int.TryParse(strngVal, out i))
                                {
                                    obj2 = i;
                                }

                                
                                //var info = propertyInfo.GetValue(type2, null);
                                propertyInfo.SetValue(to, obj2);
                            }
                        }
                    }
                    return to;
                }
                return null; 
            }

        }


        public  override string ToString()
        {
            string response = "";
            if (city !="")
                response += string.Format("City:  {0}", city);
            if (temperature != -123456)
                response += string.Format("\r\nTemperature:  {0} C", temperature);
            if (humidity != -1)
                response += string.Format("\r\nHumidity:  {0}%", humidity);
            if (pressure != -1)
                response += string.Format("\r\nPressure:  {0}", pressure);      
            return response;
        }
    }

    public  abstract class Weather
    {
        public static Weather CurrentWeather { get; set; } = null;
        public abstract TelemetryDataPoint GetWeather();

        public abstract  Task<TelemetryDataPoint> GetWeatherAsync();

        public bool DoAsync { get; set; } = false;

        public override string ToString()
        {
            return JsonConvert.SerializeObject(GetWeather());
        }

    }

    public  class Weather_Fixed: Weather
    {
        public  override TelemetryDataPoint GetWeather()
        {

            double currentTemperature = 27;
            double currentHumidity = 15;
            double currentPressure = 1001;

            var telemetryDataPoint = new TelemetryDataPoint
            {
                city = "Xanadu",
                pressure = (int)currentPressure,
                temperature = (int)currentTemperature,
                humidity = (int)currentHumidity
            };

            return telemetryDataPoint;
        }

        public override Task<TelemetryDataPoint> GetWeatherAsync()
        {
            throw new NotImplementedException();
        }
    }
    public  class Weather_Random: Weather
    {
        private  double minTemperature = 20;
        private  double minHumidity = 60;
        private  Random rand = new Random();
        public override TelemetryDataPoint GetWeather()
        {

            double currentTemperature = minTemperature + rand.NextDouble() * 15;
            double currentHumidity = minHumidity + rand.NextDouble() * 20;
            double currentPressure = 0 + rand.NextDouble() * 100;

            var telemetryDataPoint = new TelemetryDataPoint
            {
                city = "Xanadu",
                pressure = (int)currentPressure,
                temperature = (int)currentTemperature,
                humidity = (int)currentHumidity
            };

            return telemetryDataPoint;
        }

        public override Task<TelemetryDataPoint> GetWeatherAsync()
        {
            throw new NotImplementedException();
        }
    }
    public  class Weather_FromCities: Weather
    {
        public static string OpenWeatherAppKey { get; set; } = "df39100f7fe7b297c789818c5f2bb1bd";//Need this https://openweathermap.org/

        public static async Task<string> GetAsync(string uri)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);

            using (HttpWebResponse response = (HttpWebResponse)await request.GetResponseAsync())
            using (Stream stream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream))
            {
                return await reader.ReadToEndAsync();
            }
        }

        public class City
        {
            public City()
            { }

            public int id { get; set; }
            public string name { get; set; }
            public string country { get; set; }

            public Coords coord { get; set; }

            public class Coords
            {
                public float lon { get; set; }

                public float lat { get; set; }
            }
        }

        public static void GetNextCity()
        {
            CurrentCityIndex++;
            if (CurrentCityIndex >= Cities.Length)
                CurrentCityIndex = 0;
        }
        public static int CurrentCityIndex { get; set; } = -1;
        public static City[] Cities { get; set; } = null;
        public static void ReadCities()
        {
            string TempFile = "Assets\\cities.json";
            var fileStream = new FileStream(TempFile, FileMode.Open, FileAccess.Read);
            using (var streamReader = new StreamReader(fileStream, Encoding.UTF8))
            {
                string weatherjson = streamReader.ReadToEnd();
                Cities = JsonConvert.DeserializeObject<City[]>(weatherjson);
            }
            CurrentCityIndex = 0;
        }

        public  override TelemetryDataPoint GetWeather()
        {
            return GetWeatherAsync().GetAwaiter().GetResult();
        }

        public override async Task<TelemetryDataPoint> GetWeatherAsync()
        {
            if (CurrentCityIndex == -1)
                ReadCities();
            string url = string.Format("http://api.openweathermap.org/data/2.5/weather?id={0}&appid={1}",
                Cities[CurrentCityIndex].id, OpenWeatherAppKey);
            string weatherjson = await GetAsync(url);
            dynamic obj = JsonConvert.DeserializeObject(weatherjson);


            dynamic temp = obj.main.temp;
            dynamic press = obj.main.pressure;
            dynamic humid = obj.main.humidity;
            var otemperature = (int)(float.Parse(temp.ToString()));
            var opressure = (int)(int.Parse(press.ToString()));
            var ohumidity = (int)(int.Parse(humid.ToString()));
            var telemetryDataPoint = new TelemetryDataPoint()
            {
                city = Cities[CurrentCityIndex].name,
                temperature = otemperature - 273,
                pressure = opressure,
                humidity = ohumidity
            };
            GetNextCity();
            return telemetryDataPoint;
        }

        public Weather_FromCities():base()
        {
            DoAsync = true;
        }

    }
    public  class Weather_FromHardware: Weather
    {
        public override TelemetryDataPoint GetWeather()
        {
            //Get weather from (Arduino) device
            double currentTemperature = 27;
            double currentHumidity = 15;
            double currentPressure = 1001;

            var telemetryDataPoint = new TelemetryDataPoint
            {
                city = "Xanadu",
                pressure = (int)currentPressure,
                temperature = (int)currentTemperature,
                humidity = (int)currentHumidity
            };

            return telemetryDataPoint;
        }

        public override Task<TelemetryDataPoint> GetWeatherAsync()
        {
            throw new NotImplementedException();
        }

    }


}
