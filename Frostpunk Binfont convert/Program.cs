﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Frostpunk_Binfont_convert;

namespace Frostpunk_Binfont_convert
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            if(args.Length == 0)
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new Form1());
            }
            else
            {
                Frostpunk_Binfont_convert.Form1.MainEvent(args[0]);
                Console.WriteLine("Press any button...");
                Console.ReadKey();
            }
        }
    }
}
