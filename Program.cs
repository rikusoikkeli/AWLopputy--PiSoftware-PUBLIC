using Microsoft.Azure.Devices.Client;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace PiRecordingControls
{
    public class Program
    {
        private static IConfigurationRoot configuration;

        public static async Task<int> Main(string[] args)
        {
            // Luetaan konfiguraatio IConfigurationRoot-objektiin.
            var builder = new ConfigurationBuilder().AddJsonFile("appsettings.json", optional: false, reloadOnChange: false);
            configuration = builder.Build();

            // Luodaan PhotoCaptureState.json-tiedosto jollain oletusarvolla riippuen Debugging-asetuksista.
            bool manuallySetIsCapturingTo;
            var readSuccess = bool.TryParse(configuration["Debugging:ManuallySetIsCapturingTo"], out manuallySetIsCapturingTo);
            if (readSuccess)
                SetUpFiles.CreatePhotoCaptureStateJson(manuallySetIsCapturingTo);
            else
                SetUpFiles.CreatePhotoCaptureStateJson(false);

            // Luodaan Photos-kansio ja RunPyScript.sh. Lisäksi ajetaan jälkimmäinen riippuen Debugging-asetuksista.
            SetUpFiles.CreatePhotosFolder();
            SetUpFiles.CreateShellScript();
            Thread.Sleep(1000);
            if (bool.Parse(configuration["Debugging:RunPhotoCapture"]) == true)
                ShellHelper.Bash("RunPyScript.sh");

            // Luodaan DeviceClient-objekti, joka vastaanottaa MQTT-viestejä Azure IoTHubista.
            using var deviceClient = DeviceClient.CreateFromConnectionString(
                configuration["Device:PrimaryConnectionString"],
                TransportType.Mqtt);
            var sample = new MqttUtil(deviceClient, configuration);
            await sample.ReceiveC2dAsync();
            await deviceClient.CloseAsync();

            return 0;
        }
    }
}
