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
        // DONE: キャメルケース、_ プレフィックス
        // DONE: Filename は FileName だし、ファイル拡張子は name は不要です。
        // DONE: webp のピリオド
        private readonly List<string> _fileExtensions = new List<string>() { ".bmp", ".jpg", ".png", ".tiff", ".gif", ".icon", ".webp" };

        public ToDoInputWindow()
        {
            this.InitializeComponent();

            // TODO: イベントはxaml側に統一しましょう。
            // やるならアプリケーション全部で統一した方が迷いません。
            // 他のWindowではxaml側、他のWindowではコードビハインドと設定する箇所が入り乱れると間違いの元です。
            // 今までxamlに定義されていたので、このxamlを見て、イベントないねーって判断することが考えられます。
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
                // TODO: ofd より dialog と略す方がよいです。
                var dialog = new OpenFileDialog();
                // TODO: owner の指定
                dialog.ShowDialog(this);
                // TODO: キャンセルした場合はどうなりますか？
                this.OpenImageFile(dialog.FileName);
            };
        }

        // TODO: イベントメソッドは、ControlName_EventName() としましょう。
        /// <summary>
        /// （WIP）Imageにドロップした画像ファイルを取得します。
        /// </summary>
        private void Image_Drop(object sender, DragEventArgs e)
        {
            var fileNames = e.Data.GetData(DataFormats.FileDrop) as string[];

            // DONE: { } をつけましょう。
            if (fileNames is null) { return; }

            var fileName = fileNames[0];
            this.OpenImageFile(fileName);
        }

        // TODO: OpenImageFile という名前だと画像を開くという意味だけなので、もっと適切な名前が考えられます。
        /// <summary>
        /// 画像ファイルを開いてImageコントロールに表示します。
        /// </summary>
        /// <param name="fileName">画像ファイルの名前です。</param>
        private void OpenImageFile(string fileName)
        {
            // DONE: 末尾にコメントを残すコメント方法は基本的に禁止しています。
            var ext = System.IO.Path.GetExtension(fileName).ToLower();
            // TODO: null にならなそうです。色々な fileName を渡して確認してみましょう。
            if (ext == null)
            {
                return;
            }

            // DONE: this
            if (!this._fileExtensions.Contains(ext))
            {
                // TODO: 画像ファイルの拡張子は他にも考えられます。対応していない拡張子の場合も下記のメッセージが表示されてしまいます。
                MessageBox.Show(
                    "画像ファイルを選択してください。",
                    "お知らせ",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
                return;
            }

            // TODO: 外部リソースを使う場合は try-catch は重要です。
            // DONE: var で受けましょう。
            try
            {
                var bmpImage = new BitmapImage();
                // TODO: using declaration
                //using (FileStream stream = File.OpenRead(fileName))
                {
                    using FileStream stream = File.OpenRead(fileName);
                    bmpImage.BeginInit();
                    bmpImage.StreamSource = stream;
                    bmpImage.DecodePixelWidth = 500;
                    bmpImage.CacheOption = BitmapCacheOption.OnLoad;
                    bmpImage.CreateOptions = BitmapCreateOptions.None;
                    bmpImage.EndInit();

                    // DONE: this
                    this.SelectedImage.Source = bmpImage;
                }

            } catch(Exception ex) 
            {
                MessageBox.Show(ex.ToString());
            }

        }

        /// <summary>
        /// SQLにデータを送信、保存します。
        /// </summary>
        private void InsertToDoItem()
        {
            try
            {
                // DONE: var
                var title = this.ToDoTitle.Text;
                var memo = this.Memo.Text;

                #region SQL文
                // DONE: todo_items( の間にスペースがほしいです。
                // DONE: 閉じカッコの)位置は、はじめの開始カッコ(がある行のインデントに揃える方がよいです。この場合はINSERTの頭に揃えます。
                // DONE: これは個人的なものですが、最後のセミコロンは別の行にしておいてください。コピペしやすくなります。
                string sql = $@"
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
);
";

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

                // TODO: var connection はよく var conn と省略されます。
                // TODO: using declaration
                // TODO: 実行時エラーはわかりませんので、再現できたら教えてください。
                // IDbConnectionでNpgsqlConnectionを受けることはできますが、SQLコマンドの実行時にエラーが出ます。
                using (var conn = new NpgsqlConnection(Constants.ConnectionString))
                {
                    // TODO: 以下は他の箇所のコメントを参考にしてください。
                    conn.Open();
                    using (IDbCommand command = new NpgsqlCommand(sql, conn))
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
