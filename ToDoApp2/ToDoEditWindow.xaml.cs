using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Data.SqlClient;
using Dropbox.Api;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Interop;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Security.Cryptography;

namespace ToDoApp2;

// TODO: 新規登録も編集も同じような機能なので、どちらかのWindowにまとめてしまってよいでしょう。
// レイアウトだけ別々にしたいというなら中心となる機能をまとめたクラスを用意するとよいでしょう。

/// <summary>
/// ToDoタスクの詳細表示・編集を行うウィンドウです。
/// </summary>
public partial class ToDoEditWindow : Window
{
    /// <summary>
    /// ウィンドウ呼び出し時に渡されたToDoリストの読み取り結果です。
    /// </summary>
    private readonly ToDoData _item;

    public ToDoEditWindow(ToDoData item)
    {
        this.InitializeComponent();

        this._item = item;

        this.PriorityComboBox.ItemsSource = Constants.Priorities;

        this.SetValues(this._item);
    }

    /// <summary>
    /// ウィンドウ呼び出し時に渡された値を表示します。
    /// </summary>
    private async void SetValues(ToDoData item)
    {
        this.IsFinished.IsChecked = item.IsFinished;
        this.ToDoTitle.Text = item.ToDoTitle;
        this.DateStart.Text = item.DateStart.ToString();
        this.DateEnd.Text = item.DateEnd.ToString();
        this.Memo.Text = item.Memo;
        this.PriorityComboBox.Text = item.Priority.ToString();

        if (item.ImagePath == "" || item.ImagePath is null)
        {
            return;
        }

        try
        {
            using var client = new DropboxClient(Constants.DropboxAccessToken);
            var response = await client.Files.DownloadAsync(item.ImagePath);
            var imageBytes = await response.GetContentAsByteArrayAsync();
            var ms = new MemoryStream(imageBytes);

            var bmpImage = new BitmapImage();
            bmpImage.BeginInit();
            bmpImage.StreamSource = ms;
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
    /// 「変更を保存」ボタンをクリックすると、選択されたToDoの内容を更新します。
    /// その後、ToDoEditWindowを閉じます。
    /// </summary>
    private void ChangeButton_Click(object sender, RoutedEventArgs e)
    {
        this.UpdateToDoItem();

        this.Close();
    }

    /// <summary>
    /// Cancelボタンを押すと、ウィンドウを閉じます。
    /// </summary>
    private void CancelButton_Click(object sender, RoutedEventArgs e)
    {
        this.Close();
    }

    /// <summary>
    /// UPDATEコマンドを用いて、現在テキストボックスなどに入力されている内容に応じてToDoの内容を更新します。
    /// </summary>
    private void UpdateToDoItem()
    {
        try
        {
            #region SQL文

            var isFinished = this.IsFinished.IsChecked ?? false;
            var dateEnd = this.DateEnd.SelectedDate.Value;
            if (!(int.TryParse(this.PriorityComboBox.Text, out var priority)))
            {
                return;
            }
            var sql = $@"
UPDATE todo_items SET
    is_finished = @IS_FINISHED
  , title = @TITLE
  , date_end = @DATE_END
  , memo = @MEMO
  , priority = @PRIORITY
  , updated_at = @UPDATED_AT
WHERE
    id = @ID
";

            #endregion SQL文

            using var command = DbConnect.PrepareSqlCommand(sql);

            command.Parameters.AddWithValue("@IS_FINISHED", isFinished);
            command.Parameters.AddWithValue("@TITLE", this.ToDoTitle.Text);
            command.Parameters.AddWithValue("@DATE_END", dateEnd);
            command.Parameters.AddWithValue("@MEMO", this.Memo.Text);
            command.Parameters.AddWithValue("@PRIORITY", priority);
            command.Parameters.AddWithValue("@UPDATED_AT", DateTime.Now);
            command.Parameters.AddWithValue("@ID", this._item.Id);
            
            var result = command.ExecuteNonQuery();
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, ex.Message);
        }
    }
}