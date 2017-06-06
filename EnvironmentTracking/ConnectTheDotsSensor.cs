using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;

namespace EnvironmentTracking
{
    /// <summary>
    /// Class to manage sensor data and attributes 
    /// </summary>
    public class ConnectTheDotsSensor
    {
        public string guid { get; set; }
        public string deviceid { get; set; }
        public string humidity { get; set; }
        public string location { get; set; }
        public string temperatureC { get; set; }
        public string temperatureF { get; set; }
        public string smog { get; set; }

        /// <summary>
        /// Default parameterless constructor needed for serialization of the objects of this class
        /// </summary>
        public ConnectTheDotsSensor()
        {
        }

        /// <summary>
        /// Construtor taking parameters guid, measurename and unitofmeasure
        /// </summary>
        /// <param name="guid"></param>
        /// <param name="measurename"></param>
        /// <param name="unitofmeasure"></param>
        public ConnectTheDotsSensor(string guid)
        {
            this.guid = guid;
        }

        /// <summary>
        /// ToJson function is used to convert sensor data into a JSON string to be sent to Azure Event Hub
        /// </summary>
        /// <returns>JSon String containing all info for sensor data</returns>
        public string ToJson()
        {
            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(ConnectTheDotsSensor));
            MemoryStream ms = new MemoryStream();
            ser.WriteObject(ms, this);
            string json = Encoding.UTF8.GetString(ms.ToArray(), 0, (int)ms.Length);

            return json;
        }
    }
}
