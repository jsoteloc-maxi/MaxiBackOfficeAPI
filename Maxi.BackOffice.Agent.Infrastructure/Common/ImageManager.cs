using System.Drawing.Imaging;
using System.Runtime.Versioning;

namespace Maxi.BackOffice.Agent.Infrastructure.Common
{
    public class ImageManager
    {
        private string _path;
        public ImageManager(string path)
        {
            _path = path;
        }

        private bool ValidateName(string guid, string name)
        {
            var arrayName = name.Split('_');
            if (arrayName.Length > 2)
            {
                if (arrayName[2] == guid)
                    return true;
                else
                    return false;
            }
            return false;
        }
        public string GenerateName(Guid idGuid, int id, bool isfront = true)
        {
            string string1 = DateTime.Now.ToString("yyyyMMdd");
            string string2 = id.ToString();
            //quitar los guiones del guid

            string string3 = GuidRemove(idGuid);
            string string4 = isfront ? "F" : "R";
            string result = string1 + "_" + string2 + "_" + string3 + "_" + string4;
            result += ".tif";
            return result;
        }

        [SupportedOSPlatform("windows")]
        public void SaveImage(int id, byte[] ImageF, byte[] ImageR, Guid idGuid)
        {
            //generar los nombres
            string nameF = GenerateName(idGuid, id, true);
            string nameR = GenerateName(idGuid, id, false);

            //obtener path del parametro
            string path = _path + id.ToString() + "\\";

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            System.Drawing.Imaging.Encoder myEncoder;
            myEncoder = System.Drawing.Imaging.Encoder.Compression;
            EncoderParameters myEncoderParameters;
            myEncoderParameters = new EncoderParameters(1);
            EncoderParameter myEncoderParameter;
            myEncoderParameter = new EncoderParameter(myEncoder, (long)EncoderValue.CompressionCCITT4);
            myEncoderParameters.Param[0] = myEncoderParameter;

            var stream = new MemoryStream(ImageF);
            System.Drawing.Image image = System.Drawing.Image.FromStream(stream);

            string mimeType = "image/tiff";
            ImageCodecInfo imageCodec = ImageCodecInfo.GetImageEncoders().First(x => x.MimeType == mimeType);

            image.Save(path + nameF, imageCodec, myEncoderParameters);

            stream = new MemoryStream(ImageR);
            image = System.Drawing.Image.FromStream(stream);
            image.Save(path + nameR, imageCodec, myEncoderParameters);

            //File.WriteAllBytes(path + nameF, ImageF);
            //File.WriteAllBytes(path + nameR, ImageR);
        }

        public void DeleteById(int id)
        {
            string path = _path + id.ToString();

            if (Directory.Exists(path))
            {
                Directory.Delete(path, true);
            }
        }

        public List<string> MoveImage(int id, Guid idGuid, string path, string directory)
        {
            List<string> result = new List<string>();
            string name;
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            //buscar carpeta de batch
            if (Directory.Exists(_path + id.ToString()))
            {
                var files = new DirectoryInfo(_path + id.ToString());
                foreach (var item in files.GetFiles())
                {
                    if (ValidateName(GuidRemove(idGuid), item.Name))
                    {
                        name = Guid.NewGuid().ToString();
                        File.Copy(item.FullName, path + name + ".tif");
                        if (IsFront(item.Name) == 1)
                            result.Add(name + "_1");
                        else
                            result.Add(name + "_2");

                    }
                }
            }
            return result;
        }

        private int IsFront(string name)
        {
            var arrayName = name.Split('_');
            if (arrayName.Length > 2)
            {
                arrayName = arrayName[3].Split('.');
                if (arrayName[0] == "F")
                {
                    return 1;
                }
                else
                {
                    return 2;
                }
            }
            return 0;
        }

        private string GuidRemove(Guid idGuid)
        {
            var arrayGuid = idGuid.ToString().ToCharArray();
            var arrayGuidClean = arrayGuid.Where(x => x != '-').ToArray();
            return new string(arrayGuidClean);
        }
    }
}
