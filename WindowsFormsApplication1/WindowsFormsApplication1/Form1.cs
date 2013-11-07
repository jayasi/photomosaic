using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Emgu.CV;
using Emgu.Util;
using Emgu.CV.Structure;
using System.IO;


namespace WindowsFormsApplication4
{
    public partial class Form1 : Form
    {
        Image<Bgr, Byte> var;
        Image<Gray, Byte> grayvar;

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int newheight = 100;    //Height and width to which we have to resize. We need to decide how we are finalizing these values.
            int newwidth = 100;
            Bitmap bit, resized;    //bit is the original image and resized would be the resized one
            DialogResult result = folderBrowserDialog1.ShowDialog();    //Just like filedialog
            int count = 1;      //Counting number of images that are there in the folder
            List<string> ImageExtensions = new List<string> { ".JPG", ".JPE", ".BMP", ".GIF", ".PNG",".TIF" }; //List to check whether the file is an image or not

            if (result == DialogResult.OK)
            {
                string[] files = Directory.GetFiles(folderBrowserDialog1.SelectedPath);     //Stores the names of files in the selected folder
                int total = files.Length;
                progressBar1.Maximum = total; //The maximum value of the progress bar is the total number of images that are in the folder. 
                string pathString = System.IO.Path.Combine(folderBrowserDialog1.SelectedPath, "Mosaic");    //Creating a folder within the given folder by name of Mosaic
                System.IO.Directory.CreateDirectory(pathString);
                progressBar1.Visible = true;
                button1.Enabled = false; //Disable buttons while the images are getting chota. 
                button2.Enabled = false;
                double[] brightness = new double[total] ;
                foreach (string fileName in files)  //Picking up each file in the folder
                {
                    if (ImageExtensions.Contains(Path.GetExtension(fileName).ToUpperInvariant()))   //If the extension is one of those mentioned above
                    {
                        bit = new Bitmap(fileName, true);   //Open file
                        resized = new Bitmap(bit, newwidth, newheight); //Resize file
                        //Convert to grayscale.
                        for (int i = 0; i < resized.Width; i++)
                        {
                            for (int x = 0; x < resized.Height; x++)
                            {
                                Color oc = resized.GetPixel(i, x);
                                int grayScale = (int)((oc.R * 0.3) + (oc.G * 0.59) + (oc.B * 0.11));
                                Color nc = Color.FromArgb(oc.A, grayScale, grayScale, grayScale);
                                resized.SetPixel(i, x, nc);
                            }
                        }
                        string name = pathString + "\\" + count + ".jpeg"; //Save file in the folder mosaic number wise 
                        resized.Save(name, System.Drawing.Imaging.ImageFormat.Jpeg);
                        brightness[count] = avgBrightness(resized); //Storing the average brightness corresponding to the image number in the folder. 
                        Console.Out.Write(brightness[count] + "   ");
                        count++;
                            progressBar1.Value += 1;
                    }
                
                }

                progressBar1.Visible = false;   //Make the progress bar invisible after completion. 
                progressBar1.Dispose(); //Say bye bye to progress bar.
                button2.Enabled = true; //Re enable the button to select main image. 
               

           }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            DialogResult selectresult = openFileDialog1.ShowDialog(); // Show the dialog.
            if (selectresult == DialogResult.OK) // Test result.
            {
                String path = openFileDialog1.FileName;
                var= new Image<Bgr, Byte>(path);
                grayvar = var.Convert<Gray, Byte>();
                imageBox1.Image = grayvar;
                imageBox1.Show();
            }

        }


        double avgBrightness(Bitmap x)
        {
            double result =0;
            for (int i = 0; i < x.Height; i++)
                for (int j = 0; j < x.Width; j++)
                   result += x.GetPixel(j, i).GetBrightness();

            return result / (x.Height + x.Width);
        }

      /*  private double avgIntensity(Image<Gray, Byte> var)
        {
            double result=0;
            for(int i= 0; i<var.Width;i++)
                for(int j=0; j<var.Height;j++)
                    result += var[i,j].Intensity ;
            result = result/(var.Height + var.Width);
            return result; 
        }*/

    }
}
