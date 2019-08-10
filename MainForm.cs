using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
//using System.Threading.Tasks;
using System.Windows.Forms;
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.ML;
using Emgu.CV.ML.Structure;
using Emgu.CV.UI;
using Emgu.Util;
using System.Diagnostics;
using Emgu.CV.CvEnum;
using System.IO;
using System.IO.Ports;
using tesseract;
using System.Collections;
using System.Threading;
using System.Media;
using System.Runtime.InteropServices;
using Emgu.CV.Util;


namespace Auto_parking
{
    public partial class MainForm : Form
    {
        Image<Bgr, byte> imgInput;

        
        public MainForm()
        {
            InitializeComponent();
        }


        #region định nghĩa
        List<Image<Bgr, byte>> PlateImagesList = new List<Image<Bgr, byte>>();
        Image Plate_Draw;
        List<string> PlateTextList = new List<string>();
        List<Rectangle> listRect = new List<Rectangle>();
        PictureBox[] box = new PictureBox[12];

        public TesseractProcessor full_tesseract = null;
        public TesseractProcessor ch_tesseract = null;
        public TesseractProcessor num_tesseract = null;
        private string m_path = Application.StartupPath + @"\data\";
        private List<string> lstimages = new List<string>();
        private const string m_lang = "eng";

        //int current = 0;
        Capture capture = null;
        #endregion


       /* #region di chuyển
        bool mouseDown = false;
        Point lastLocation;
        private void button_Leave(object sender, EventArgs e)
        {
            Button bsen = (Button)sender;
            bsen.ForeColor = Color.Black;
        }

        private void button_Enter(object sender, EventArgs e)
        {
            Button bsen = (Button)sender;
            bsen.ForeColor = Color.White;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Environment.Exit(0);
            this.Close();
        }

        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            if (mouseDown == false && e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                mouseDown = true;
                lastLocation = e.Location;
            }
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                //contextMenuStrip1.Show(this.DesktopLocation.X + e.X, this.DesktopLocation.Y + e.Y);	
            }
        }

        private void panel1_MouseMove(object sender, MouseEventArgs e)
        {
            if (mouseDown)
            {
                this.SetDesktopLocation(this.DesktopLocation.X - lastLocation.X + e.X, this.DesktopLocation.Y - lastLocation.Y + e.Y);
                this.Update();
            }
        }

        private void panel1_MouseUp(object sender, MouseEventArgs e)
        {
            mouseDown = false;
        }
        private void panel1_MouseHover(object sender, EventArgs e)
        {
            panel1.BackColor = Color.FromArgb(15, 15, 15);
        }
        private void panel1_MouseLeave(object sender, EventArgs e)
        {
            panel1.BackColor = Color.FromArgb(64, 64, 64);
        }
        #endregion*/

        ImageForm IF;
       /* private void MainForm_Load(object sender, EventArgs e)
        {
            capture = new Emgu.CV.Capture();
            timer1.Enabled = true;

            IF = new ImageForm();
           
            full_tesseract = new TesseractProcessor();
            bool succeed = full_tesseract.Init(m_path, m_lang, 3);
            if (!succeed)
            {
                MessageBox.Show("Tesseract initialization failed. The application will exit.");
                Application.Exit();
            }
            full_tesseract.SetVariable("tessedit_char_whitelist", "ABCDEFHKLMNPRSTVXY1234567890").ToString();

            ch_tesseract = new TesseractProcessor();
            succeed = ch_tesseract.Init(m_path, m_lang, 3);
            if (!succeed)
            {
                MessageBox.Show("Tesseract initialization failed. The application will exit.");
                Application.Exit();
            }
            ch_tesseract.SetVariable("tessedit_char_whitelist", "ABCDEFHKLMNPRSTUVXY").ToString();

            num_tesseract = new TesseractProcessor();
            succeed = num_tesseract.Init(m_path, m_lang, 3);
            if (!succeed)
            {
                MessageBox.Show("Tesseract initialization failed. The application will exit.");
                Application.Exit();
            }
            num_tesseract.SetVariable("tessedit_char_whitelist", "1234567890").ToString();


            m_path = System.Environment.CurrentDirectory + "\\";
            string[] ports = SerialPort.GetPortNames();
            for (int i = 0; i < box.Length; i++)
            {
                box[i] = new PictureBox();
            }
        } */
        private void button5_Click(object sender, EventArgs e)
        {
            if (IF.Visible == false)
            {
                IF.Show();
            }
            else
            {
                IF.Hide();
            }
        }
        bool success = true;
        private void timer1_Tick(object sender, EventArgs e)
        {
            if (success == true)
            {
                success = false;
                new Thread(() =>
                {
                    try
                    {
                        capture.SetCaptureProperty(CAP_PROP.CV_CAP_PROP_FRAME_WIDTH, 640);
                        capture.SetCaptureProperty(CAP_PROP.CV_CAP_PROP_FRAME_HEIGHT, 480);
                        Image<Bgr, byte> cap = capture.QueryFrame();
                        if (cap != null)
                        {
                            MethodInvoker mi = delegate
                            {
                                try
                                {
                                    Bitmap bmp = cap.ToBitmap();
                                    pictureBox_WC.Image = bmp;
                                    IF.pictureBox4.Image = bmp;
                                    pictureBox_WC.Update();
                                    IF.pictureBox4.Update();
                                   
                                }
                                catch (Exception ex)
                                { }
                            };
                            if (InvokeRequired)
                                Invoke(mi);
                        }
                    }
                    catch (Exception) { }
                    success = true;
                }).Start();
                
            }
            
            
        }

        public void ProcessImage(string urlImage)
        {
            PlateImagesList.Clear();
            PlateTextList.Clear();
            FileStream fs = new FileStream(urlImage, FileMode.Open, FileAccess.Read);
            Image img = Image.FromStream(fs);
            Bitmap image = new Bitmap(img);
            pictureBox2.Image = image;
           // IF.pictureBox2.Image = image;
            fs.Close();

           // FindLicensePlate4(image, out Plate_Draw);

        }
        public static Bitmap RotateImage(Image image, float angle)
        {
            if (image == null)
                throw new ArgumentNullException("image");

            PointF offset = new PointF((float)image.Width / 2, (float)image.Height / 2);

            //create a new empty bitmap to hold rotated image
            Bitmap rotatedBmp = new Bitmap(image.Width, image.Height);
            rotatedBmp.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            //make a graphics object from the empty bitmap
            Graphics g = Graphics.FromImage(rotatedBmp);

            //Put the rotation point in the center of the image
            g.TranslateTransform(offset.X, offset.Y);

            //rotate the image
            g.RotateTransform(angle);

            //move the image back
            g.TranslateTransform(-offset.X, -offset.Y);

            //draw passed in image onto graphics object
            g.DrawImage(image, new PointF(0, 0));

            return rotatedBmp;
        }

        private string Ocr(Bitmap image_s, bool isFull, bool isNum = false)
        {
            string temp = "";
            Image<Gray, byte> src = new Image<Gray, byte>(image_s);
            double ratio = 1;
            while (true)
            {
                ratio = (double)CvInvoke.cvCountNonZero(src) / (src.Width * src.Height);
                if (ratio > 0.5) break;
                src = src.Dilate(2);
            }
            Bitmap image = src.ToBitmap();

            TesseractProcessor ocr;
            if (isFull)
                ocr = full_tesseract;
            else if (isNum)
                ocr = num_tesseract;
            else
                ocr = ch_tesseract;

            int cou = 0;
            ocr.Clear();
            ocr.ClearAdaptiveClassifier();
            temp = ocr.Apply(image);
            while (temp.Length > 3)
            {
                Image<Gray, byte> temp2 = new Image<Gray, byte>(image);
                temp2 = temp2.Erode(2);
                image = temp2.ToBitmap();
                ocr.Clear();
                ocr.ClearAdaptiveClassifier();
                temp = ocr.Apply(image);
                cou++;
                if (cou > 10)
                {
                    temp = "";
                    break;
                }
            }
            return temp;

        }

       
       

        private void button2_Click(object sender, EventArgs e)
        {
            //while (true) ;
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "Image (*.bmp; *.jpg; *.jpeg; *.png) |*.bmp; *.jpg; *.jpeg; *.png|All files (*.*)|*.*||";
            dlg.InitialDirectory = Application.StartupPath + "\\ImageTest";
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.Cancel)
            {
                return;
            }
            string startupPath = dlg.FileName;

           /* Image temp1;
            string temp2, temp3;
            //Reconize(startupPath, out temp1, out temp2, out temp3);
            pictureBox_XeVAO.Image = temp1;
            if (temp3 == "")
                text_BiensoVAO.Text = "ko nhận dạng dc biển số";
            else
                text_BiensoVAO.Text = temp3;*/
        }

     /*   private void button3_Click(object sender, EventArgs e)
        {
            if (capture != null)
            {
                timer1.Enabled = false;
                pictureBox_XeRA.Image = null;
                IF.pictureBox2.Image = null;
                capture.QueryFrame().Save("aa.bmp");
                FileStream fs = new FileStream(m_path + "aa.bmp", FileMode.Open, FileAccess.Read);
                Image temp = Image.FromStream(fs);
                fs.Close();
                pictureBox_XeRA.Image = temp;
                IF.pictureBox2.Image = temp;
                pictureBox_XeRA.Update();
                IF.pictureBox2.Update();
                Image temp1;
                string temp2, temp3;
                /*Reconize(m_path + "aa.bmp", out temp1, out temp2, out temp3);
                pictureBox_XeVAO.Image = temp1;
                if(temp3 == "")
                    text_BiensoVAO.Text = "ko nhận dạng dc biển số";
                else
                    text_BiensoVAO.Text = temp3;

                timer1.Enabled = true;
            }
            
        }*/

        private void MainForm_Load_1(object sender, EventArgs e)
        {
            capture = new Emgu.CV.Capture();
            timer2.Enabled = true;
            timer4.Start();

            IF = new ImageForm();

            full_tesseract = new TesseractProcessor();
            bool succeed = full_tesseract.Init(m_path, m_lang, 3);
            if (!succeed)
            {
                MessageBox.Show("Tesseract initialization failed. The application will exit.");
                Application.Exit();
            }
            full_tesseract.SetVariable("tessedit_char_whitelist", "ABCDEFHKLMNPRSTVXY1234567890").ToString();

            ch_tesseract = new TesseractProcessor();
            succeed = ch_tesseract.Init(m_path, m_lang, 3);
            if (!succeed)
            {
                MessageBox.Show("Tesseract initialization failed. The application will exit.");
                Application.Exit();
            }
            ch_tesseract.SetVariable("tessedit_char_whitelist", "ABCDEFHKLMNPRSTUVXY").ToString();

            num_tesseract = new TesseractProcessor();
            succeed = num_tesseract.Init(m_path, m_lang, 3);
            if (!succeed)
            {
                MessageBox.Show("Tesseract initialization failed. The application will exit.");
                Application.Exit();
            }
            num_tesseract.SetVariable("tessedit_char_whitelist", "1234567890").ToString();


            m_path = System.Environment.CurrentDirectory + "\\";
            string[] ports = SerialPort.GetPortNames();
            for (int i = 0; i < box.Length; i++)
            {
                box[i] = new PictureBox();
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog dialog = new OpenFileDialog();
                dialog.Filter = "Image (*.bmp; *.jpg; *.jpeg; *.png) |*.bmp; *.jpg; *.jpeg; *.png|All files (*.*)|*.*||";
                dialog.InitialDirectory = Application.StartupPath + "\\ImageTest";
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    imgInput = new Image<Bgr, byte>(dialog.FileName);
                    pictureBox1.Image = imgInput.Bitmap;
                }
                else
                {
                    throw new Exception("No file selected.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            try
            {
                if (imgInput == null)
                {
                    throw new Exception("Chon anh hoac chup anh !");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            Image<Bgr, byte> imgInputClone = imgInput.Clone();
            Image<Gray, byte> grayInput = imgInput.Convert<Gray, byte>().Clone();

            var faces = grayInput.DetectHaarCascade(new HaarCascade(Application.StartupPath + "\\vn2best.xml"), 1.1, 8, HAAR_DETECTION_TYPE.DO_CANNY_PRUNING, new Size(0, 0))[0];
            foreach (var face in faces)
            {
                imgInput.ROI = face.rect;

                imgInputClone.Draw(face.rect, new Bgr(Color.Blue), 2);
                pictureBox1.Image = imgInputClone.Bitmap;
                pictureBox2.Image = imgInput.Bitmap;

            }

            Image<Bgr, byte> color1 = imgInput.Resize(450, 300, Emgu.CV.CvEnum.INTER.CV_INTER_LINEAR);
            Bitmap src = color1.ToBitmap();
            Bitmap grayframe;
            
            FindContours con = new FindContours();
            Bitmap color;
            List<Rectangle> listR = new List<Rectangle>();
            int c = con.IdentifyContours(src, 50, false, out grayframe, out color, out listRect);
            pictureBox2.Image = color;
            IF.pictureBox1.Image = grayframe;
            IF.pictureBox2.Image = imgInputClone.Bitmap;

            Image<Gray, byte> dst = new Image<Gray, byte>(grayframe);
            grayframe = dst.ToBitmap();
            string zz = "";
            string sothe = "";
            // lọc và sắp xếp số
            List<Bitmap> bmp = new List<Bitmap>();
            List<int> erode = new List<int>();
            List<Rectangle> up = new List<Rectangle>();
            List<Rectangle> dow = new List<Rectangle>();
            int up_y = 0, dow_y = 0;
            bool flag_up = false;

            int di = 0;

            if (listRect == null) return;

            for (int i = 0; i < listRect.Count; i++)
            {
                Bitmap ch = grayframe.Clone(listRect[i], grayframe.PixelFormat);
                int cou = 0;
                full_tesseract.Clear();
                full_tesseract.ClearAdaptiveClassifier();
                string temp = full_tesseract.Apply(ch);
                while (temp.Length > 3)
                {
                    Image<Gray, byte> temp2 = new Image<Gray, byte>(ch);
                    temp2 = temp2.Erode(2);
                    ch = temp2.ToBitmap();
                    full_tesseract.Clear();
                    full_tesseract.ClearAdaptiveClassifier();
                    temp = full_tesseract.Apply(ch);
                    cou++;
                    if (cou > 10)
                    {
                        listRect.RemoveAt(i);
                        i--;
                        di = 0;
                        break;
                    }
                    di = cou;
                }
            }
          
            for (int i = 0; i < listRect.Count; i++)
            {
                for (int j = i; j < listRect.Count; j++)
                {
                    if (listRect[i].Y > listRect[j].Y + 100)
                    {
                        flag_up = true;
                        up_y = listRect[j].Y;
                        dow_y = listRect[i].Y;
                        break;
                    }
                    else if (listRect[j].Y > listRect[i].Y + 100)
                    {
                        flag_up = true;
                        up_y = listRect[i].Y;
                        dow_y = listRect[j].Y;
                        break;
                    }
                    if (flag_up == true) break;
                }
            }

            for (int i = 0; i < listRect.Count; i++)
            {
                if (listRect[i].Y < up_y + 50 && listRect[i].Y > up_y - 50)
                {
                    up.Add(listRect[i]);
                }
                else if (listRect[i].Y < dow_y + 50 && listRect[i].Y > dow_y - 50)
                {
                    dow.Add(listRect[i]);
                }
            }

            if (flag_up == false) dow = listRect;

            for (int i = 0; i < up.Count; i++)
            {
                for (int j = i; j < up.Count; j++)
                {
                    if (up[i].X > up[j].X)
                    {
                        Rectangle w = up[i];
                        up[i] = up[j];
                        up[j] = w;
                    }
                }
            }
            for (int i = 0; i < dow.Count; i++)
            {
                for (int j = i; j < dow.Count; j++)
                {
                    if (dow[i].X > dow[j].X)
                    {
                        Rectangle w = dow[i];
                        dow[i] = dow[j];
                        dow[j] = w;
                    }
                }
            }

            int x = 12;
            int c_x = 0;

            for (int i = 0; i < up.Count; i++)
            {
                Bitmap ch = grayframe.Clone(up[i], grayframe.PixelFormat);
                Bitmap o = ch;
                //ch = con.Erodetion(ch);
                string temp;
                if (i < 2)
                {
                    temp = Ocr(ch, false, true); // nhan dien so
                }
                else
                {
                    temp = Ocr(ch, false, false);// nhan dien chu
                }

                zz += temp; c_x++;sothe += temp;
            }

            zz += " - ";
            for (int i = 0; i < dow.Count; i++)
            {
                Bitmap ch = grayframe.Clone(dow[i], grayframe.PixelFormat);
                //ch = con.Erodetion(ch);
                string temp = Ocr(ch, false, true); // nhan dien so
                zz += temp;
            }
            sothe = dateTimePicker1.Value.Second + dateTimePicker1.Value.Hour+" - "+sothe;
            if(zz==" - ")
            {
                textBox1.Text = "khong nhan dien duoc bien so";
            }
            else textBox1.Text = zz;
            IF.textBox6.Text = zz;
            textBox3.Text = sothe;
            textBox4.Text = label18.Text +" - "+ dateTimePicker1.Text;

            }

        // doc bien 1 dong
       /* private void button9_Click_1(object sender, EventArgs e)
        {   
            Image<Bgr, byte> color1 = imgInput.Resize(450, 200, Emgu.CV.CvEnum.INTER.CV_INTER_LINEAR);
            Bitmap src = color1.ToBitmap();
            Bitmap grayframe;
            string bienso = "";
            string bienso_text = "";
            FindContours con = new FindContours();
            Bitmap color;
            List<Rectangle> listR = new List<Rectangle>();
            int c = con.IdentifyContours(src, 50, false, out grayframe, out color, out listRect);
            pictureBox1.Image = color;
            pictureBox2.Image = grayframe;

           

            Image<Gray, byte> dst = new Image<Gray, byte>(grayframe);
            grayframe = dst.ToBitmap();
            string zz = "";

            // lọc và sắp xếp số
            List<Bitmap> bmp = new List<Bitmap>();
            List<int> erode = new List<int>();
            List<Rectangle> up = new List<Rectangle>();
            List<Rectangle> dow = new List<Rectangle>();
            int up_y = 0, dow_y = 0;
            bool flag_up = false;

            int di = 0;

            if (listRect == null) return;

            for (int i = 0; i < listRect.Count; i++)
            {
                Bitmap ch = grayframe.Clone(listRect[i], grayframe.PixelFormat);
                int cou = 0;
                full_tesseract.Clear();
                full_tesseract.ClearAdaptiveClassifier();
                string temp = full_tesseract.Apply(ch);
                while (temp.Length > 3)
                {
                    Image<Gray, byte> temp2 = new Image<Gray, byte>(ch);
                    temp2 = temp2.Erode(2);
                    ch = temp2.ToBitmap();
                    full_tesseract.Clear();
                    full_tesseract.ClearAdaptiveClassifier();
                    temp = full_tesseract.Apply(ch);
                    cou++;
                    if (cou > 10)
                    {
                        listRect.RemoveAt(i);
                        i--;
                        di = 0;
                        break;
                    }
                    di = cou;
                }
            }

            for (int i = 0; i < listRect.Count; i++)
            {
                for (int j = i; j < listRect.Count; j++)
                {
                    if (listRect[i].X > listRect[j].X)
                    {
                        Rectangle w = listRect[i]; 
                        listRect[i] = listRect[j];
                        listRect[j] = w;
                    }
                }
            }

            for (int i = 0; i < 3; i++)
            {
                Bitmap ch = grayframe.Clone(listRect[i], grayframe.PixelFormat);
                Bitmap o = ch;
                //ch = con.Erodetion(ch);
                string temp;
                if (i == 2)
                {
                    temp = Ocr(ch, false, false); // nhan dien chu
                }
                else
                {
                    temp = Ocr(ch, false, true);// nhan dien so
                }

                zz += temp; 
            }
            zz += " - ";
            for (int i = 3; i < listRect.Count; i++)
            {
                Bitmap ch = grayframe.Clone(listRect[i], grayframe.PixelFormat);
                Bitmap o = ch;
                //ch = con.Erodetion(ch);
                string temp;
                             
                    temp = Ocr(ch, false, true); // nhan dien so
                    zz += temp;
                            
            }
            textBox1.Text = zz;
        }*/
        
        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void timer2_Tick(object sender, EventArgs e)
        {

            if (success == true)
            {
                success = false;
                new Thread(() =>
                {
                    try
                    {
                        capture.SetCaptureProperty(CAP_PROP.CV_CAP_PROP_FRAME_WIDTH, 640);
                        capture.SetCaptureProperty(CAP_PROP.CV_CAP_PROP_FRAME_HEIGHT, 480);
                        Image<Bgr, byte> cap = capture.QueryFrame();
                        if (cap != null)
                        {
                            MethodInvoker mi = delegate
                            {
                                try
                                {
                                    Bitmap bmp = cap.ToBitmap();
                                    pictureBox3.Image = bmp;
                                    IF.pictureBox4.Image = bmp;
                                    pictureBox3.Update();
                                    IF.pictureBox4.Update();

                                }
                                catch (Exception ex)
                                { }
                            };
                            if (InvokeRequired)
                                Invoke(mi);
                        }
                    }
                    catch (Exception) { }
                    success = true;
                }).Start();

            }
        }

        private void button12_Click(object sender, EventArgs e)
                       
        {
            if (capture != null)
            {
                timer2.Enabled = false;
                pictureBox1.Image = null;
                IF.pictureBox2.Image = null;
                capture.QueryFrame().Save("aa.bmp");
                FileStream fs = new FileStream(m_path + "aa.bmp", FileMode.Open, FileAccess.Read);
                Image temp = Image.FromStream(fs);               
                fs.Close();
                pictureBox1.Image = temp;
                IF.pictureBox2.Image = temp;
                pictureBox1.Update();
                IF.pictureBox2.Update();
                imgInput = new Image<Bgr, byte>(m_path + "aa.bmp");
                timer2.Enabled = true;
            }

        }

        private void timer4_Tick(object sender, EventArgs e)
        {
            label18.Text = DateTime.Now.ToLongTimeString();
        }

        private void button9_Click(object sender, EventArgs e)
        {
            if (IF.Visible == false)
            {
                IF.Show();
            }
            else
            {
                IF.Hide();
            }
            string sothe = "";
            string bienso = "";
           
            sothe = sothe + " - " + dateTimePicker1.Value.Second + dateTimePicker1.Value.Hour + bienso ;
            textBox3.Text = sothe;
            textBox4.Text = label18.Text + " - " + dateTimePicker1.Text;
        }

        private void button13_Click(object sender, EventArgs e)
        {

        }




        /*  #region WEBCAM
          WEBCAM[] cam = new WEBCAM[3];
          private void pictureBox_MouseDown(object sender, MouseEventArgs e)
          {
              if (e.Button == MouseButtons.Right)
              {
                  PictureBox p = (PictureBox)sender;
                  for (int i = 0; i < cam.Length; i++)
                  {
                      if (cam[i] != null && cam[i].status == "run" && cam[i].pb == p.Name)
                      {
                          cam[i].Stop();
                          cam[i] = null;
                      }
                  }
                  ContextMenu m = new ContextMenu();
                  List<string> ls = WEBCAM.get_all_cam();
                  for(int i = 0; i<=2 & i < ls.Count; i++)
                  {
                      m.MenuItems.Add(ls[i], (s, e2) => {
                          MenuItem menuItem = s as MenuItem;
                          ContextMenu owner = menuItem.Parent as ContextMenu;
                          PictureBox pb = (PictureBox)owner.SourceControl;
                          if (cam[menuItem.Index] != null && cam[menuItem.Index].status == "run")
                          {
                              cam[menuItem.Index].Stop();
                              //cam[menuItem.Index] = null;
                          }
                          cam[menuItem.Index] = new WEBCAM();
                          cam[menuItem.Index].Start(menuItem.Index);
                          cam[menuItem.Index].put_picturebox(pb.Name);
                      });
                  }
                  m.Show(p, new Point(e.X, e.Y));
              }
          }
          private void timer3_Tick(object sender, EventArgs e)
          {
              try
              {
                  for (int i = 0; i < cam.Length; i++)
                  {
                      if (cam[i] != null && cam[i].status == "run" && cam[i].image != null)
                      {
                          MethodInvoker mi = delegate
                          {
                              PictureBox pb = this.Controls.Find(cam[i].pb, true).FirstOrDefault() as PictureBox;
                              pb.Image = cam[i].image;
                              pb.Update();
                              pb.Invalidate();
                          };
                          if (InvokeRequired)
                          {
                              Invoke(mi);
                              return;
                          }

                          PictureBox pb2 = this.Controls.Find(cam[i].pb, true).FirstOrDefault() as PictureBox;
                          pb2.Image = cam[i].image;
                          pb2.Update();
                          pb2.Invalidate();
                      }
                  }
              }
              catch (Exception) { }
          }

          #endregion */
    }
}
 