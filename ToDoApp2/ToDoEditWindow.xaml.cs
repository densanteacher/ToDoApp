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

// TODO: using static 構文はなるべく使わないようにします。static なメソッドを呼ぶときにインスタンスメソッドと見分けがつかなくなってミスの元になります。
using static ToDoApp2.MainWindow;

namespace ToDoApp2;

/// <summary>
/// ToDoEditWindow.xaml の相互作用ロジック
/// </summary>
public partial class ToDoEditWindow : Window
{
    // TODO: コメントの内容だと、SQLが何を指すのか、IDが何を指すのかわかりません。
    /// <summary>
    /// <see cref="MainWindow"/>から渡されたSQLのIDです。
    /// </summary>
    private readonly int _id;

    // TODO: コメントの内容だと、SQLが何を指すのかわかりません。
    /// <summary>
    /// <see cref="MainWindow"/>から渡されたSQLの読み取り結果です。
    /// </summary>
    private readonly DataItem _item;

    // TODO: Ctrl + K, D
    public ToDoEditWindow(int id,DataItem item)
    {
        this.InitializeComponent();

        this._id = id;
        this._item = item;

        this.PriorityList.ItemsSource = Constants.PriorityDataList;

        this.SetColumns(this._item);
    }

    /// <summary>
    /// idに対応する行をSQLから取得し、表示します。
    /// </summary>
    private void SetColumns(DataItem item)
    {
        this.CheckDone.IsChecked = item.CheckDone;
        this.ToDoTitle.Text = item.ToDoTitle;
        this.DateEnd.Text = item.DateEnd.ToString();
        this.Memo.Text = item.Memo;
    }

    // TODO: /* */ の範囲コメントは使わないほうがよいです。git上での変化は2行だけですので、マージで苦労することになります。
    // DONE: ドキュメンテーションコメントもコメンアウト内に含めてしまってください。
    // TODO: LINQ の Select() からメソッドチェーンで記述してみてください。
    // TODO: ForEach() はすべて処理されますが、この場合はひとつ取れればよいので、First() や FirstOrDefault() を使ってみましょう。Where() を使ってもよいです。
    /*
    /// <summary>
    /// メインウィンドウから渡された<see cref="_id"/>と一致するIdを持つ<see cref="_item">の要素を検索します。
    /// </summary>
    /// <returns>_itemsの要素番号です。</returns>
    private void SearchRowNumber2()
    {
        var indexes = this._items.Select((item, index) => (item, index)).ToList();
        indexes.ForEach(tuple =>
        {
            if (tuple.item.Id == _id)
            {
                this._rowNumber = tuple.index;
            }
        });
    }
    */

    /// <summary>
    /// 変更を保存ボタンをクリックすると、現在テキストボックスなどに入力されている内容に応じてSQLにUPDATEを実行します。
    /// その後、ToDoEditWindowを閉じます。
    /// </summary>
    private void ChangeButton_Click(object sender, RoutedEventArgs e)
    {
        this.UpdateSql();

        this.Close();
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e)
    {
        this.Close();
    }

    // TODO: UpdateSql() だと、SQLを更新という意味になってしまうので・・・？
    private void UpdateSql()
    {
        try
        {
            var dateEnd = this.DateEnd.SelectedDate.Value;
            // TODO: isChecked は null を取ります。つまり・・・？
            var sql = $@"
UPDATE todo_items SET
    check_done = {this.CheckDone.IsChecked}
  , title = '{this.ToDoTitle.Text}'
  , date_end = '{dateEnd}'
  , memo = '{this.Memo.Text}'
  , date_update = current_timestamp
WHERE
    id = {this._id}
";

            // TODO: この中括弧は不要です。変数が生き残るスコープを常に意識しましょう。
            {
                using var conn = new NpgsqlConnection(Constants.ConnectionString);

                try
                {
                    conn.Open();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, ex.Message);
                }

                using var command = new NpgsqlCommand(sql, conn);
                var result = command.ExecuteNonQuery();
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, ex.Message);
        }
    }
}
