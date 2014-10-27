using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;


namespace _3DtextureConverter
{
    /// <summary>
    ///  Raw to PNG converter by Leandro Barbagallo (lebarba)
    /// </summary>
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            String path = textBox1.Text;


            int width = int.Parse( widthTextBox.Text);
            int height = int.Parse(heightTextBox.Text); ;
            int depth = int.Parse(depthTextBox.Text); ;

            //To convert raw files larger than 256x256x256 you can tweak these values.
            const int maxTilesWidth = 16;
            const int maxTilesHeight = 16;

            int newWidth = width * maxTilesWidth;
            int newHeight = height * maxTilesHeight;


            progressBar1.Maximum = width * height * depth;

            using (Bitmap b = new Bitmap(newWidth, newHeight))
            {
                using (Graphics g = Graphics.FromImage(b))
                {

                    try
                    {
                        
                        using (BinaryReader breader = new BinaryReader(File.Open(path, FileMode.Open)))
                        {

                            bool flipY = flipYcheckBox.Checked;

                            int length = (int)breader.BaseStream.Length;

                            int realPosX, realPosY;

                            for (int z = 0; z < depth; z++)
                            {
                                for (int y = 0; y < height; y++)
                                {
                                    for (int x = 0; x < width; x++)
                                    {
                                        int value = breader.ReadByte();

                                        realPosX = x + (z % maxTilesHeight) * width;

                                        int yPos;
                                        if (flipY)
                                        {
                                            yPos = height - y - 1;
                                        }
                                        else
                                        {
                                            yPos = y;
                                        }

                                        realPosY = yPos + ((int)(z / maxTilesWidth)) * height;
                                        b.SetPixel(realPosX, realPosY, Color.FromArgb(value, value, value, value));

                                    }

                                }
                                progressBar1.Value = z * width * height;
                                progressBar1.Update();
                            }

                        }

                        //TODO: Encode only A8.
                        b.Save(path + ".png", ImageFormat.Png);

                        progressBar1.Value = depth * width * height;
                        progressBar1.Update();

                        MessageBox.Show("Done converting " + path + ".png");

                    
                    }
                    catch (Exception)
                    {

                        MessageBox.Show("error reading content. Check that file exists and that the dimensions are correct.");
                    }

                    progressBar1.Value = 0;
                    progressBar1.Update();
                }

                
            }






        }

        private void button2_Click(object sender, EventArgs e)
        {
             OpenFileDialog openFileDialog1 = new OpenFileDialog();

            openFileDialog1.InitialDirectory = textBox1.Text ;
            openFileDialog1.Filter = "raw files (*.raw)|*.raw|All files (*.*)|*.*" ;
            openFileDialog1.FilterIndex = 1 ;
            openFileDialog1.RestoreDirectory = true ;

            if(openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                textBox1.Text = openFileDialog1.FileName;
                button1.Enabled = true;
            }

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
           
        }

        private void depthTextBox_Leave(object sender, EventArgs e)
        {
            int val;

            try
            {
                val = int.Parse(depthTextBox.Text);
                if (val <= 0)
                    throw new Exception();

                if (val > 256)
                    throw new Exception();
            }
            catch (Exception)
            {

                MessageBox.Show("Invalid depth.");
                depthTextBox.Text = "256";

            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            textBox1.Text = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        }


    }
}
