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
            opf.Filter = "Binfont file|*.binfont|FNT file|*.fnt";
            if(opf.ShowDialog() == DialogResult.OK)
            {
                if (!string.IsNullOrEmpty(opf.FileName))
                {
                    MainEvent(opf.FileName);
                }
            }
        }

        public static byte[] Dec(byte[] data)
        {
            var rd = new BinaryReader(new MemoryStream(data));
            var ms = new MemoryStream();
            var wt = new BinaryWriter(ms);
            for (int i = 0; i < rd.BaseStream.Length / 4; i++)
            {
                byte[] argb = rd.ReadBytes(4);
                if(argb[0] == argb[1])
                {
                    wt.Write(argb[0]);
                    wt.Write(argb[0]);
                    wt.Write(argb[0]);
                    wt.Write((byte)0xFF);
                }
                else if (argb[0] != argb[1] && argb[0] == argb[2])
                {
                    wt.Write(argb[0]);
                    wt.Write(argb[0]);
                    wt.Write(argb[0]);
                    wt.Write((byte)0xFF);
                }
                else if (argb[0] != argb[1] && argb[1] == argb[2])
                {
                    wt.Write(argb[1]);
                    wt.Write(argb[1]);
                    wt.Write(argb[1]);
                    wt.Write((byte)0xFF);
                }
            }
            return ms.ToArray();
        }

        public static void MainEvent(string path)
        {
            string pth = Path.GetDirectoryName(path);
            string filename = Path.GetFileNameWithoutExtension(path);
            if (Path.GetExtension(path) == ".binfont")
            {
                try
                {
                    byte[] hexData = {
                            0x44, 0x44, 0x53, 0x20, 0x7C, 0x00, 0x00, 0x00, 0x0F, 0x10, 0x02, 0x00 };
                    byte[] nextData = {
                            0x01, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00,
                            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x20, 0x00, 0x00, 0x00,
                            0x41, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x20, 0x00, 0x00, 0x00, 0x00, 0x00, 0xFF, 0x00,
                            0x00, 0xFF, 0x00, 0x00, 0xFF, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xFF, 0x00, 0x10, 0x00, 0x00,
                            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
                        };

                    var rd = new BinaryReader(File.OpenRead(path));
                    int width = 4096;
                    int magic = rd.ReadInt32();
                    int ver = rd.ReadInt32();
                    int texs = rd.ReadInt32();
                    byte[] height = rd.ReadBytes(texs * 4);
                    Console.WriteLine("Version {0} , Textures {1}", ver, texs);
                    var ms = new BinaryReader(new MemoryStream(height));
                    for (int i = 0; i < texs; i++)
                    {
                        int h = ms.ReadInt32();
                        int Size = width * h * 4;
                        byte[] outbuf = Dec(rd.ReadBytes(Size));
                        Console.WriteLine("Dump file {0}", pth + "\\" + filename + "_tex_" + i.ToString() + ".dds");
                        var wt = new BinaryWriter(File.Create(pth + "\\" + filename + "_tex_" + i.ToString() + ".dds"));
                        wt.Write(hexData);
                        wt.Write(h);
                        wt.Write(width);
                        wt.Write(Size);
                        wt.Write(nextData);
                        wt.Write(outbuf);
                        wt.Flush();
                        wt.Close();
                        Console.WriteLine("Convert Done!");
                    }
                    Console.WriteLine("Dump fnt {0}", pth + "\\" + filename + "_tex_.fnt");
                    int count = rd.ReadInt32();
                    StreamWriter fntwt = new StreamWriter(File.Create(pth + "\\" + filename + "_tex_.fnt"), Encoding.UTF8);
                    for (int i = 0; i < count; i++)
                    {
                        float left = rd.ReadSingle();
                        float right = rd.ReadSingle();
                        float top = rd.ReadSingle();
                        float bottom = rd.ReadSingle();
                        float wd = rd.ReadSingle();
                        float offset_left = rd.ReadSingle();
                        float offset_top = rd.ReadSingle();
                        ushort id = rd.ReadUInt16();
                        byte page = rd.ReadByte();
                        byte char_left = rd.ReadByte();
                        fntwt.WriteLine("char=" + Convert.ToChar(id).ToString() + " id=" + id.ToString() + " left=" + left.ToString() + " right=" + right.ToString() + " top=" + top.ToString() + " bottom=" + bottom.ToString() + " width=" + wd.ToString() + " off_left=" + offset_left.ToString() + " off_top=" + offset_top.ToString() + " page=" + page.ToString() + " char_left=" + char_left.ToString());
                    }
                    float next_adv = rd.ReadSingle();
                    float next_adv_a = rd.ReadSingle();
                    float next_adv_k = rd.ReadSingle();
                    fntwt.WriteLine("next_adv=" + next_adv.ToString() + " next_adv_a=" + next_adv.ToString() + " next_adv_k=" + next_adv.ToString());
                    fntwt.Close();
                    Console.WriteLine("Done!");
                }
                catch
                {
                    MessageBox.Show("File format isn't support!");
                    Console.WriteLine("Wrong format file!");
                }
            }
            else if(Path.GetExtension(path) == ".fnt")
            {
                try
                {
                    var ms = new MemoryStream();
                    var mswt = new BinaryWriter(ms);
                    string[] lines = File.ReadAllLines(path);
                    foreach (string line in lines)
                    {
                        mswt.Write(lines.Length - 1);
                        var data = line.Split(' ');
                        
                    }
                }
                catch
                {
                    MessageBox.Show("File format isn't support!");
                    Console.WriteLine("Wrong format file!");
                }
            }
            else
            {
                MessageBox.Show("File format isn't support!");
                Console.WriteLine("Wrong format file!");
            }
        }
    }
}
