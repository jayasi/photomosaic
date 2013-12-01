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
using Emgu.CV.Util;
using System.IO;


namespace WindowsFormsApplication4
{
    public partial class Form1 : Form
    {
        Image<Bgr, Byte> var;
        Image<Gray, Byte> grayvar;

        Image<Gray, float> fvar;
        Image<Gray, float> var1;
        Image<Bgr, Byte> Mosaic; 
        Image<Gray, Byte> GMosaic;
        int size =10;    
        double[] brightness10; 
        Color[] colors10;
        double[] brightness5;
        double[] sDev5;
        double[] sDev10;
        Color[] colors5; 
        int total =0;
        int ftotal;
        string pathString10;
        string pathString5;
        string path;
        int Isgray = 0;
        int single = 0;
        int Mixedflag = 0;
        public Form1()
        {
            InitializeComponent();
        }

        //Selection of folders. 
        private void button1_Click(object sender, EventArgs e)
        {
            int newheight = size;    //Height and width to which we have to resize. We need to decide how we are finalizing these values.
            int newwidth = size;
            Bitmap bit, resized10;    //bit is the original image and resized would be the resized one
            Bitmap resized5;    //bit is the original image and resized would be the resized one
            DialogResult result = folderBrowserDialog1.ShowDialog();    //Just like filedialog
            int count = 1;      //Counting number of images that are there in the folder
            List<string> ImageExtensions = new List<string> { ".JPG", ".JPE", ".BMP", ".GIF", ".PNG",".TIF" }; //List to check whether the file is an image or not

            if (result == DialogResult.OK)
            {
                string[] files = Directory.GetFiles(folderBrowserDialog1.SelectedPath);     //Stores the names of files in the selected folder
                ftotal = files.Length;
                ftotal++;
                progressBar1.Maximum = ftotal; //The maximum value of the progress bar is the total number of images that are in the folder. 
                pathString10 = System.IO.Path.Combine(folderBrowserDialog1.SelectedPath, "Mosaic10");    //Creating a folder within the given folder by name of Mosaic
                pathString5 = System.IO.Path.Combine(folderBrowserDialog1.SelectedPath, "Mosaic5"); 
                System.IO.Directory.CreateDirectory(pathString10);
                System.IO.Directory.CreateDirectory(pathString5);
                progressBar1.Visible = true;
                progressBar1.Value = 0;
                button1.Enabled = false; //Disable buttons while the images are getting chota. 
                button2.Enabled = false;
               // total;
                brightness10 = new double[ftotal];
                colors10 = new Color[ftotal];
                brightness5 = new double[ftotal];
                colors5 = new Color[ftotal];
                sDev10 = new double[ftotal];
                sDev5 = new double[ftotal];
                foreach (string fileName in files)  //Picking up each file in the folder
                {
                    if (ImageExtensions.Contains(Path.GetExtension(fileName).ToUpperInvariant()))   //If the extension is one of those mentioned above
                    {
                        total++;
                        bit = new Bitmap(fileName, true);   //Open file
                        resized10 = new Bitmap(bit, newwidth, newheight); //Resize file
                        resized5 = new Bitmap(bit, newwidth/2, newheight/2); //Resize file
                        //Convert to grayscale.
                        if (Isgray == 1)
                        {
                            for (int i = 0; i < resized10.Width; i++)
                            {
                                for (int x = 0; x < resized10.Height; x++)
                                {
                                    Color oc = resized10.GetPixel(i, x);
                                    Color oc2 = resized10.GetPixel(i, x); 
                                    int grayScale = (int)((oc.R * 0.3) + (oc.G * 0.59) + (oc.B * 0.11));
                                    Color nc = Color.FromArgb(oc.A, grayScale, grayScale, grayScale);
                                    resized10.SetPixel(i, x, nc);
                                }
                            }

                            for (int i = 0; i < resized5.Width; i++)
                            {
                                for (int x = 0; x < resized5.Height; x++)
                                {
                                    Color oc = resized5.GetPixel(i, x);
                                    Color oc2 = resized5.GetPixel(i, x);
                                    int grayScale = (int)((oc.R * 0.3) + (oc.G * 0.59) + (oc.B * 0.11));
                                    Color nc = Color.FromArgb(oc.A, grayScale, grayScale, grayScale);
                                    resized5.SetPixel(i, x, nc);
                                }
                            }

                        }
                        string name = pathString10 + "\\" + count + ".jpeg"; //Save file in the folder mosaic number wise 
                        resized10.Save(name, System.Drawing.Imaging.ImageFormat.Jpeg);
                        name = pathString5 + "\\" + count + ".jpeg";
                        resized5.Save(name, System.Drawing.Imaging.ImageFormat.Jpeg);
                        brightness10[count] = avgBrightness(resized10); //Storing the average brightness corresponding to the image number in the folder. 
                        colors10[count] = getDominantColor(resized10);
                        brightness5[count] = avgBrightness(resized5); //Storing the average brightness corresponding to the image number in the folder. 
                        sDev10[count] = standardDeviation(brightness10[count], resized10);
                        sDev5[count] = standardDeviation(brightness5[count], resized5);
                        colors5[count] = getDominantColor(resized5);
                        count++;
                    }
                    progressBar1.Value += 1;

                
                }
                total = total + 2;
            //    progressBar1.Dispose(); //Say bye bye to progress bar.
                button2.Enabled = true; //Re enable the button to select main image. 
                progressBar1.Visible = false;   //Make the progress bar invisible after completion. 

           }
        }

        private void button2_Click(object sender, EventArgs e)
        {

           
            DialogResult selectresult = openFileDialog1.ShowDialog(); // Show the dialog.
          

            if (selectresult == DialogResult.OK) // Test result.
            {
                path = openFileDialog1.FileName;
                var = new Image<Bgr, Byte>(path);
                grayvar = new Image<Gray, Byte>(path);
          
                // grayvar = var.Convert<Gray, Byte>();
               /* if (Isgray == 0)
                    imageBox1.Image = var;
                if (Isgray == 1 || single == 1)
                    imageBox1.Image = grayvar; */
              //  progressBar1.Maximum = (grayvar.Width * grayvar.Height) / 100;

            //    imageBox1.Show();
                //Console.Out.Write("The height : " + grayvar.Height);
                //button9.Enabled = true;
                //button8.Enabled = true;
                //button7.Enabled = true;
                button5.Enabled = true;
                
            }
        }

        //To find the avg brightness of the small images and the individual grids. 
        double avgBrightness(Bitmap x)
        {
            double result =0;
            for (int i = 0; i < x.Height; i++)
                for (int j = 0; j < x.Width; j++)
                   result += x.GetPixel(j, i).GetBrightness();

            return result / ((x.Height * x.Width));
        }

        double standardDeviation(double brightness, Bitmap x)
        {
             double result =0;
            for (int i = 0; i < x.Height; i++)
                for (int j = 0; j < x.Width; j++)
                    result += ((brightness - x.GetPixel(j, i).GetBrightness()) * (brightness - x.GetPixel(j, i).GetBrightness()));

            return Math.Pow(result / ((x.Height * x.Width)), 0.5);
        }

        Color getDominantColor(Bitmap bmp)
        {

            //Used for tally
            int r = 0;
            int g = 0;
            int b = 0;

            int total = 0;

            for (int x = 0; x < bmp.Width; x++)
            {
                for (int y = 0; y < bmp.Height; y++)
                {
                    Color clr = bmp.GetPixel(x, y);

                    r += clr.R;
                    g += clr.G;
                    b += clr.B;

                    total++;
                }
            }

            //Calculate average
            r /= total;
            g /= total;
            b /= total;

            return Color.FromArgb(r, g, b);
        }

        //NOT USING THIS. MADE IT PEHLE. NOW SHIFTED TO BITMAP. BUT JUST IN CASE :P
        private double avgIntensity(Image<Gray, Byte> var)
        {
            double result=0;
            for(int i= 0; i<var.Width;i++)
                for(int j=0; j<var.Height;j++)
                    result += var[j,i].Intensity ;
            result = result/(var.Height + var.Width);
            return result; 
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Isgray = 1;
            button1.Enabled = true;
           // button2.Enabled = true;
            button4.Enabled = false;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            button1.Enabled = true;
            //button2.Enabled = true;
            button3.Enabled = false;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (Isgray == 0)
            {
                CvInvoke.cvSmooth(Mosaic, var, Emgu.CV.CvEnum.SMOOTH_TYPE.CV_GAUSSIAN, 9, 9, 9, 9);
                imageBox3.Image = var;
                imageBox3.Show();
            }

            else
            {
                fvar = GMosaic.Convert<Gray, float>();
                Image<Gray, float> fin = new Image<Gray, float>(GMosaic.Width, GMosaic.Height);
                fin = GMosaic.Convert<Gray, float>();

                CvInvoke.cvSmooth(fvar, fin, Emgu.CV.CvEnum.SMOOTH_TYPE.CV_GAUSSIAN, 9, 9, 9, 9);
                imageBox3.Image = fin;
                imageBox3.Show();
            }
            
       }

        private void button6_Click(object sender, EventArgs e)
        {

            single = 1;
            //button1.Enabled = true;
            button2.Enabled = true;
            button6.Enabled = false;

        }

        private void button7_Click(object sender, EventArgs e)
        {
            size = 10;
           
        }
        private void button8_Click(object sender, EventArgs e)
        {
            size = 5;
            
        }

        private void button9_Click(object sender, EventArgs e)
        {
 
        }
        private void button10_Click(object sender, EventArgs e)
        {
            Mixedflag = 0;
            if (radioButton1.Checked)
            {
                size = 5;
            }
            else if (radioButton2.Checked)
            {
                size = 10;
            }
            else if (radioButton3.Checked)
            {
                size = 10;
                Mixedflag = 1;
            }
            var = new Image<Bgr, byte>(path);
            if (Mixedflag==0)
            {
                int hrsize = (var.Height) % (size);
                int wrsize = (var.Width) % (size);
                Bitmap re = new Bitmap(var.ToBitmap(), var.Width - wrsize, var.Height - hrsize);
                var = new Image<Bgr,Byte>(re.Width,re.Height);
                var = new Image<Bgr, Byte>(re);
                grayvar = new Image<Gray, Byte>(re.Width, re.Height);
                grayvar = new Image<Gray, Byte>(re);
         
                imageBox1.Image = var;
                imageBox1.Show();

                progressBar1.Maximum = (grayvar.Height * grayvar.Width) / (size * size);

                progressBar1.Value = 0;
                progressBar1.Visible = true;
                Mosaic = new Image<Bgr, Byte>(var.Size); 
                GMosaic = new Image<Gray, Byte>(grayvar.Size);
            
                if (Isgray == 0)
                {
                    Mosaic = new Image<Bgr, Byte>(var.Size);
                    var.CopyTo(Mosaic);
                }
                else
                {
                    GMosaic = new Image<Gray, Byte>(grayvar.Size);
                    grayvar.CopyTo(GMosaic);
                }

                if (single == 1)
                {
                    Image<Bgr, Byte> tochange = new Image<Bgr, Byte>(path);
                    Bitmap bit = new Bitmap(path, true);   //Open file
                    Bitmap resized = new Bitmap(bit, size, size); //Resize file
                
                    imageBox1.Image = tochange;
                    tochange = new Image<Bgr,Byte>(resized);

                    Mosaic = new Image<Bgr, Byte>(var.Size);
                    var.CopyTo(Mosaic);

                    Image<Gray, Byte>[] channels = tochange.Split();
                    for (int i = 0; i < (grayvar.Height ); i += size)
                        for (int j = 0; j < grayvar.Width; j += size)
                        {
                            progressBar1.Value++;

                            float amin=1;
                            float max=1;

                            Rectangle roi = var.ROI;
                            var.ROI = new Rectangle(j, i, size, size);
                            //Sets the region of interest of the image to the abpve by which the image is treated just as that rectangle and nothing else 
                            Bitmap smallbox = var.ToBitmap();
                            Image<Bgr, float> ismallbox = new Image<Bgr, float>(smallbox);
                            channels = tochange.Split();
           
                            for (int k = 0; k < channels.Length; k++)
                            {
                                max = 0;
                                amin = 300;
                                for (int a = 0; a < ismallbox.Height; a++)
                                {
                                    for (int b = 0; b < ismallbox.Width; b++)
                                    {
                                        if ((int)ismallbox.Data[a, b, k] > max)
                                        {
                                            max = (int)ismallbox.Data[a, b, k];
                                        }
                                        if ((int)ismallbox.Data[a, b, k] < amin)
                                        {
                                            amin = (int)ismallbox.Data[a, b, k];
                                        }
                                    }
                                }
                                CvInvoke.cvNormalize(channels[k], channels[k], amin, max, Emgu.CV.CvEnum.NORM_TYPE.CV_MINMAX, IntPtr.Zero);

                            }
                            Image<Bgr, Byte> n = new Image<Bgr, byte>(channels);
                            resized = new Bitmap(n.Bitmap, size, size); //Resize file
                        
                            var.ROI = roi;
                            {
                                Image<Bgr, Byte> temp = new Image<Bgr, byte>(resized);
                                temp.CopyTo(Mosaic.GetSubRect(new Rectangle(j, i, size, size)));

                            }
                        
                        }
         
                   imageBox2.Image = Mosaic;
                    imageBox2.Show();

                }
            //Single nonmixed done.
                else
                {
                    progressBar1.Value = 0;
                    double min = 10000;
                    int index = 10000;

                    for (int i = 0; i < (grayvar.Height); i += size)
                        for (int j = 0; j < grayvar.Width; j += size)
                        {
                            Rectangle roi = var.ROI;
                            var.ROI = new Rectangle(j, i, size, size);
                            //Sets the region of interest of the image to the abpve by which the image is treated just as that rectangle and nothing else 
                            Bitmap smallbox = var.ToBitmap();

                            double brightnesshere = avgBrightness(smallbox); //this is storing the avg brightness of the small grid 
                            double sDev = standardDeviation(brightnesshere, smallbox);
                            Color here = getDominantColor(smallbox);
                            min = 100000;
                            progressBar1.Value++;


                            for (int k = 0; k < total - 2; k++)
                            {
                                if (Isgray == 1)
                                {


                                    if (brightnesshere - brightness10[k + 1] < 0)
                                    {
                                        if (brightness10[k + 1] - brightnesshere < min)
                                        {
                                            min = brightness10[k + 1] - brightnesshere;
                                            index = k + 1;
                                        }
                                    }
                                    else if (brightnesshere - brightness10[k + 1] < min)
                                    {
                                        min = brightnesshere - brightness10[k + 1];
                                        index = k + 1;

                                    }
                                }
                                else
                                {
                                    if ((Math.Pow((here.R - colors5[k + 1].R) * 0.3, 2) + Math.Pow((here.G - colors5[k + 1].G) * 0.59, 2) + Math.Pow((here.B - colors5[k + 1].B) * 0.11, 2)) < min)
                                    {
                                        min = Math.Pow((here.R - colors10[k + 1].R) * 0.3, 2) + Math.Pow((here.G - colors10[k + 1].G) * 0.59, 2) + Math.Pow((here.B - colors10[k + 1].B) * 0.11, 2);
                                        index = k + 1;
                                    }
                                }
                            }


                            // Console.Out.Write("Standard Deviation" + sDev + "....");
                        // progressBar1.Value++;
                         Bitmap heresmallbox = var.ToBitmap();
                        Image<Bgr, float> ismallbox = new Image<Bgr, float>(heresmallbox);
                        var.ROI = roi;
              
                        if (Isgray == 0)
                        {
                            float amin = 1;
                          //  string n = "pathString" + size;
                            Image<Bgr,Byte> temp;

                            if(size == 10)
                            temp = new Image<Bgr, Byte>(pathString10 + "\\" + index.ToString() + ".jpeg");
                            else
                            temp = new Image<Bgr, Byte>(pathString5 + "\\" + index.ToString() + ".jpeg");
                            
                            Image<Bgr, Byte> tochange = temp;
                            //  Bitmap heresmallbox = var.ToBitmap();
                            //Image<Bgr, float> ismallbox = new Image<Bgr, float>(heresmallbox);


                            Image<Gray, Byte>[] channels = tochange.Split();

                            channels = tochange.Split();

                            for (int k = 0; k < channels.Length; k++)
                            {
                                int max = 0;
                                amin = 300;
                                for (int a = 0; a < ismallbox.Height; a++)
                                {
                                    for (int b = 0; b < ismallbox.Width; b++)
                                    {
                                        if ((int)ismallbox.Data[a, b, k] > max)
                                        {
                                            max = (int)ismallbox.Data[a, b, k];
                                        }
                                        if ((int)ismallbox.Data[a, b, k] < amin)
                                        {
                                            amin = (int)ismallbox.Data[a, b, k];
                                        }
                                    }
                                }
                                CvInvoke.cvNormalize(channels[k], channels[k], amin, max, Emgu.CV.CvEnum.NORM_TYPE.CV_MINMAX, IntPtr.Zero);

                            }
                            temp = new Image<Bgr, byte>(channels);
                            temp.CopyTo(Mosaic.GetSubRect(new Rectangle(j, i, size, size)));
                        }
                        else
                        {
                            Image<Gray, Byte> temp;
                            if (size == 10)
                                temp = new Image<Gray, Byte>(pathString10 + "\\" + index.ToString() + ".jpeg");
                            else
                                temp = new Image<Gray, Byte>(pathString5 + "\\" + index.ToString() + ".jpeg");
                            
                            temp.CopyTo(GMosaic.GetSubRect(new Rectangle(j, i, size, size)));

                        }

                    }
            }
                progressBar1.Value = 0;
                progressBar1.Visible = false;
                ///progressBar1.Dispose();
                if (Isgray == 0)
                {
                    imageBox2.Image = Mosaic;
                    imageBox2.Show();
                    CvInvoke.cvShowImage("Hi", Mosaic);
                }
                else
                {
                    imageBox2.Image = GMosaic;
                    imageBox2.Show();
                    CvInvoke.cvShowImage("Hi", GMosaic);
                }
            }
        

        else if (Mixedflag == 1)
        {
            size = 10;
            double min = 0;
            int index = 0;
            int hrsize = (var.Height) % (size);
            int wrsize = (var.Width) % (size);
            Bitmap re = new Bitmap(var.ToBitmap(), var.Width - wrsize, var.Height - hrsize);
            var = new Image<Bgr, Byte>(re.Width, re.Height);
            var = new Image<Bgr, Byte>(re);
            grayvar = new Image<Gray, Byte>(re.Width, re.Height);
            grayvar = new Image<Gray, Byte>(re);

            progressBar1.Maximum = (grayvar.Height * grayvar.Width) / (size * size);

            Mosaic = new Image<Bgr, Byte>(grayvar.Size); ;
            GMosaic = new Image<Gray, Byte>(grayvar.Size);
            progressBar1.Value = 0;
            progressBar1.Visible = true;
            
            if (Isgray == 0)
            {
                Mosaic = new Image<Bgr, Byte>(grayvar.Size);
                var.CopyTo(Mosaic);
            }
            else
            {
                GMosaic = new Image<Gray, Byte>(grayvar.Size);
                grayvar.CopyTo(GMosaic);
            }

            if (single == 1)
            {

                Image<Gray, Byte> tochange = new Image<Gray, Byte>(path);
                Bitmap bit = new Bitmap(path, true);   //Open file
                Bitmap resized = new Bitmap(bit, size, size); //Resize file

                GMosaic = new Image<Gray, Byte>(grayvar.Size);
                grayvar.CopyTo(GMosaic);

                double tochangebright = avgBrightness(resized);

                for (int i = 0; i < (grayvar.Height); i += size)
                    for (int j = 0; j < grayvar.Width; j += size)
                    {
                        progressBar1.Value++;

                        float amin;
                        float max;
                        max = 0;
                        amin = 300;
                        Rectangle roi = var.ROI;
                        var.ROI = new Rectangle(j, i, size, size);
                        //Sets the region of interest of the image to the abpve by which the image is treated just as that rectangle and nothing else 
                        Bitmap smallbox = var.ToBitmap();
                        Image<Gray, float> ismallbox = new Image<Gray, float>(smallbox);

                        for (int a = 0; a < ismallbox.Height; a++)
                        {
                            for (int b = 0; b < ismallbox.Width; b++)
                            {
                                if ((int)ismallbox.Data[a, b, 0] > max)
                                {
                                    max = (int)ismallbox.Data[a, b, 0];
                                }
                                if ((int)ismallbox.Data[a, b, 0] < amin)
                                {
                                    amin = (int)ismallbox.Data[a, b, 0];
                                }
                            }
                        }

              
                        bit = tochange.Bitmap;
                        resized = new Bitmap(bit, size, size); //Resize file

                        IntPtr final = CvInvoke.cvCreateImage(resized.Size, Emgu.CV.CvEnum.IPL_DEPTH.IPL_DEPTH_32F, 2);
                        Image<Gray, float> atochange = new Image<Gray, float>(resized);
                        CvInvoke.cvSetZero(final);  // In6itialize all elements to Zero
                        CvInvoke.cvSetImageCOI(final, 1);    //Value 1 means that the first channel is selected! 
                        CvInvoke.cvCopy(atochange, final, IntPtr.Zero);    //Copying the value of the image selected to the first channel. 
                        CvInvoke.cvSetImageCOI(final, 0);    //Value 0 means that all channels are selected!
                        CvInvoke.cvNormalize(atochange, atochange, amin, max, Emgu.CV.CvEnum.NORM_TYPE.CV_MINMAX, IntPtr.Zero);


                        var.ROI = roi;
                        {
                            Image<Gray, Byte> temp = atochange.Convert<Gray, Byte>();
                            temp.CopyTo(GMosaic.GetSubRect(new Rectangle(j, i, size, size)));

                        }
                    }
                imageBox2.Image = GMosaic;
                imageBox2.Show();
            }

            else
            {

                for (int i = 0; i < (grayvar.Height); i += size)
                    for (int j = 0; j < grayvar.Width ; j += size)
                    {
                        Rectangle roi = var.ROI;
                        var.ROI = new Rectangle(j, i, size, size);
                        //Sets the region of interest of the image to the abpve by which the image is treated just as that rectangle and nothing else 
                        Bitmap smallbox = var.ToBitmap();

                        double brightnesshere = avgBrightness(smallbox); //this is storing the avg brightness of the small grid 
                        double sDev = standardDeviation(brightnesshere, smallbox);
                        Color here = getDominantColor(smallbox);
                        min = 100000;

                        if (sDev > 0.02)
                        {
                            for (int k = i; k <= i + (size / 2); k += (size / 2))
                                for (int l = j; l <= j + (size / 2); l += (size / 2))
                                {
                                    Rectangle roi5 = var.ROI;
                                    var.ROI = new Rectangle(l, k, size / 2, size / 2);
                                    Bitmap smallersmallbox = var.Bitmap;
                                    double sbright = avgBrightness(smallersmallbox);
                                    min = 100000;
                                    for (int m = 0; m < total - 2; m++)

                                        if (Isgray == 1)
                                        {

                                            if (sbright - brightness5[m + 1] < 0)
                                            {
                                                if (brightness5[m + 1] - sbright < min)
                                                {
                                                    min = brightness5[m + 1] - sbright;
                                                    index = m + 1;
                                                }
                                            }
                                            else if (sbright - brightness5[m + 1] < min)
                                            {
                                                min = sbright - brightness5[m + 1];
                                                index = m + 1;

                                            }
                                        }
         
                                    var.ROI = roi;
                                    if (Isgray == 0)
                                    {
                                        Image<Bgr, Byte> temp;
                                        temp = new Image<Bgr, Byte>(pathString5 + "\\" + index.ToString() + ".jpeg");
                           
                                        temp.CopyTo(Mosaic.GetSubRect(new Rectangle(l, k, size / 2, size / 2)));
                                    }
                                    else
                                    {
                                        Image<Gray, Byte> temp;
                                            temp = new Image<Gray, Byte>(pathString5 + "\\" + index.ToString() + ".jpeg");
                           
                                        temp.CopyTo(GMosaic.GetSubRect(new Rectangle(l, k, size / 2, size / 2)));
                                    }

                                }

   
                        }
                        else
                        {
                            for (int k = 0; k < total - 2; k++)
                            {
                                if (Isgray == 1)
                                {


                                    if (brightnesshere - brightness10[k + 1] < 0)
                                    {
                                        if (brightness10[k + 1] - brightnesshere < min)
                                        {
                                            min = brightness10[k + 1] - brightnesshere;
                                            index = k + 1;
                                        }
                                    }
                                    else if (brightnesshere - brightness10[k + 1] < min)
                                    {
                                        min = brightnesshere - brightness10[k + 1];
                                        index = k + 1;

                                    }
                                }
                                else
                                {
                                    if ((Math.Pow((here.R - colors10[k + 1].R) * 0.3, 2) + Math.Pow((here.G - colors10[k + 1].G) * 0.59, 2) + Math.Pow((here.B - colors10[k + 1].B) * 0.11, 2)) < min)
                                    {
                                        min = Math.Pow((here.R - colors10[k + 1].R) * 0.3, 2) + Math.Pow((here.G - colors10[k + 1].G) * 0.59, 2) + Math.Pow((here.B - colors10[k + 1].B) * 0.11, 2);
                                        index = k + 1;
                                    }
                                }
                            }

                             progressBar1.Value++;

                            var.ROI = roi;
         
                            if (Isgray == 0)
                            {
                                Image<Bgr, Byte> temp;
                                if (size == 10)
                                    temp = new Image<Bgr, Byte>(pathString10 + "\\" + index.ToString() + ".jpeg");
                                else
                                    temp = new Image<Bgr, Byte>(pathString5 + "\\" + index.ToString() + ".jpeg");
                           
                                temp.CopyTo(Mosaic.GetSubRect(new Rectangle(j, i, size, size)));
                            }
                            else
                            {
                                Image<Gray, Byte> temp;
                                if (size == 10)
                                    temp = new Image<Gray, Byte>(pathString10 + "\\" + index.ToString() + ".jpeg");
                                else
                                    temp = new Image<Gray, Byte>(pathString5 + "\\" + index.ToString() + ".jpeg");
                           
                                temp.CopyTo(GMosaic.GetSubRect(new Rectangle(j, i, size, size)));

                            }
                        }

                     
                    }
                progressBar1.Value = 0;
                progressBar1.Visible = false;
                ///progressBar1.Dispose();
                if (Isgray == 0)
                {
                    imageBox2.Image = Mosaic;
                    imageBox2.Show();
                }
                else
                {
                    imageBox2.Image = GMosaic;
                    imageBox2.Show();
                }
            }
        }

    }



        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
        }
       
        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {

        }
        }
    }

