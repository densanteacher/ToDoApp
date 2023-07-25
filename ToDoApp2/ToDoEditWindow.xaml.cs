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
using static ToDoApp2.MainWindow;

// DONE: namespace の { } を省略する新しい書き方があります。使ってみましょう。
namespace ToDoApp2;



/// <summary>
/// ToDoEditWindow.xaml の相互作用ロジック
/// </summary>
public partial class ToDoEditWindow : Window
{
    // DONE: コメント
    /// <summary>
    /// <see cref="MainWindow"/>から渡されたSQLのIDです。
    /// </summary>
    private readonly int _id;

    // DONE: コメント
    /// <summary>
    /// <see cref="MainWindow"/>から渡されたSQLの読み取り結果です。
    /// </summary>
    private readonly DataItem _item;

    // DONE: DataItem を引数に取れるようにしましょう。
    public ToDoEditWindow(int id,DataItem item)
    {
        this.InitializeComponent();

        this._id = id;
        this._item = item;

        this.PriorityList.ItemsSource = Constants.PriorityDataList;

        // DONE: this
        this.SetColumns(this._item);
    }

    // DONE: Load は呼び出して乗せるイメージなので、この処理では軽すぎます。rowNumber の検索はしていますが、set しかしていません。
    // DONE: _items のプレフィックスはフィールド変数の場合に使います。
    /// <summary>
    /// idに対応する行をSQLから取得し、表示します。
    /// </summary>
    private void SetColumns(DataItem item)
    {

        // DONE: this

        this.CheckDone.IsChecked = item.CheckDone;
        this.ToDoTitle.Text = item.ToDoTitle;
        this.DateEnd.Text = item.DateEnd.ToString();
        this.Memo.Text = item.Memo;
    }

    /// <summary>
    /// メインウィンドウから渡された<see cref="_id"/>と一致するIdを持つ<see cref="_item">の要素を検索します。
    /// </summary>
    /// <returns>_itemsの要素番号です。</returns>
   /* private void SearchRowNumber()
    {
        // DONE: index を取得したい場合は、for の方がよいでしょう。
        // DONE: LINQとタプルを使う方法もあるので、考えてみましょう。別メソッドとして作ってみてください。

        for (int i = 0; i < this._item.Count; i++)
        {
            var item = this._item;
            if (item.Id == _id)
            {
                _rowNumber = i;
            }
        }
    }
    private void SearchRowNumber2()
    {
        var indexList = this._item.Select((item, index) => (item, index)).ToList();
        indexList.ForEach(tuple =>
        {
            if (tuple.item.Id == _id)
            {
                _rowNumber = tuple.index;
            }
        });
    }*/

    /// <summary>
    /// 変更を保存ボタンをクリックすると、現在テキストボックスなどに入力されている内容に応じてSQLにUPDATEを実行します。
    /// その後、ToDoEditWindowを閉じます。
    /// </summary>
    private void ChangeButton_Click(object sender, RoutedEventArgs e)
    {
        // DONE: 下記の処理の中で this.Close() があるのは、あまりよくありません。
        // それ以外をメソッド化してみてください。
        this.UpdateSql();

        this.Close();
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e)
    {
        this.Close();
    }
    private void UpdateSql()
    {
        try
        {
            // DONE: WHERE のあとは改行してください。
            // DONE: AND の後のスペースはひとつにしてください。
            // DONE: WHERE の次の AND のインデントが足りません。
            // TODO: 基本的にPK列を使って更新します。
            // DONE: 変数化
            // DONE: this
            var dateEnd = this.DateEnd.SelectedDate.Value;
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

            {
                using var conn = new NpgsqlConnection(Constants.ConnectionString);

                // DONE: try-catch
                try
                {
                    conn.Open();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, ex.Message);
                }
                using var command = new NpgsqlCommand(sql, conn);
                // DONE: 返り値は何になりますか？更新されなかった場合はどうなりますか？

                // UPDATEは何かを読み取るわけではないのでExecuteReaderではなくExecuteNonQueryを使用するべきでした。
                // 戻り値は更新の影響を受けた行数で、更新されなかった場合は戻り値は０になります。
                var result = command.ExecuteNonQuery();
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, ex.Message);
        }
    }
}
