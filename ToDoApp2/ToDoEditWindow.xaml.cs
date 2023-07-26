using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ToDoApp2;

/// <summary>
/// ToDoEditWindow.xaml の相互作用ロジック
/// </summary>
public partial class ToDoEditWindow : Window
{
    /// <summary>
    /// <see cref="MainWindow"/>から渡されたToDoリストのSQLデータベースにおけるIDです。
    /// </summary>
    private readonly int _id;

    // TODO: 今回の使用ですと MainWindow から渡されますが、他の画面から呼ばれた場合はコメントを直す必要があります。
    // クラスは独立したものと考えるとよいでしょう。
    // 各コメントにかかれているSQLという言葉も不適切です。
    // 全体を通してコメントを見直してみてください。
    /// <summary>
    /// <see cref="MainWindow"/>から渡されたToDoリストの読み取り結果です。
    /// </summary>
    private readonly DataItem _item;

    public ToDoEditWindow(int id, DataItem item)
    {
        this.InitializeComponent();

        this._id = id;
        this._item = item;

        this.PriorityComboBox.ItemsSource = Constants.Priorities;

        this.SetColumns(this._item);
    }

    /// <summary>
    /// <see cref="MainWindow"/>から渡された値を表示します。
    /// </summary>
    private void SetColumns(DataItem item)
    {
        this.CheckDone.IsChecked = item.CheckDone;
        this.ToDoTitle.Text = item.ToDoTitle;
        this.DateEnd.Text = item.DateEnd.ToString();
        this.Memo.Text = item.Memo;
        this.PriorityComboBox.Text = item.Priority.ToString();
    }

    // TODO: コメント部分にも // を追加します。// が多いですがドキュメンテーションコメントもメソッドの一部です。
    /// <summary>
    /// メインウィンドウから渡された<see cref="_id"/>と一致するIdを持つ<see cref="_item">の要素を検索します。
    /// </summary>
    /// <returns>_itemsの要素番号です。</returns>
    private void SearchRowNumber2()
    {
        // DONE: 書きにくいと思うのでダミーです。
        var items = new List<DataItem>();
        var rowNumber = 0;

        // TODO: .ForEach() を使わないので .ToList() は不要になります。
        // TODO: スペース4つのインデントを入れましょう。
        // TODO: .First() の使い方が間違っています。最初のひとつを受け取ることができるので・・・？
        //items.Select((item, index) => (item, index))
        //.ToList()
        //.First(tuple =>
        //{
        //    if (tuple.item.Id == _id)
        //    {
        //        rowNumber = tuple.index;
        //    }
        //});
    }

    /// <summary>
    /// 変更を保存ボタンをクリックすると、選択されたToDoの内容を更新します。
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

            // TODO: bool変数の名前
            var checkDone = this.CheckDone.IsChecked ?? false;
            var dateEnd = this.DateEnd.SelectedDate.Value;
            if (!(int.TryParse(this.PriorityComboBox.Text, out var priority)))
            {
                return;
            }
            var sql = $@"
UPDATE todo_items SET
    check_done = {checkDone}
  , title = '{this.ToDoTitle.Text}'
  , date_end = '{dateEnd}'
  , memo = '{this.Memo.Text}'
  , priority = {priority}
  , updated_at = current_timestamp
WHERE
    id = {this._id}
";

            #endregion SQL文

            // TODO: ここは IDbConnectionで受けましょう。
            using var conn = new NpgsqlConnection(Constants.ConnectionString);

            try
            {
                conn.Open();
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message);
            }

            // TODO: インターフェースを使ったやり方に変えましょう。
            using var command = new NpgsqlCommand(sql, conn);
            var result = command.ExecuteNonQuery();
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, ex.Message);
        }
    }
}
