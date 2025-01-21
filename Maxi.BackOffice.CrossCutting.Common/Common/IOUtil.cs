using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maxi.BackOffice.CrossCutting.Common.Common
{
    public static class IOUtil
    {
        public static byte[] GetFileBytes(string aFile)
        {
            return System.IO.File.ReadAllBytes(aFile);

            //var s = new FileStream(aFile, FileMode.Open, FileAccess.Read);
            //var buf = new byte[s.Length];
            //var numBytesToRead = (int)s.Length;
            //var numBytesRead = 0;
            //while (numBytesToRead > 0)
            //{
            //    var n = s.Read(buf, numBytesRead, numBytesToRead);
            //    numBytesRead += n;
            //    numBytesToRead -= n;
            //}

            //s.Close();
            //return buf;
        }

        public static void DeleteFile(string aFile)
        {
            System.IO.File.Delete(aFile);
        }

        public static string AppBaseDirForce(string folder)
        {
            var root = $@"{AppDomain.CurrentDomain.BaseDirectory}\{folder}\";
            if (!Directory.Exists(root))
                Directory.CreateDirectory(root);
            return root;
        }
    }
}
