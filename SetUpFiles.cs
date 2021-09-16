using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PiRecordingControls
{
    public static class SetUpFiles
    {
        /// <summary>
        /// Luo Shell skriptin juureen, joka käynnistää PhotoCapture.py.
        /// </summary>
        public static void CreateShellScript()
        {
            var file = File.Create("RunPyScript.sh");
            file.Close();
            File.WriteAllLines("RunPyScript.sh", new List<string> { "python3 PhotoCapture.py" });
        }

        /// <summary>
        /// Luo Photos-kansion juureen, johon PhotoCapture.py tallentaa ottamansa kuvat.
        /// </summary>
        public static void CreatePhotosFolder()
        {
            bool photosFolderExists = Directory.Exists("./Photos");
            if (!photosFolderExists)
            {
                Directory.CreateDirectory("./Photos");
            }
        }

        /// <summary>
        /// Luo PhotoCaptureState.json-tiedoston juureen, jonka perusteella PhotoCapture.py tietää, ottaako kuvia vai ei.
        /// </summary>
        /// <param name="defaultState"></param>
        public static void CreatePhotoCaptureStateJson(bool defaultState)
        {
            var file = File.Create("PhotoCaptureState.json");
            file.Close();
            if (defaultState)
            {
                File.WriteAllLines("PhotoCaptureState.json", new List<string> { "{\"IsCapturing\":true}" });
            }
            else
            {
                File.WriteAllLines("PhotoCaptureState.json", new List<string> { "{\"IsCapturing\":false}" });
            }
        }
    }
}
