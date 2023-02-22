using System;
using System.Windows.Forms;

namespace AlkoCalc
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            if (args.Length < 1)
                Application.Run(new GUI());
            else if (args.Length == 1)
                Application.Run(new GUI(args[0]));
        }
    }
}
