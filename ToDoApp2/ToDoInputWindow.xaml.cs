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
        // DONE: 固定値だけなのでConstantsにおいたほうがよさそうです。
        // DONE: リストで固定値を設定するときは、量にもよりますが、縦に並べたほうが追加しやすくなるでしょう。

        public ToDoInputWindow()
        {
            this.InitializeComponent();

            // DONE: イベントはxaml側に統一しましょう。
            // やるならアプリケーション全部で統一した方が迷いません。
            // 他のWindowではxaml側、他のWindowではコードビハインドと設定する箇所が入り乱れると間違いの元です。
            // 今までxamlに定義されていたので、このxamlを見て、イベントないねーって判断することが考えられます。

        }

        // DONE: イベントメソッドは、ControlName_EventName() としましょう。
        /// <summary>
        /// （WIP）Imageにドロップした画像ファイルを取得します。
        /// </summary>
        private void Image_Drop(object sender, DragEventArgs e)
        {
            var fileNames = e.Data.GetData(DataFormats.FileDrop) as string[];

            // DONE: { } をつけたら改行しましょう。
            if (fileNames is null)
            {
                return;
            }

            var fileName = fileNames[0];
            this.ReadImageFile(fileName);
        }

        // DONE: OpenImageFile という名前だと画像を開くという意味だけなので、もっと適切な名前が考えられます。
        /// <summary>
        /// 画像ファイルを開いてImageコントロールに表示します。
        /// </summary>
        /// <param name="fileName">画像ファイルの名前です。</param>
        private void ReadImageFile(string fileName)
        {
            var ext = System.IO.Path.GetExtension(fileName).ToLower();
            // DONE: null にならなそうです。色々な fileName を渡して確認してみましょう。

            if (!Constants.fileExtensions.Contains(ext))
            {
                // DONE?: 画像ファイルの拡張子は他にも考えられます。対応していない拡張子の場合も下記のメッセージが表示されてしまいます。
                MessageBox.Show(
                    "このファイルには対応していません。",
                    "お知らせ",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
                return;
            }

            // DONE: 外部リソースを使う場合は try-catch は重要です。
            try
            {
                var bmpImage = new BitmapImage();
                // DONE: この場合の中カッコはスコープを区切る意味がないので不要です。
                using FileStream stream = File.OpenRead(fileName);
                bmpImage.BeginInit();
                bmpImage.StreamSource = stream;
                bmpImage.DecodePixelWidth = 500;
                bmpImage.CacheOption = BitmapCacheOption.OnLoad;
                bmpImage.CreateOptions = BitmapCreateOptions.None;
                bmpImage.EndInit();
                this.SelectedImage.Source = bmpImage;

            }
            catch (Exception ex)
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
                var title = this.ToDoTitle.Text;
                var memo = this.Memo.Text;

                #region SQL文
                // DONE: this
                string sql = $@"
INSERT INTO todo_items (
    title
  , date_start
  , date_end
  , memo
  , image
)
VALUES (
    '{this.ToDoTitle}'
  , '{this.StartDate.SelectedDate.Value}'
  , '{this.EndDate.SelectedDate.Value}'
  , '{this.Memo}'
  , '{this.SelectedImage.Source}'
);
";
                #endregion SQL文

                // DONE: var connection はよく var conn と省略されます。
                // DONE: using declaration
                // DONE: 実行時エラーはわかりませんので、再現できたら教えてください。
                // キャストすれば実行できました。
                {
                    using IDbConnection conn = new NpgsqlConnection(Constants.ConnectionString);

                    // TODO: 以下は他の箇所のコメントを参考にしてください。
                    try
                    {
                        conn.Open();

                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(this, ex.Message);
                    }


                    try
                    {
                        using IDbCommand command = new NpgsqlCommand(sql, (NpgsqlConnection)conn);
                        int result = command.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(this, ex.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message);
            }
        }

        private void ImageChooseButton_Click(object sender, RoutedEventArgs e)
        {
            // DONE: ofd より dialog と略す方がよいです。
            var dialog = new OpenFileDialog();
            // DONE: owner の指定
            // DONE: キャンセルした場合はどうなりますか？
            var dialogResult = dialog.ShowDialog(this) ?? false;
            if (!dialogResult)
            {
                return;
            }
            this.ReadImageFile(dialog.FileName);
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            this.InsertToDoItem();
            this.Close();
        }
    }
}
