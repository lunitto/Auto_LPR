using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Auto_parking
{
    public partial class ImageForm : Form
    {
        public ImageForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Hide();
        }

        #region di chuyển
        bool mouseDown = false;
        Point lastLocation;

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
        #endregion

        private void ImageForm_Load(object sender, EventArgs e)
        {
            timer1.Start();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            textBox1.Text = textBox6.Text;
            textBox2.Text = dateTimePicker1.Value.Second + dateTimePicker1.Value.Hour + " - " + textBox1.Text;
            textBox3.Text = label4.Text + " - " + dateTimePicker1.Text;



        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            label4.Text = DateTime.Now.ToLongTimeString();
        }
    }
}
