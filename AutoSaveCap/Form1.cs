using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AutoSaveCap
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        /// <summary>
        /// ロード時に初期値を読み込む
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_Load(object sender, EventArgs e)
        {
            radioButton2.Checked = true;
            textBox2.Text = Application.StartupPath;
        }

        /// <summary>
        /// クリップボードから取り込み
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonClip_Click(object sender, EventArgs e)
        {
            var clip = Clipboard.GetDataObject();
            // 画像ファイルのみ取り込み
            var bmp = clip.GetData(typeof(Bitmap)) as Bitmap;
            if (bmp == null) return;
            // ピクチャへ表示
            pictureBox1.Image = bmp;
        }

        /// <summary>
        /// 特定フォルダに保存
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonSave_Click(object sender, EventArgs e)
        {
            Bitmap bmp = (Bitmap)pictureBox1.Image;
            if (bmp == null)
                return;

            ImageFormat fmt = ImageFormat.Bmp;
            if (radioButton1.Checked) fmt = ImageFormat.Bmp;
            if (radioButton2.Checked) fmt = ImageFormat.Jpeg;
            if (radioButton3.Checked) fmt = ImageFormat.Png;
            string fname = MakeFileName(fmt);
            bmp.Save(fname, fmt);

            textBox1.Text = "[img " + Path.GetFileName(fname) + "]";

        }

        string MakeFileName(ImageFormat fmt)
        {
            string path = textBox2.Text+"\\";
            DateTime dt = DateTime.Now;
            string dname = string.Format("{0:0000}{1:00}{2:00}", dt.Year, dt.Month, dt.Day);
            var files = System.IO.Directory.GetFiles(path);
            var file  = files.Where(f => f.IndexOf(dname + "_") > 0).Max();
            if ( file == null ) {
                path += dname + "_01." + fmt.ToString();
            } else {
                var mat = new Regex( @"_(\d+)\.").Match(file);
                int num = int.Parse( mat.Groups[1].Value );
                num++;
                path += dname + "_" + num.ToString("00") + "." + fmt.ToString();
            }
            return path;
        }

        /// <summary>
        /// ショートカットを有効にする
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control == true)
            {
                if (e.KeyCode == Keys.V)
                {
                    buttonClip_Click(sender, null);
                }
                else if (e.KeyCode == Keys.S)
                {
                    buttonSave_Click(sender, null);
                }
            }
        }

    }
}
