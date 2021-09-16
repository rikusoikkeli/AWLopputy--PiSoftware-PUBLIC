using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PiRecordingControls
{
    public class DirectoryUtil
    {
        public string photosDirPath = @"./Photos/";
        public string[] photos;

        /// <summary>
        /// Hakee kuvat kansiosta photosDirPath.
        /// </summary>
        /// <returns>Palauttaa string-matriisin polkuja.</returns>
        public string[] GetAllPhotos()
        {
            var tempPhotos = Directory.GetFiles(photosDirPath);
            photos = tempPhotos;
            return photos;
        }

        /// <summary>
        /// Poistaa jo analysoidut kuvat levyltä.
        /// </summary>
        public void DeleteAnalysedPhotos()
        {
            if (photos != null)
            {
                foreach (var photo in photos)
                {
                    File.Delete(photo);
                }
                photos = null;
            }
        }

        /// <summary>
        /// Tarkistaa, sisältääkö photosDirPath kuvia (vai onko se tyhjä).
        /// </summary>
        /// <returns>Palauttaa booleanin.</returns>
        public bool ExistsPhotosInFolder()
        {
            return Directory.GetFiles(photosDirPath).Length > 0;
        }
    }
}
