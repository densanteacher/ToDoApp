using Microsoft.Win32;
using System;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;
using Npgsql;
using System.Data;
using System.Data.SqlClient;
using System.Data.Common;

namespace ToDoApp2
{
    /// <summary>
    /// ToDoInputWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class ToDoInputWindow : Window
    {
        public ToDoInputWindow()
        {
            this.InitializeComponent();

            this.Cancel.Click += (s, e) =>
            {
                this.Close();
            };
            this.OK.Click += (s, e) =>
            {
                this.InsertToDoItem();
                this.Close();
            };
            this.ImageChoose.Click += (s, e) =>
            {
                var ofd = new OpenFileDialog();
                ofd.ShowDialog();
                this.OpenImageFile(ofd.FileName);
            };
        }

        /// <summary>
        /// （WIP）Imageにドロップした画像ファイルを取得します。
        /// </summary>
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
            if (ext == null)
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

        // DONE: このメソッドで挿入されるのは、todo_item となります。Insert"SQL" という名前だと、SQLを挿入するという訳のわからない意味になります。
        /// <summary>
        /// SQLにデータを送信、保存します。
        /// </summary>
        private void InsertToDoItem()
        {
            try
            {
                string title = this.ToDoTitle.Text;
                string memo = this.Memo.Text;
                string sql = $@"INSERT INTO todo_items(
                                title
                               ,date_start
                               ,date_end
                               ,memo
                               ,image
                            )
                            VALUES(
                                '{title}'
                               ,'{StartDate.SelectedDate.Value}'
                               ,'{EndDate.SelectedDate.Value}'
                               ,'{memo}'
                               ,'{SelectedImage.Source}'
                            );";

                // INFO: C# には @"" の文字列でエスケープなしで記述できる方法がありますが、これは改行も含めることができます。
                // C# でｈ逐次的文字列リテラルなんて呼び方をしますが、他のプログラミング言語ではヒアドキュメントと呼ばれます。
                // おそらくヒアドキュメントの方が通りがよいかと思います。
                // SQL文などの長い文字列を記述したいときに重宝します。
                string sql2 = $@"
INSERT INTO todo_items (
    title
  , date_start
  , date_end
  , memo
  , image
)
VALUES (
    '{title}'
  , '{StartDate.SelectedDate.Value}'
  , '{EndDate.SelectedDate.Value}'
  , '{memo}'
  , '{SelectedImage.Source}'
)
;
";

                // TODO: キャメルケース
                // DONE: Constantsを定義して接続文字列を移しましょう。

                // DONE: ファイルやDBなど外部のリソースを使うときは忘れずにusing句を使いましょう。
                // TODO: IDBConnectionで受けてみましょう。
                using (var connection = new NpgsqlConnection(Constants.ConnectionString))
                {
                    connection.Open();
                    IDbCommand command = new NpgsqlCommand(sql, connection);
                    int result = command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}
