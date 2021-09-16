using Microsoft.Azure.CognitiveServices.Vision.Face;
using Microsoft.Azure.CognitiveServices.Vision.Face.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.SecurityTokenService;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PiRecordingControls
{
    public class CognitiveUtil
    {
        private IConfiguration _configuration;
        private string _subscriptionKey;
        private string _endPoint;
        private string _recognitionModel4 = RecognitionModel.Recognition04;
        private string _deviceId;

        public CognitiveUtil(IConfiguration configuration)
        {
            _configuration = configuration;
            _subscriptionKey = configuration["CognitiveServices:SubscriptionKey"];
            _endPoint = configuration["CognitiveServices:EndPoint"];
            _deviceId = configuration["Device:DeviceId"];
        }

        /// <summary>
        /// Palauttaa autentikoidun FaceClient objektin, jolla käytetään Azure Cognitive Services -palveluita.
        /// </summary>
        /// <param name="endpoint"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static IFaceClient Authenticate(string endpoint, string key)
        {
            return new FaceClient(new ApiKeyServiceClientCredentials(key)) { Endpoint = endpoint };
        }

        /// <summary>
        /// Lähettää kasvokuvan Azuren Cognitive Services rajapintaan, josta se saa Emotion-objektin vastauksena.
        /// Tämä objekti serialisoidaan ja lähetetään myöhempää tilastollista analyysiä varten projektin rajapintaan.
        /// </summary>
        /// <param name="photoPath"></param>
        /// <returns></returns>
        public async Task DetectFaceExtract(string photoPath)
        {
            IFaceClient client = Authenticate(_endPoint, _subscriptionKey);
            Stream image = File.OpenRead(photoPath);

            IList<DetectedFace> detectedFaces;
            DetectedFace face = null;
            try
            {
                detectedFaces = await client.Face.DetectWithStreamAsync(
                    image,
                    returnFaceAttributes: new List<FaceAttributeType> { FaceAttributeType.Age, FaceAttributeType.Emotion, FaceAttributeType.Gender, },
                    detectionModel: DetectionModel.Detection01,
                    recognitionModel: _recognitionModel4
                );
                face = detectedFaces.FirstOrDefault();
            }
            catch (BadRequestException ex)
            {
                Console.WriteLine("Error in sending data to Cognitive Services. Reason: " + ex);
            }
            catch (APIErrorException ex)
            {
                Console.WriteLine("Error in sending data to Cognitive Services. Reason: " + ex);
            }
            if (face == null)
            {
                Console.WriteLine("No face detected!");
            }
            else
            {
                Emotion emotion = face.FaceAttributes.Emotion;
                var data = new ApiData(photoPath, emotion, _deviceId);
                var apiUtil = new ApiUtil(_configuration);
                var response = await apiUtil.SendDataToAPI(data);
                Console.WriteLine(response);
            }
        }

        /// <summary>
        /// Ajaa metodin DetectFaceExtract kaikille matriisin photoPaths sisältämille kuville.
        /// </summary>
        /// <param name="photoPaths"></param>
        /// <returns></returns>
        public async Task DetectFacesExtract(string[] photoPaths)
        {
            foreach (var path in photoPaths)
            {
                await DetectFaceExtract(path);
            }
        }
    }
}
