using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace xerox_operations.api
{
    public class PushNotifications
    {
        private string time;
        private string text;

        private readonly static string FIREBASE_LINK = @"https://fcm.googleapis.com/fcm/send";
        private readonly static string INTERFACE_API_KEY = @"AIzaSyCQF9aVxCaT6ohrylsofE7wGLQKqR-Tito";
        private readonly static string SENDER_ID = @"1914897138";

        string phone1 = "dsbLY1d3wvQ:APA91bG_1PIO-Uv_55ZLt7bd3sAOkZyglmPofBgsyaIaOQn-C_kn6lZdCHFXU_D_1xDU4T3bACRFKKC5PHgkxDDdE2mneH-wfEy5vZ0__YajvSS8LxB0y9fesZZO901CDhItPr6vhQTX";
        string phone2 = "c-jCj70hNkg:APA91bFU-6V_JliuzwQyTpLVBRp7zQwkSHB79qga6Vk7ZQGmsSvRw88Jv56UmkY6RsfeFdHXPouenm3TYsWIAzqvBg19r-NOE78XlX_Dklu__c6s4rnd8GPBK8rXzsNN-BZRobN6CA1e";

        public PushNotifications(string time, string text)
        {
            this.time = time;
            this.text = text;
            if (time != null && text != null)
            {
                sendNotification();
                //Thread.Sleep(1000);
                sendNotificationWithVibration();
            }

        }

        public void sendNotification()
        {
            WebRequest tRequest = WebRequest.Create(FIREBASE_LINK);
            tRequest.Method = "post";
            //serverKey - Key from Firebase cloud messaging server  
            tRequest.Headers.Add(string.Format("Authorization: key={0}", INTERFACE_API_KEY));
            //Sender Id - From firebase project setting  
            tRequest.Headers.Add(string.Format("Sender: id={0}", SENDER_ID));
            tRequest.ContentType = "application/json";

            var payload = new
            {
                to = phone1,
                priority = "high",
                content_available = false,
                notification = new
                {
                    title = time,
                    body = text,
                    badge = 3,
                    sound = "default",
                    vibrate = 1000
                },
            };

            string postbody = JsonConvert.SerializeObject(payload).ToString();
            Byte[] byteArray = Encoding.UTF8.GetBytes(postbody);
            tRequest.ContentLength = byteArray.Length;
            using (Stream dataStream = tRequest.GetRequestStream())
            {
                dataStream.Write(byteArray, 0, byteArray.Length);
                using (WebResponse tResponse = tRequest.GetResponse())
                {
                    using (Stream dataStreamResponse = tResponse.GetResponseStream())
                    {
                        if (dataStreamResponse != null) using (StreamReader tReader = new StreamReader(dataStreamResponse))
                            {
                                String sResponseFromServer = tReader.ReadToEnd();
                                //result.Response = sResponseFromServer;
                            }
                    }
                    dataStream.Close();
                }
            }
        }

        public void sendNotificationWithVibration()
        {
            WebRequest tRequest = WebRequest.Create(FIREBASE_LINK);
            tRequest.Method = "post";
            //serverKey - Key from Firebase cloud messaging server  
            tRequest.Headers.Add(string.Format("Authorization: key={0}", INTERFACE_API_KEY));
            //Sender Id - From firebase project setting  
            tRequest.Headers.Add(string.Format("Sender: id={0}", SENDER_ID));
            tRequest.ContentType = "application/json";

            var payload = new
            {
                to = phone1,
                priority = "high",
                content_available = false,               
            };

            string postbody = JsonConvert.SerializeObject(payload).ToString();
            Byte[] byteArray = Encoding.UTF8.GetBytes(postbody);
            tRequest.ContentLength = byteArray.Length;
            using (Stream dataStream = tRequest.GetRequestStream())
            {
                dataStream.Write(byteArray, 0, byteArray.Length);
                using (WebResponse tResponse = tRequest.GetResponse())
                {
                    using (Stream dataStreamResponse = tResponse.GetResponseStream())
                    {
                        if (dataStreamResponse != null) using (StreamReader tReader = new StreamReader(dataStreamResponse))
                            {
                                String sResponseFromServer = tReader.ReadToEnd();
                                //result.Response = sResponseFromServer;
                            }
                    }
                    dataStream.Close();
                }
            }            
        }
    }

    
}
