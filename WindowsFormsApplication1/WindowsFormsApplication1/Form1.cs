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
        IntPtr midImage;
        Matrix<float> dft;
        Matrix<float> shift_real;
        Matrix<float> real;
        Matrix<float> im;
        Matrix<float> fft;
        Matrix<float> final;
        Image<Bgr, Byte> Mosaic; 
        Image<Gray, Byte> GMosaic;
        int size = 10;    
        double[] brightness; 
        Color[] colors; 
        int total;
        string pathString;
        int Isgray = 0;
        int single = 0;

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int newheight = size;    //Height and width to which we have to resize. We need to decide how we are finalizing these values.
            int newwidth = size;
            Bitmap bit, resized;    //bit is the original image and resized would be the resized one
            DialogResult result = folderBrowserDialog1.ShowDialog();    //Just like filedialog
            int count = 1;      //Counting number of images that are there in the folder
            List<string> ImageExtensions = new List<string> { ".JPG", ".JPE", ".BMP", ".GIF", ".PNG",".TIF" }; //List to check whether the file is an image or not

            if (result == DialogResult.OK)
            {
                string[] files = Directory.GetFiles(folderBrowserDialog1.SelectedPath);     //Stores the names of files in the selected folder
                total = files.Length;
                progressBar1.Maximum = total; //The maximum value of the progress bar is the total number of images that are in the folder. 
                pathString = System.IO.Path.Combine(folderBrowserDialog1.SelectedPath, "Mosaic");    //Creating a folder within the given folder by name of Mosaic
                System.IO.Directory.CreateDirectory(pathString);
                progressBar1.Visible = true;
                button1.Enabled = false; //Disable buttons while the images are getting chota. 
                button2.Enabled = false;
               // double[] brightness = new double[total] ;
                total++;
                brightness = new double[total];
                colors = new Color[total];
                foreach (string fileName in files)  //Picking up each file in the folder
                {
                    if (ImageExtensions.Contains(Path.GetExtension(fileName).ToUpperInvariant()))   //If the extension is one of those mentioned above
                    {
                        bit = new Bitmap(fileName, true);   //Open file
                        resized = new Bitmap(bit, newwidth, newheight); //Resize file
                        //Convert to grayscale.
                        if (Isgray == 1)
                        {
                            for (int i = 0; i < resized.Width; i++)
                            {
                                for (int x = 0; x < resized.Height; x++)
                                {
                                    Color oc = resized.GetPixel(i, x);
                                    Color oc2 = resized.GetPixel(i, x); 
                                    //Console.Out.Write("Colour...." + oc.);
                                    int grayScale = (int)((oc.R * 0.3) + (oc.G * 0.59) + (oc.B * 0.11));
                                    Color nc = Color.FromArgb(oc.A, grayScale, grayScale, grayScale);
                                    resized.SetPixel(i, x, nc);
                                }
                            }
                        }
                        string name = pathString + "\\" + count + ".jpeg"; //Save file in the folder mosaic number wise 
                        resized.Save(name, System.Drawing.Imaging.ImageFormat.Jpeg);
                        brightness[count] = avgBrightness(resized); //Storing the average brightness corresponding to the image number in the folder. 
                        colors[count] = getDominantColor(resized);
                        //Console.Out.Write(colors[count] + "   ");
                        count++;
                        progressBar1.Value += 1;
                    }
                
                }

                progressBar1.Visible = false;   //Make the progress bar invisible after completion. 
            //    progressBar1.Dispose(); //Say bye bye to progress bar.
                button2.Enabled = true; //Re enable the button to select main image. 

           }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            
            double min;
            int index = 1; 
            DialogResult selectresult = openFileDialog1.ShowDialog(); // Show the dialog.
            progressBar1.Value = 0;
            progressBar1.Visible = true;

            if (selectresult == DialogResult.OK) // Test result.
            {
                String path = openFileDialog1.FileName;
                var = new Image<Bgr, Byte>(path);
                grayvar = var.Convert<Gray, Byte>();
                if (Isgray == 0)
                    imageBox1.Image = var;
                if (Isgray == 1 || single == 1)
                    imageBox1.Image = grayvar;
                progressBar1.Maximum = (grayvar.Width * grayvar.Height) / 100;

                imageBox1.Show();
                Console.Out.Write("The height : " + grayvar.Height);

                Mosaic = new Image<Bgr, Byte>(grayvar.Size); ;
                GMosaic = new Image<Gray, Byte>(grayvar.Size); ;

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
        
                    System.Console.WriteLine(tochangebright);

                    for (int i = 0; i < (grayvar.Height - size); i += size)
                        for (int j = 0; j < grayvar.Width - size; j += size)
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
                            Image<Gray, float> ismallbox = new Image<Gray,float> (smallbox);

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

                            System.Console.WriteLine(max);
                            System.Console.WriteLine(amin);

                            bit = tochange.Bitmap;
                            resized = new Bitmap(bit, size, size); //Resize file
                            
                            System.Console.WriteLine("Here");
                    
                            IntPtr final = CvInvoke.cvCreateImage(resized.Size, Emgu.CV.CvEnum.IPL_DEPTH.IPL_DEPTH_32F, 2); 
                            System.Console.WriteLine("Here1");
                    
                            Image<Gray, float> atochange = new Image<Gray, float>(resized);

                            System.Console.WriteLine("Here2");

                            CvInvoke.cvSetZero(final);  // In6itialize all elements to Zero

                            System.Console.WriteLine("Here3");


                            CvInvoke.cvSetImageCOI(final, 1);    //Value 1 means that the first channel is selected! 
                            System.Console.WriteLine("Here4");

                            CvInvoke.cvCopy(atochange, final, IntPtr.Zero);    //Copying the value of the image selected to the first channel. 
                            System.Console.WriteLine("Here5");

                            CvInvoke.cvSetImageCOI(final, 0);    //Value 0 means that all channels are selected!

                            System.Console.WriteLine("Here3");
                    
                            CvInvoke.cvNormalize(atochange, atochange, amin, max, Emgu.CV.CvEnum.NORM_TYPE.CV_MINMAX, IntPtr.Zero);
                        
                
                            var.ROI = roi;
                            {
                                Image<Gray, Byte> temp = atochange.Convert<Gray,Byte>();
                                temp.CopyTo(GMosaic.GetSubRect(new Rectangle(j, i, size, size)));

                            } 
                        }
                    imageBox2.Image = GMosaic;
                    imageBox2.Show();
                }
                //Image<Gray, Byte> Mosaic2 = new Image<Gray, Byte>(20,20);



                    //Processing each 10x10 grid and finding their brightness as well. 
                else
                {

                    for (int i = 0; i < (grayvar.Height - size); i += size)
                        for (int j = 0; j < grayvar.Width - size; j += size)
                        {
                            Rectangle roi = var.ROI;
                            var.ROI = new Rectangle(j, i, size, size);
                            //Sets the region of interest of the image to the abpve by which the image is treated just as that rectangle and nothing else 
                            Bitmap smallbox = var.ToBitmap();

                            double brightnesshere = avgBrightness(smallbox); //this is storing the avg brightness of the small grid 
                            Color here = getDominantColor(smallbox);
                            min = 100000;

                            for (int k = 0; k < total - 2; k++)
                            {
                                //Console.Out.Write("Difference is:" + Math.Abs(brightnesshere - brightness[k + 1]) + "at index" + k + " min: " + min);
                                if (Isgray == 1)
                                {
                                    if (brightnesshere - brightness[k + 1] < 0)
                                    {
                                        if (brightness[k + 1] - brightnesshere < min)
                                        {
                                            min = brightness[k + 1] - brightnesshere;
                                            index = k + 1;
                                        }
                                    }
                                    else if (brightnesshere - brightness[k + 1] < min)
                                    {
                                        min = brightnesshere - brightness[k + 1];
                                        index = k + 1;

                                    }
                                }
                                else
                                {
                                    if ((Math.Pow((here.R - colors[k + 1].R) * 0.3, 2) + Math.Pow((here.G - colors[k + 1].G) * 0.59, 2) + Math.Pow((here.B - colors[k + 1].B) * 0.11, 2)) < min)
                                    {
                                        min = Math.Pow((here.R - colors[k + 1].R) * 0.3, 2) + Math.Pow((here.G - colors[k + 1].G) * 0.59, 2) + Math.Pow((here.B - colors[k + 1].B) * 0.11, 2);
                                        index = k + 1;
                                    }
                                }
                            }

                            Console.Out.Write("Index " + index + "....");
                            progressBar1.Value++;

                            var.ROI = roi;
                            //index = 9;
                            if (index >= 220)
                                index = 219;
                            //Image<Gray, Byte> temp = new Image<Gray, Byte>(pathString + "\\" + index.ToString() + ".jpeg");
                            if (Isgray == 0)
                            {
                                Image<Bgr, Byte> temp = new Image<Bgr, Byte>(pathString + "\\" + index.ToString() + ".jpeg");
                                temp.CopyTo(Mosaic.GetSubRect(new Rectangle(j, i, size, size)));
                            }
                            else
                            {
                                Image<Gray, Byte> temp = new Image<Gray, Byte>(pathString + "\\" + index.ToString() + ".jpeg");
                                temp.CopyTo(GMosaic.GetSubRect(new Rectangle(j, i, size, size)));

                            }
                        }

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

        //To find the avg brightness of the small images and the individual grids. 
        double avgBrightness(Bitmap x)
        {
            double result =0;
            for (int i = 0; i < x.Height; i++)
                for (int j = 0; j < x.Width; j++)
                   result += x.GetPixel(j, i).GetBrightness();

            return result / ((x.Height * x.Width));
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
            button2.Enabled = true;
            button4.Enabled = false;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            button1.Enabled = true;
            button2.Enabled = true;
            button3.Enabled = false;
        }

        void cvShiftDFT(Matrix<float> source, Matrix<float> destination)
        {
            Matrix<float> des1, des2, des3, quad1, quad2, quad3, quad4, des4 = new Matrix<float>(destination.Width, destination.Height, 2);


            des1 = destination.GetSubRect(new Rectangle(0, 0, destination.Width / 2, destination.Height / 2));
            des2 = destination.GetSubRect(new Rectangle(destination.Width / 2, 0, destination.Width / 2, destination.Height / 2));
            des3 = destination.GetSubRect(new Rectangle(destination.Width / 2, destination.Height / 2, destination.Width / 2, destination.Height / 2));
            des4 = destination.GetSubRect(new Rectangle(0, destination.Height / 2, destination.Width / 2, destination.Height / 2));
            quad1 = source.GetSubRect(new Rectangle(0, 0, destination.Width / 2, destination.Height / 2));
            quad2 = source.GetSubRect(new Rectangle(destination.Width / 2, 0, destination.Width / 2, destination.Height / 2));
            quad3 = source.GetSubRect(new Rectangle(destination.Width / 2, destination.Height / 2, destination.Width / 2, destination.Height / 2));
            quad4 = source.GetSubRect(new Rectangle(0, destination.Height / 2, destination.Width / 2, destination.Height / 2));


            quad2.GetSubRect(new Rectangle(Point.Empty, quad2.Size)).CopyTo(des4);
            quad1.GetSubRect(new Rectangle(Point.Empty, quad1.Size)).CopyTo(des3);
            quad3.GetSubRect(new Rectangle(Point.Empty, quad3.Size)).CopyTo(des1);
            quad4.GetSubRect(new Rectangle(Point.Empty, quad4.Size)).CopyTo(des2);


        }

        private void button5_Click(object sender, EventArgs e)
        {

            fvar = GMosaic.Convert<Gray, float>();
            var1 = var.Convert<Gray, float>();
         //   var1 = CvInvoke.
            //var1 = new Image<Gray, float>(pathString);
            midImage = CvInvoke.cvCreateImage(var.Size, Emgu.CV.CvEnum.IPL_DEPTH.IPL_DEPTH_32F, 2);
           
            dft = new Matrix<float>(var.Rows, var.Cols, 2);   //2D Matrix of 2 channels which will store the value of the fourier transform 
            shift_real = new Matrix<float>(dft.Size);
            real = new Matrix<float>(var.Size);//The matrix which shall contain the real part of the the fourier transform contained in dft
            im = new Matrix<float>(var.Size);//The matrix which shall contain the imaginary part of the fourier transform contained in dft 
            fft = new Matrix<float>(var.Rows, var.Cols, 2); //The matrix which will store the H(u,v), the filter 
            final = new Matrix<float>(var.Rows, var.Cols, 2);

            CvInvoke.cvSetZero(midImage);  // Initialize all elements to Zero
            CvInvoke.cvSetImageCOI(midImage, 1);    //Value 1 means that the first channel is selected! 
            CvInvoke.cvCopy(fvar, midImage, IntPtr.Zero);    //Copying the value of the image selected to the first channel. 
            CvInvoke.cvSetImageCOI(midImage, 0);    //Value 0 means that all channels are selected!

            CvInvoke.cvDFT(midImage, dft, Emgu.CV.CvEnum.CV_DXT.CV_DXT_FORWARD, -1); //Performing fourier transform on the image and storing its result in dft. 

            shift_real = new Matrix<float>(dft.Size);
            final = new Matrix<float>(var.Rows, var.Cols, 2);
            double dx = 80;
            for (int i = 0; i < dft.Rows; i++)
                for (int j = 0; j < dft.Cols; j++)
                {
                    double d = Math.Sqrt((i - (dft.Rows) / 2) * (i - (dft.Rows) / 2) + (j - (dft.Cols) / 2) * (j - (dft.Cols) / 2));
                 //   double dx = double.Parse(textBox1.Text);
                    double mid1 = 2 * dx * dx;
                    double mid2 = -(d * d) / mid1;
                    float h = (float)Math.Pow(2.71828, mid2);

                    fft[i, j] = h;

                }
            Matrix<float> fft1 = new Matrix<float>(var.Rows, var.Cols, 2);
            cvShiftDFT(fft, fft1);  //Fix the positioning. 
            fft = fft1;

            CvInvoke.cvMulSpectrums(dft, fft, final, Emgu.CV.CvEnum.MUL_SPECTRUMS_TYPE.DEFAULT);
            CvInvoke.cvDFT(final, var1, Emgu.CV.CvEnum.CV_DXT.CV_DXT_INVERSE, -1);
           
           imageBox3.Image = var1;
            imageBox3.Show();
         
       }

        private void button6_Click(object sender, EventArgs e)
        {

            single = 1;
            button1.Enabled = true;
            button2.Enabled = true;
            button6.Enabled = false;

        }

    }
}
