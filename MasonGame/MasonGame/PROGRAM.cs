using System;
using System.Windows.Forms;

namespace MasonGame
{
    internal static class PROGRAM
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new GAME());
        }
    }
}
