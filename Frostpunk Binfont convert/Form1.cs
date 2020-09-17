using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Frostpunk_Binfont_convert
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        OpenFileDialog opf = new OpenFileDialog();

        private void btnConvert_Click(object sender, EventArgs e)
        {
            //opf.Filter = "Binfont file|*.binfont|FNT file|*.fnt";
            if(opf.ShowDialog() == DialogResult.OK)
            {
                if (!string.IsNullOrEmpty(opf.FileName))
                {
                    var rd = new BinaryReader(File.OpenRead(opf.FileName));
                    var wt = new BinaryWriter(File.Create("out.dat"));
                    for(int i = 0; i < rd.BaseStream.Length / 4; i++)
                    {
                        byte[] argb = rd.ReadBytes(4);
                        /*if (argb[0] == 0xFF && argb[1] == 0xFF && argb[2] == 0xFF)
                        {
                            wt.Write((byte)0x00);
                            wt.Write((byte)0x00);
                            wt.Write((byte)0x00);
                            wt.Write((byte)0xFF);
                        }
                        else*/ if (argb[0] == argb[1] && argb[0] != 0xFF)
                        {
                            wt.Write(argb[0]);
                            wt.Write(argb[0]);
                            wt.Write(argb[0]);
                            wt.Write((byte)0xFF);
                        }  
                        else if(argb[0] != argb[1] && argb[0] == argb[2])
                        {
                            wt.Write(argb[0]);
                            wt.Write(argb[0]);
                            wt.Write(argb[0]);
                            wt.Write((byte)0xFF);
                        }
                        else if(argb[0] != argb[1] && argb[1] == argb[2])
                        {
                            wt.Write(argb[1]);
                            wt.Write(argb[1]);
                            wt.Write(argb[1]);
                            wt.Write((byte)0xFF);
                        }
                    }
                    rd.Close();
                    wt.Close();
                }
            }
        }
    }
}
