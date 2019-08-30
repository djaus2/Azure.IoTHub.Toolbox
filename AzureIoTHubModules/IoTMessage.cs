using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Microsoft.Azure.Devices.Client;
using Microsoft.Azure.EventHubs;
using Newtonsoft.Json;

namespace Azure_IoTHub_Telemetry
{
    // A Simple Class to pass Microsoft.Azure.Devices.Client.Message properties
    public class SyntheticIoTMessage
    {
        public string MessageId { get; private set; }
        public string MessageAsString { get; set; }
        [NonSerialized]
        public byte[] bytes;
        public List<Tuple<string,string>> Properties { get; set; } 
        public string UserId { get; private set; }
        public uint DeliveryCount { get; private set; }
        public DateTime CreationTimeUtc { get; private set; }
        public string CorrelationId { get; private set; }
        public string ContentType { get; private set; }
        public string ContentEncoding { get; private set; }
        public string To { get; private set; }
        public string MessageSchema { get; private set; }

        //Needed for serialization:
        public SyntheticIoTMessage()
        {

        }

        // Construct instance of this clas form Microosft.Azure.Devices.Client.Message
        public SyntheticIoTMessage(Message message)
        {
            try
            {
                MessageId = message.MessageId;
                bytes = message.GetBytes();
                MessageAsString = Encoding.UTF8.GetString(bytes, 0, bytes.Length);
                Properties = new List<Tuple<string, string>>();

                foreach (var prop in message.Properties)
                {
                    Properties.Add(new Tuple<string, string>(prop.Key, prop.Value));
                }
                UserId = message.UserId;
                CreationTimeUtc = message.CreationTimeUtc;
                CorrelationId = message.CorrelationId;
                ContentType = message.ContentType;
                ContentEncoding = message.ContentEncoding;
                To = message.To;
                MessageSchema = message.MessageSchema;
            }
            catch (Exception)
            {
            }
        }

        public Message ToMessage()
        {
            try
            {
                Message message = new Message(Encoding.ASCII.GetBytes(MessageAsString));
                message.MessageId = MessageId;
                foreach (var prop in Properties)
                    message.Properties.Add(prop.Item1, prop.Item2);
                message.UserId = UserId;
                message.CreationTimeUtc = CreationTimeUtc;
                message.CorrelationId = CorrelationId;
                message.ContentType = ContentType;
                message.ContentEncoding = ContentEncoding;
                message.To = To;
                message.MessageSchema = MessageSchema;
                return message;
            }
            catch (Exception )
            {
                return null;
            }
        }

        public EventData ToEventData()
        {
            try
            {
                Message message = this.ToMessage();
                EventData eventData = new EventData(message.GetBytes());
                foreach (var prop in message.Properties)
                    eventData.Properties.Add(prop.Key, prop.Value);
                return eventData;
            } catch (Exception)
            {
                return null;
            }
        }

        public static EventData ToEventData(Message message)
        {
            try
            {
                EventData eventData = new EventData(message.GetBytes());
                foreach (var prop in message.Properties)
                    eventData.Properties.Add(prop.Key, prop.Value);
                return eventData;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public string Serialise()
        {
            return JsonConvert.SerializeObject(this);
        }

        public static SyntheticIoTMessage Deserialize(string msg)
        {
            try
            {
                object obj = JsonConvert.DeserializeObject(msg);
                SyntheticIoTMessage iMsg = JsonConvert.DeserializeObject<SyntheticIoTMessage>(msg);
                return (SyntheticIoTMessage)iMsg;
            }
            catch (Exception)
            {
                return null;
            }

        }

        public static string EventData_ToString(EventData eventData)
        {
            string response = "";
            if (eventData == null)
                response = "Null";
            else
            {
                string data = "";
                if (eventData.Body != null)
                    data = Encoding.UTF8.GetString(eventData.Body.Array);
                response += string.Format("Message received on partition {0}:\r\n", 0);
                //response += string.Format("\r\n  {0}:", data);
                Azure_IoTHub_Sensors.TelemetryDataPoint telemetryDataPoint =
                    Azure_IoTHub_Sensors.TelemetryDataPoint.Deserialize(data);
                if (!string.IsNullOrEmpty(telemetryDataPoint.city))
                    response += string.Format("City:  {0}", telemetryDataPoint.city);
                if (telemetryDataPoint.temperature != -123456)
                    response += string.Format("\r\nTemperature:  {0} C", telemetryDataPoint.temperature);
                if (!(telemetryDataPoint.humidity <0))
                    response += string.Format("\r\nHumidity:  {0}%", telemetryDataPoint.humidity);
                if (!(telemetryDataPoint.pressure<0))
                    response += string.Format("\r\nPressure:  {0}", telemetryDataPoint.pressure);


                if (eventData.Properties != null)
                    if (eventData.Properties.Count != 0)
                    {
                    response += string.Format("\r\nApplication properties (set by device):");
                    foreach (var prop in eventData.Properties)
                    {
                        response += string.Format("\r\n  {0}: {1}", prop.Key, prop.Value);
                    }
                }

                if (eventData.SystemProperties != null)
                    if (eventData.SystemProperties.Count != 0)
                    {
                    response += string.Format("\r\nSystem properties (set by IoT Hub):");
                    foreach (var prop in eventData.SystemProperties)
                    {
                        response += string.Format("\r\n  {0}: {1}", prop.Key, prop.Value);
                    }
                }
                
            }
            return response;
        }


    }
}
