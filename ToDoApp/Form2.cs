using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Npgsql;

namespace ToDoApp
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }

        //cancel button
        private void button3_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        //画像添付ボタン
        private void button1_Click(object sender, EventArgs e)
        {
            var img = ImageFileOpen();
            this.pictureBox1.Image = img;
        }

        /// <summary> 
        /// ファイルを開くダイアログボックスを表示して画像ファイルを開く 
        /// </summary> 
        /// <returns>生成したBitmapクラスオブジェクト</returns>
        private Bitmap ImageFileOpen()
        {
            //ファイルを開くダイアログボックスの作成  
            var ofd = new OpenFileDialog();
            //ファイルフィルタ  
            ofd.Filter = "Image File(*.bmp,*.jpg,*.png,*.tif,*.gif)|*.bmp;*.jpg;*.png;*.tif;*.gif|Bitmap(*.bmp)|*.bmp|Jpeg(*.jpg)|*.jpg|PNG(*.png)|*.png|GIF(*.gif)|*.gif";
            //ダイアログの表示 （Cancelボタンがクリックされた場合は何もしない）
            if (ofd.ShowDialog() == DialogResult.Cancel) return null;

            return ImageFileOpen(ofd.FileName);
        }

        /// <summary>
        /// ファイルパスを指定して画像ファイルを開く
        /// </summary>
        /// <param name="fileName">画像ファイルのファイルパスを指定します。</param>
        /// <returns>生成したBitmapクラスオブジェクト</returns>
        private Bitmap ImageFileOpen(string fileName)
        {
            // 指定したファイルが存在するか？確認
            if (System.IO.File.Exists(fileName) == false) return null;

            // 拡張子の確認
            var ext = System.IO.Path.GetExtension(fileName).ToLower();

            // ファイルの拡張子が対応しているファイルかどうか調べる
            var extConfirm = new List<string>() { ".bmp", ".jpg", ".png", ".tif", ".gif" };
            foreach (var item in extConfirm.Where(x => x == ext))
            {
                Bitmap bmp;

                // ファイルストリームでファイルを開く
                using (var fs = new System.IO.FileStream(
                    fileName,
                    System.IO.FileMode.Open,
                    System.IO.FileAccess.Read))
                {
                    bmp = new Bitmap(fs);
                }
                return bmp;
            }
            /* if (
                 (ext != ".bmp") &&
                 (ext != ".jpg") &&
                 (ext != ".png") &&
                 (ext != ".tif") &&
                 (ext != ".gif")
                 )*/

            return null;
        }

        private void button2_Click(object sender, EventArgs e)
        {
           
            
                insertSQL();
                this.Dispose();

        }
        private void insertSQL()
        {
            string title = this.textBox1.Text;
            string memo = this.textBox2.Text;
            string sql = $"INSERT INTO todo_items(title,date_start,date_end,memo) VALUES('{title}','{dateTimePicker1.Value}','{dateTimePicker2.Value}','{memo}');";
            string ConnectionString =
               "Server=127.0.0.1;"
               + "Port=5432;"
               + "Database=todoapp_db;"
               + "User ID=postgres;"
               + "Password=postgres;";
            NpgsqlConnection conn = new NpgsqlConnection(ConnectionString);
            conn.Open();
            
               
            NpgsqlCommand command = new NpgsqlCommand(sql, conn);
            int result = command.ExecuteNonQuery();


            conn.Close();
        }
    }
}
