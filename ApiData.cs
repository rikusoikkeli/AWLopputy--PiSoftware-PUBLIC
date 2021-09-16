using Microsoft.Azure.CognitiveServices.Vision.Face.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PiRecordingControls
{
    public class ApiData
    {
        public DateTime Time { get; set; }
        public double Anger { get; set; }
        public double Happiness { get; set; }
        public double Sadness { get; set; }
        public double Neutral { get; set; }
        public double Surprise { get; set; }
        public double Disgust { get; set; }
        public string DeviceId { get; set; }

        public ApiData(string imagePath, Emotion emotion, string deviceId)
        {
            Time = ParseTimeFromPath(imagePath);
            Anger = emotion.Anger;
            Happiness = emotion.Happiness;
            Sadness = emotion.Sadness;
            Neutral = emotion.Neutral;
            Surprise = emotion.Surprise;
            Disgust = emotion.Disgust;
            DeviceId = deviceId;
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }

        /// <summary>
        /// Parseroi kuvanottoajan kuvista, jotka on nimetty seuraavan tyyppisesti: photo_2021-09-04_17-12-25-772738.jpg
        /// </summary>
        /// <param name="imagePath"></param>
        /// <returns></returns>
        public DateTime ParseTimeFromPath(string imagePath)
        {
            var fileName = Path.GetFileName(imagePath);
            var tempArr = fileName.Split("_");
            var date = tempArr[1].Split("-"); 
            var time = tempArr[2].Split("-");
            var dateTime = new DateTime(
                int.Parse(date[0]), 
                int.Parse(date[1]), 
                int.Parse(date[2]), 
                int.Parse(time[0]), 
                int.Parse(time[1]), 
                int.Parse(time[2])
            );
            return dateTime;
        }
    }
}
