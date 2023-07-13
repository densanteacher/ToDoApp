using Microsoft.Win32;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;
using Npgsql;

namespace ToDoApp2
{
    /// <summary>
    /// ToDoInputWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class ToDoInputWindow : Window
    {
        public ToDoInputWindow()
        {
            InitializeComponent();

            this.Cancel.Click += (s, e) =>
            {
                this.Close();
            };
            this.OK.Click += (s, e) =>
            {
                this.InsertSQL();
                this.Close();
            };
            this.ImageChoose.Click += (s, e) =>
            {
                var ofd = new OpenFileDialog();
                ofd.ShowDialog();
                this.OpenImageFile(ofd.FileName);
            };
            this.SelectedImage.Drop += (s, e) =>
            {
                DropImage(s, e);
            };
        }

        private void DropImage(object sender, DragEventArgs e)
        {
            var fileNames = e.Data.GetData(DataFormats.FileDrop) as string[];
            if (fileNames is null) return;                    // nullなら返す

            var fileName = fileNames[0];                      // 画像ファイルのパスを取得
            this.OpenImageFile(fileName);
        }

        /// <summary>
        /// 画像ファイルを開いてImageコントロールに表示します。
        /// </summary>
        /// <param name="fileName">画像ファイルの名前です。</param>
        private void OpenImageFile(string fileName)
        {
            var ext = System.IO.Path.GetExtension(fileName).ToLower();  // 拡張子の確認
            if(ext == null)
            {
                return;
            }
            // ファイルの拡張子が対応しているか確認する
            if (ext != ".bmp" && ext != ".jpg" && ext != ".jpeg" && ext != ".png" && ext != ".tiff" && ext != ".gif" && ext != ".icon")
            {
                MessageBox.Show("画像ファイルを選択してください。", "お知らせ", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            BitmapImage bmpImage = new BitmapImage();
            using (FileStream stream = File.OpenRead(fileName))
            {
                bmpImage.BeginInit();
                bmpImage.StreamSource = stream;
                bmpImage.DecodePixelWidth = 500;
                bmpImage.CacheOption = BitmapCacheOption.OnLoad;
                bmpImage.CreateOptions = BitmapCreateOptions.None;
                bmpImage.EndInit();
            }

            SelectedImage.Source = bmpImage;
        }
        /// <summary>
        /// SQLにデータを送信、保存します。
        /// </summary>
        private void InsertSQL()
        {
            string title = this.ToDoTitle.Text;
            string memo = this.Memo.Text;
            string sql = $"INSERT INTO todo_items(title,date_start,date_end,memo,image) " +
                $"VALUES('{title}','{StartDate.SelectedDate.Value}','{EndDate.SelectedDate.Value}','{memo}','{SelectedImage.Source}');";
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
