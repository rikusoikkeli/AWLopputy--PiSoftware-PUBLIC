using FileUploadSample;
using Microsoft.Azure.Devices.Client;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace PiRecordingControls
{
    public class MqttUtil
    {
        private DeviceClient _deviceClient;
        private TimeSpan _timeBetweenPhotoAnalysis;
        private TimeSpan _timeBetweenMqttMessages;
        private IConfiguration _configuration;

        public MqttUtil(DeviceClient deviceClient, IConfiguration configuration)
        {
            _deviceClient = deviceClient;
            _configuration = configuration;
            _timeBetweenPhotoAnalysis = TimeSpan.FromSeconds(int.Parse(configuration["Settings:TimeBetweenPhotoAnalysis"]));
            _timeBetweenMqttMessages = TimeSpan.FromSeconds(int.Parse(configuration["Settings:TimeBetweenMqttMessages"]));
        }

        /// <summary>
        /// Päivittää tiedoston PhotoCaptureState.json uudella sisällöllä newState.
        /// </summary>
        /// <param name="newState"></param>
        /// <returns></returns>
        public async Task UpdatePhotoCaptureState(PhotoCaptureState newState)
        {
            await File.WriteAllTextAsync("PhotoCaptureState.json", JsonConvert.SerializeObject(newState));
        }

        /// <summary>
        /// Käytännössä koko ohjelman elinkaari sisältyy tähän silmukkaan. Vastaanottaa MQTT-viestejä IoTHubista. 
        /// Ja jos IoTHub-haluaa, päivittää tiedoston PhotoCaptureState.json, jolloin PhotoCapture.py alkaa kuvaamaan.
        /// </summary>
        /// <returns></returns>
        public async Task ReceiveC2dAsync()
        {
            Console.WriteLine("\nReceiving messages from IoT Hub to Raspberry");

            var cogUtils = new CognitiveUtil(_configuration);
            var dirUtils = new DirectoryUtil();
            DateTime timeOfLastAnalysis = DateTime.Now;
            while (true)
            {
                //Console.WriteLine("test 1");
                if (DateTime.Now - timeOfLastAnalysis > _timeBetweenPhotoAnalysis && dirUtils.ExistsPhotosInFolder())
                {
                    dirUtils.DeleteAnalysedPhotos();
                    timeOfLastAnalysis = DateTime.Now;
                    var photos = dirUtils.GetAllPhotos();
                    await cogUtils.DetectFacesExtract(photos);
                }

                //Console.WriteLine("test 2");
                Message receivedMessage = await _deviceClient.ReceiveAsync(_timeBetweenMqttMessages);
                if (receivedMessage == null)
                {
                    continue;
                }

                //Console.WriteLine("test 3");
                Console.ForegroundColor = ConsoleColor.Yellow;
                string message = Encoding.ASCII.GetString(receivedMessage.GetBytes());
                Console.WriteLine($"Received message: {message}");
                Console.ResetColor();

                try
                {
                    //Console.WriteLine("test 4");
                    var data = JsonConvert.DeserializeObject<PhotoCaptureState>(message);
                    if (data != null)
                    {
                        if (data.IsCapturing == true || data.IsCapturing == false)
                        {
                            await UpdatePhotoCaptureState(data);
                        }
                    }
                }
                catch (JsonReaderException)
                {
                    Console.WriteLine("Message not JSON.");
                }
                catch (JsonSerializationException)
                {
                    Console.WriteLine("Could not serialise json to PhotoCaptureState.");
                }

                await _deviceClient.CompleteAsync(receivedMessage);
            }
        }
    }
}
