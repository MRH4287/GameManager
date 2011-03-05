using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using System.IO;
namespace Game
{
    public partial class Help : Form
    {
        public Help()
        {
            InitializeComponent();
        }

        int ow, oh;
        public int k = 0;
        public string passPhrase;


        private void pictureBox1_DoubleClick(object sender, EventArgs e)
        {
          
                if ((!pictureBox3.Visible)&&(k == 5))
                {
                    try
                    {
                        byte[] data = global::Game.Properties.Resources.Control;

                        Encryption.AesEncryptedInformation aei = new Encryption.AesEncryptedInformation(passPhrase);
                        Stream DataStream = new System.IO.MemoryStream(data);
                        DataStream.Position = 0;

                        Encryption.AesEncryptedInformation.DataPackage package = aei.readInformation(DataStream);

                        Stream EncryptionStream = new MemoryStream(package.Decrypted);
                        EncryptionStream.Position = 0;

                        ow = Width;
                        oh = Height;
                       
                        pictureBox3.Image =new Bitmap(EncryptionStream);
                        EncryptionStream.Close();
                        DataStream.Close();
                        
                        pictureBox3.Width = pictureBox3.Image.Width;
                        pictureBox3.Height = pictureBox3.Image.Height;
                        pictureBox3.Visible = true;
                        Opacity = 1;
                        checkBox1.Checked = false;
                        webBrowser1.Dock = DockStyle.None;
                    }
                    catch
                    { Text += "."; }
                }
                else if(pictureBox3.Image != null)
                {
                    
                    Height = oh;
                    Width = ow;
                    pictureBox3.Image.Dispose();
                    pictureBox3.Image = null;
                    pictureBox3.Visible = false;
                    Opacity = 0.8;
                    checkBox1.Checked = true;
                    webBrowser1.Dock = DockStyle.Bottom;
                }
            
            k = 0;
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }


        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                Opacity = 0.8;
            }
            else
            {
                Opacity = 1;
            }
        }

        private void Help_Load(object sender, EventArgs e)
        {
            webBrowser1.DocumentStream = File.OpenRead("./help.htm");


        }

        private void Help_Resize(object sender, EventArgs e)
        {
            webBrowser1.Height = ClientSize.Height - 250;
        }
     
    }
}
