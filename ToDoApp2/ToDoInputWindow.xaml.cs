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
using Dropbox.Api;
using System.Threading.Tasks;

namespace ToDoApp2;

/// <summary>
/// ToDoInputWindow.xaml の相互作用ロジック
/// </summary>
public partial class ToDoInputWindow : Window
{
    private string _imagePath;

    private readonly int _maxId;

    private string _ext;
    public ToDoInputWindow(int maxId)
    {
        this.InitializeComponent();

        this._maxId = maxId + 1;

        this.PriorityComboBox.ItemsSource = Constants.Priorities;
        this.PriorityComboBox.SelectedIndex = 5;

        this.ImageFrame.Source = new RenderTargetBitmap(450, 450, 96.0d, 96.0d, PixelFormats.Pbgra32);

    }

    /// <summary>
    /// Imageにドロップした画像ファイルを取得します。
    /// </summary>
    private void ImageFrame_Drop(object sender, DragEventArgs e)
    {
        var droppedData = e.Data.GetData(DataFormats.FileDrop);
        if (droppedData is string[] fileNames)
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

        this._imagePath = fileName;
        this._ext = ext;

        try
        {
            var bmpImage = new BitmapImage();
            using FileStream stream = File.OpenRead(fileName);
            bmpImage.BeginInit();
            bmpImage.StreamSource = stream;
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
    /// データベースにデータを送信、保存します。
    /// </summary>
    private void InsertToDoItem()
    {
        try
        {
            var imagePath = "";

            if (this._imagePath is not null)
            {
                var task = this.UploadDropbox();
                imagePath = Constants.DropboxImagePath + Constants.DropboxImageName + this._maxId + _ext;
            }

            #region SQL文

            var title = this.ToDoTitle.Text;
            var memo = this.Memo.Text;
            if (!(int.TryParse(this.PriorityComboBox.Text, out int priority)))
            {
                return;
            }
            var startDate = this.StartDate.SelectedDate.Value;
            var endDate = this.EndDate.SelectedDate.Value;

            // TODO: データベースに画像を保存する方法を解説します。
            // this.ImageFrame.Sourceに入れたのはbmp形式になります。
            // それを一度変数に格納し、そのときにBitmapImageにasでキャストしておきます。
            // データベースに格納するときに、バイト配列に変換する必要があります。
            // BitmapImageをバイト配列に変換してからテーブルに挿入しましょう。
            // あとは、bmpに変換しないでエンコードされた状態で保存したい場合は、
            // ファイルの種類を知るための列があった方がよいでしょう。
            // https://symfoware.blog.fc2.com/blog-entry-1280.html

            string sql = $@"
INSERT INTO todo_items (
    title
  , date_start
  , date_end
  , memo
  , priority
  , image_path
)
VALUES (
    '{title}'
  , '{startDate}'
  , '{endDate}'
  , '{memo}'
  , '{priority}'
  , '{imagePath}'
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
                using var command = conn.CreateCommand();
                command.CommandText = sql;
                command.CommandTimeout = Constants.TimeoutSecond;
                var result = command.ExecuteNonQuery();
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

    /// <summary>
    /// 「画像を追加」ボタンを押したとき、ダイアログを開いて読み込む画像をユーザーに選択させます。
    /// </summary>
    private void ImageChooseButton_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new OpenFileDialog();
        var dialogResult = dialog.ShowDialog(this) ?? false;
        if (dialogResult)
        {
            this.ReadImageFile(dialog.FileName);
        }
    }

    /// <summary>
    /// Cancelボタンを押したとき、ウィンドウを閉じます。
    /// </summary>
    private void CancelButton_Click(object sender, RoutedEventArgs e)
    {
        this.Close();
    }

    /// <summary>
    /// OKボタンを押したとき、内容を保存してからウィンドウを閉じます。
    /// </summary>
    private void OkButton_Click(object sender, RoutedEventArgs e)
    {
        this.InsertToDoItem();
        this.Close();
    }

    private async Task<bool> UploadDropbox()
    {
        var result = this.CreateDropboxFolder();

        using var client = new DropboxClient(Constants.DropboxAccessToken);


        var saveFolderName = Constants.DropboxImagePath;

        //アップロードファイル名
        var saveFileName = Constants.DropboxImageName + this._maxId + this._ext;

        //アップロードファイルパス
        var uploadSource = _imagePath;

        //ファイルのアップロード
        using var stream = new MemoryStream(File.ReadAllBytes(uploadSource));

        //ストリームに変換して、bodyに渡す
        await client.Files.UploadAsync(saveFolderName + saveFileName, body: stream);

        return await this.UploadDropbox();
    }

    private async Task<bool> CreateDropboxFolder()
    {
        using var client = new DropboxClient(Constants.DropboxAccessToken);

        //作成フォルダ
        var folder = Constants.DropboxImagePath;
        //フォルダ作成
        await client.Files.CreateFolderV2Async(folder);

        var result = await this.CreateDropboxFolder();
        return result;
    }
}
