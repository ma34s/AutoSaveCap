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
        private Bitmap savedBmp;
        private ClipBoardWatcher cbw;
        public Form1()
        {
            InitializeComponent();
            cbw = null;
        }
        /// <summary>
        /// ロード時に初期値を読み込む
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_Load(object sender, EventArgs e)
        {
            radioButton2.Checked = true;
            buttonSave.Enabled = false;

            textBox2.Text = Application.StartupPath;
        }
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (cbw != null)
            {
                cbw.Dispose();
                cbw = null;
            }
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
            

            bool isImgUpdaed = true;
            if (pictureBox1.Image == null)
            {//初回取り込み
                pictureBox1.Image = bmp;
            }
            else
            {//2回目以降
                if (BitmapUtil.Compare(bmp, (Bitmap)pictureBox1.Image) == false)
                {//更新あり
                    pictureBox1.Image = bmp;

                    if (savedBmp != null)
                    {
                        if (BitmapUtil.Compare(bmp, savedBmp) == true)
                        {
                            isImgUpdaed = false;
                        }
                    }
                }
            }
            buttonSave.Enabled = isImgUpdaed;
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

            buttonSave.Enabled = false;
            if (savedBmp != null)
            {
                if(BitmapUtil.Compare(bmp, savedBmp)==true)
                {
                    return;
                }
            }
            //
            savedBmp = new Bitmap(bmp);

            ImageFormat fmt = ImageFormat.Bmp;
            if (radioButton1.Checked) fmt = ImageFormat.Bmp;
            if (radioButton2.Checked) fmt = ImageFormat.Jpeg;
            if (radioButton3.Checked) fmt = ImageFormat.Png;
            string fname = MakeFileName(fmt);
            if (fname != "")
            {
                bmp.Save(fname, fmt);
                textBox1.Text = "[img " + Path.GetFileName(fname) + "]";
            }
            else
            {
                textBox1.Text = "[error] Error could not access the output directory";
            }

        }

        string MakeFileName(ImageFormat fmt)
        {
            string path = textBox2.Text+"\\";
            if (!System.IO.Directory.Exists(path))
            {
                try
                {
                    System.IO.Directory.CreateDirectory(path);
                }
                catch (Exception)
                {
                    return "";
                }
            }

#if false
            DateTime dt = DateTime.Now;
            string dname = string.Format("{0:0000}{1:00}{2:00}", dt.Year, dt.Month, dt.Day);
#else
            string dname = "img";
#endif
            var files = System.IO.Directory.GetFiles(path);
            var file  = files.Where(f => f.IndexOf(dname + "_") > 0).Max();
            if ( file == null ) {
                path += dname + "_01." + fmt.ToString().ToLower();
            } else {
                var mat = new Regex( @"_(\d+)\.").Match(file);
                int num = int.Parse( mat.Groups[1].Value );
                num++;
                path += dname + "_" + num.ToString("00") + "." + fmt.ToString().ToLower();
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

        private void ButtonWatch_Click(object sender, EventArgs e)
        {
            buttonWatch.Enabled = false;//2度押し防止
            if (labelWatchStatus.Text=="監視中")
            {
                if (cbw != null)
                {
                    cbw.Dispose();
                    cbw = null;
                }

                labelWatchStatus.Text = "監視停止中";

            }
            else
            {
                labelWatchStatus.Text = "監視中";
                cbw = new ClipBoardWatcher();
                cbw.DrawClipBoard += (sender2, e2) => {
                    if(Clipboard.ContainsImage())
                    {
                        buttonClip_Click(sender, null);
                        buttonSave_Click(sender, null);
                    }
                };
            }
            buttonWatch.Enabled = true;//2度押し防止
        }

 
    }
}
