using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Drawing.Imaging;
using System.Reflection;

namespace ThumbCreator
{
    public partial class ThumbW : Form
    {
        public static string AppTitle = "ThumbCreator " + Assembly.GetExecutingAssembly().GetName().Version.ToString();

        public ThumbW()
        {
            InitializeComponent();
            this.Text = AppTitle;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Archivos de Imágenes (*.bmp; *.png; *.jpg)|*.bmp; *.png; *.jpg";
            ofd.Multiselect = true;
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                foreach (string s in ofd.FileNames)
                    if (!listBox1.Items.Contains(s))
                        listBox1.Items.Add(s);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            listBox1.Items.Clear();
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox1.SelectedItems.Count == 0) return;

            string newname = listBox1.SelectedItems[0].ToString();
            FileInfo finfo = new FileInfo(newname);

            previewImage.Load(newname);

            double ratio1 = (double)previewImage.Image.Width / (double)previewImage.Image.Height;
            double ratio2 = (double)previewImage.Image.Height / (double)previewImage.Image.Width;

            int b_height = 90;
            int b_width = 160;
            b_width = Math.Min((int)Math.Round(ratio1 * b_height), b_width);
            b_height = Math.Min((int)Math.Round(ratio2 * b_width), b_height);

            previewImage.Width = b_width;
            previewImage.Height = b_height;

            infoResolucion.Text = previewImage.Image.Width + "x" + previewImage.Image.Height;
            infoTamaño.Text = finfo.Length + " bytes";
            infoProfundidad.Text = previewImage.Image.PixelFormat.ToString();
            infoUltimaModificacion.Text = finfo.LastWriteTime.ToString();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (listBox1.Items.Count == 0) return;
            bool errs = false;

            for (int i=0; i<listBox1.Items.Count; i++)
            {
                try
                {
                    FileInfo fi = new FileInfo(listBox1.Items[i].ToString());
                    Bitmap orig = new Bitmap(fi.FullName);
                    double ratio = (double)orig.Width / (double)orig.Height;
                    int new_x = int.Parse(thumb_x.Text);
                    int new_y = (keepRatio.Checked)? Convert.ToInt32(Math.Floor((double)new_x / ratio)) : int.Parse(thumb_y.Text);

                    Bitmap newb = (doResize.Checked) ? (isBilinear.Checked) ? BitmapFunctions.Resize(orig, new_x, new_y, true) : BitmapFunctions.FastResize(orig, new_x, new_y) : orig;

                    string new_f = Path.Combine(fi.Directory.ToString(), fi.Name.Substring(0, fi.Name.Length - fi.Extension.Length) + ((doResize.Checked) ? ".thumb." : ".new.") + ((isJPG.Checked) ? "jpg" : "png"));
                    /*
                    if (isPNG.Checked)
                        newb.Save(new_f, ImageFormat.Png);
                    else {
                        EncoderParameters enc = new EncoderParameters(1);
                        enc.Param[0] = new EncoderParameter( System.Drawing.Imaging.Encoder.Quality, 100L);
                        ImageCodecInfo[] bleh = ImageCodecInfo.GetImageEncoders(); //GetEncoderInfo("image/jpeg");
                        ImageCodecInfo mio = null;
                        for (int x = 0; x < bleh.Length; x++)
                            if ((bleh[x].MimeType).Equals("image/jpeg")) mio = bleh[x];
                        //ImageCodecInfo ici2 = ImageCodecInfo.
                        newb.Save(new_f, mio, enc);
                    }*/

                    SaveImageToFile(newb, new_f, isPNG.Checked);

                    newb.Dispose();
                    orig.Dispose();
                }
                catch (Exception xxx) { errs = true;  MessageBox.Show("Se ha detectado un error creando el thumb de " + listBox1.Items[i].ToString()+"\n\nError:\n"+xxx.Message, "ThumbW", MessageBoxButtons.OK, MessageBoxIcon.Error); }
            }
            if (errs)
                MessageBox.Show("Thumbnails generados con algún error.", "ThumbW", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            else
                MessageBox.Show("Thumbnails generados correctamente.", "ThumbW", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void SaveImageToFile(Bitmap OriginalImage, string NewFile, bool IsPNG)
        {
            if (IsPNG)
                OriginalImage.Save(NewFile, ImageFormat.Png);
            else
            {
                EncoderParameters enc = new EncoderParameters(1);
                enc.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 100L);
                ImageCodecInfo[] bleh = ImageCodecInfo.GetImageEncoders(); //GetEncoderInfo("image/jpeg");
                ImageCodecInfo mio = null;
                for (int x = 0; x < bleh.Length; x++)
                    if ((bleh[x].MimeType).Equals("image/jpeg")) mio = bleh[x];
                //ImageCodecInfo ici2 = ImageCodecInfo.
                OriginalImage.Save(NewFile, mio, enc);
            }
        }

        private bool IsValidFile(string FileName)
        {
            FileInfo fi = new FileInfo(FileName);

            switch (fi.Extension.ToLower())
            {
                case ".bmp":
                case ".png":
                case ".jpg":
                case ".jpeg":
                case ".tga":
                    return true;
            }

            return false;
        }

        private void ThumbW_Load(object sender, EventArgs e)
        {
            listBox1.DragEnter += new DragEventHandler(listBox1_DragEnter);
            listBox1.DragDrop += new DragEventHandler(listBox1_DragDrop);
        }

        void listBox1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop, false) == true)
                e.Effect = DragDropEffects.All;
        }

        void listBox1_DragDrop(object sender, DragEventArgs e)
        {
            string[] ss = (string[])e.Data.GetData(DataFormats.FileDrop);
            foreach (string s in ss)
                if ((!listBox1.Items.Contains(s)) && (IsValidFile(s)))
                    listBox1.Items.Add(s);
        }

        private void doResize_CheckedChanged(object sender, EventArgs e)
        {
            groupBox1.Enabled = doResize.Checked;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            // grande
            if (listBox1.SelectedItems.Count == 0) return;

            string newname = listBox1.SelectedItems[0].ToString();

            FileInfo fi = new FileInfo(newname);
            Bitmap orig = new Bitmap(fi.FullName);

            double ratio = (double)orig.Width / (double)orig.Height;
            int new_x = 422;
            int new_y = Convert.ToInt32(Math.Floor((double)new_x / ratio));

            Bitmap newb = BitmapFunctions.FastResize(orig, new_x, new_y);

            string new_f = Path.Combine(fi.Directory.ToString(), fi.Name.Substring(0, fi.Name.Length - fi.Extension.Length) + ".thumbG." + ((isJPG.Checked) ? "jpg" : "png"));
            SaveImageToFile(newb, new_f, isPNG.Checked);

            newb.Dispose();
            orig.Dispose();

            MessageBox.Show("Thumbnail (422x para la web de AU) generado correctamente.", "ThumbW", MessageBoxButtons.OK, MessageBoxIcon.Information);

        }

        private void button5_Click(object sender, EventArgs e)
        {
            // peke
            if (listBox1.SelectedItems.Count == 0) return;

            string newname = listBox1.SelectedItems[0].ToString();

            FileInfo fi = new FileInfo(newname);
            Bitmap orig = new Bitmap(fi.FullName);

            double ratio = (double)orig.Width / (double)orig.Height;
            int new_x = 102;
            int new_y = Convert.ToInt32(Math.Floor((double)new_x / ratio));

            Bitmap newb = BitmapFunctions.FastResize(orig, new_x, new_y);

            string new_f = Path.Combine(fi.Directory.ToString(), fi.Name.Substring(0, fi.Name.Length - fi.Extension.Length) + ".thumbP." + ((isJPG.Checked) ? "jpg" : "png"));
            SaveImageToFile(newb, new_f, isPNG.Checked);

            newb.Dispose();
            orig.Dispose();

            MessageBox.Show("Thumbnail (102x para la web de AU) generado correctamente.", "ThumbW", MessageBoxButtons.OK, MessageBoxIcon.Information);

        }
    }
}