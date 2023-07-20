using Microsoft.Win32;
using System;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;
using Npgsql;
using System.Data;
using System.Data.SqlClient;
using System.Data.Common;
using System.Windows.Documents;
using System.Collections.Generic;

namespace ToDoApp2
{
    /// <summary>
    /// ToDoInputWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class ToDoInputWindow : Window
    {
        private readonly List<string> FilenameExtensions = new List<string>() { ".bmp", ".jpg", ".png", ".tiff", ".gif", ".icon", "webp" };

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

            // DONE: 末尾にコメントを残すコメント方法は基本的に禁止しています。
            // 行コピーがしにくい
            // インデントを揃えてもズレるので見づらくなる、揃える手間がかかる
            // 横スクロールは悪なので、横には伸ばさないようにする
            // このようなコメントが無くても良いように命名や構造を心がける
            // などの理由が考えられます。
            // WEBで解説を優先するためにこのような表記をすることはありますが、
            // コピペしてきた場合はコメントを削除してきれいにしておきましょう。
            if (fileNames is null) return;

            var fileName = fileNames[0];
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

            // DONE: 拡張子の List<T> にして Contains() または Exists() で判定するようにしましょう。
            // ファイルの拡張子が対応しているか確認する
            if (!FilenameExtensions.Contains(ext))
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
        private void InsertToDoItem()
        {
            try
            {
                string title = this.ToDoTitle.Text;
                string memo = this.Memo.Text;
                // DONE: 英文の場合は , の後ろには基本的にスペースを入れます。
                // ソースコードは基本的には英語なので、このルールは覚えておくとよいでしょう。
                // () の前後にもスペースも英文的にあったほうがよいです。
                // DONE: SQLの記述ルールとして、インデントはスペース2つとされることが多いです。
                // DONE: 大量のスペースによるインデントはエディタに貼り付けるときに邪魔になります。
                // 先頭に改行を入れて、参考例(sql2)のように、べったり左寄せにして書いてしまいましょう。
                //　ソースコード全体として見たときに気になるようであれば region で囲っておくと、折り畳めます。
                #region SQL文
                string sql = $@"
INSERT INTO todo_items(
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
);";

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

                #endregion SQL文

                // DONE: キャメルケース
                // DONE？: IDBConnectionで受けてみましょう。

                // IDbConnectionでNpgsqlConnectionを受けることはできますが、SQLコマンドの実行時にエラーが出ます。
                using (var connection = new NpgsqlConnection(Constants.connectionString))
                {
                    connection.Open();
                    // DONE: DbCommandもIDisposableを継承しています。
                    using (IDbCommand command = new NpgsqlCommand(sql, connection))
                    {
                        int result = command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}
