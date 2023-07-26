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
using System.Windows.Media;

namespace ToDoApp2;

/// <summary>
/// ToDoInputWindow.xaml の相互作用ロジック
/// </summary>
public partial class ToDoInputWindow : Window
{
    public ToDoInputWindow()
    {
        this.InitializeComponent();

        this.PriorityComboBox.ItemsSource = Constants.Priorities;
        this.PriorityComboBox.SelectedIndex = 5;

        this.ImageFrame.Source = new RenderTargetBitmap(450, 450, 96.0d, 96.0d, PixelFormats.Pbgra32);

    }

    /// <summary>
    /// Imageにドロップした画像ファイルを取得します。
    /// </summary>
    private void ImageFrame_Drop(object sender, DragEventArgs e)
    {
        // DONE: if の条件は肯定的な方が読みやすくなります。not でわかりやすくなる時以外は、not をつけないようにしましょう。
        if (e.Data.GetData(DataFormats.FileDrop) is string[] fileNames)
        {
            var fileName = fileNames[0];
            this.ReadImageFile(fileName);
        }

    }

    /// <summary>
    /// 画像ファイルを開いてImageコントロールに表示します。
    /// </summary>
    /// <param name="fileName">画像ファイルの名前です。</param>
    private void ReadImageFile(string fileName)
    {
        var ext = System.IO.Path.GetExtension(fileName).ToLower();

        if (!Constants.FileExtensions.Contains(ext))
        {
            MessageBox.Show(
                "このファイルには対応していません。",
                "お知らせ",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
            return;
        }

        try
        {
            var bmpImage = new BitmapImage();
            using FileStream stream = File.OpenRead(fileName);
            bmpImage.BeginInit();
            bmpImage.StreamSource = stream;
            bmpImage.DecodePixelWidth = 500;
            bmpImage.CacheOption = BitmapCacheOption.OnLoad;
            bmpImage.CreateOptions = BitmapCreateOptions.None;
            bmpImage.EndInit();
            this.ImageFrame.Source = bmpImage;
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

            #region SQL文

            var title = this.ToDoTitle.Text;
            var memo = this.Memo.Text;
            if (!(int.TryParse(this.PriorityComboBox.Text, out int priority)))
            {
                return;
            }
            var startDate = this.StartDate.SelectedDate.Value;
            var endDate = this.EndDate.SelectedDate.Value;

            string sql = $@"
INSERT INTO todo_items (
    title
  , date_start
  , date_end
  , memo
  , priority
  , image
)
VALUES (
    '{title}'
  , '{startDate}'
  , '{endDate}'
  , '{memo}'
  , '{priority}'
  , '{this.ImageFrame.Source}'
);
";

            #endregion SQL文

            using IDbConnection conn = new NpgsqlConnection(Constants.ConnectionString);

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
                // TODO: ここは var が使えます。
                // conn が IDbConnection なので、IDbCommand を返すようになっています。
                using IDbCommand command = conn.CreateCommand();
                command.CommandText = sql;
                command.CommandTimeout = 15;
                command.CommandType = CommandType.Text;
                // TODO: var
                int result = command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, ex.Message);
        }
    }

    private void ImageChooseButton_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new OpenFileDialog();
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
