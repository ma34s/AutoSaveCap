using System.Linq;
using System.Drawing;

namespace AutoSaveCap
{
    class BitmapUtil
    {
        private static byte[] getBmpHash(Bitmap img1)
        {
            //ImageConverterで比較
            ImageConverter ic = new ImageConverter();
            byte[] byte1 = (byte[])ic.ConvertTo(img1, typeof(byte[]));
            System.Security.Cryptography.MD5CryptoServiceProvider md5 =
                new System.Security.Cryptography.MD5CryptoServiceProvider();
            return md5.ComputeHash(byte1);
        }

        public static bool Compare(Bitmap img1, Bitmap img2)
        {
            //高さが違えばfalse
            if (img1.Width != img2.Width || img1.Height != img2.Height) return false;

            byte[] hash1 = getBmpHash(img1);
            byte[] hash2 = getBmpHash(img2);
            //ハッシュを比較
            return hash1.SequenceEqual(hash2);
        }
    }
}

