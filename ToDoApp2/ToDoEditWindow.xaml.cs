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

// TODO: namespace の { } を省略する新しい書き方があります。使ってみましょう。
namespace ToDoApp2
{


    /// <summary>
    /// ToDoEditWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class ToDoEditWindow : Window
    {
        private int _rowNumber;

        // TODO: コメント
        /// <summary>
        ///
        /// </summary>
        private readonly int _id;

        // TODO: コメント
        private readonly List<DataItem> _items;

        // TODO: DataItem を引数に取れるようにしましょう。
        public ToDoEditWindow(int id, List<DataItem> items)
        {
            this.InitializeComponent();

            this._id = id;
            this._items = items;

            this.SearchRowNumber();
            // TODO: this
            this.ReadColumns(_items);
        }

        // TODO: Load は呼び出して乗せるイメージなので、この処理では軽すぎます。rowNumber の検索はしていますが、set しかしていません。
        // TODO: _items のプレフィックスはフィールド変数の場合に使います。
        /// <summary>
        /// idに対応する行をSQLから取得し、表示します。
        /// </summary>
        private void ReadColumns(List<DataItem> _items)
        {

            // TODO: this
            var item = _items[_rowNumber];

            this.CheckDone.IsChecked = item.CheckDone;
            this.ToDoTitle.Text = item.ToDoTitle;
            this.DateEnd.Text = item.DateEnd.ToString();
            this.Memo.Text = item.Memo;
        }

        /// <summary>
        /// メインウィンドウから渡された<see cref="_id"/>と一致するIdを持つ<see cref="_items">の要素を検索します。
        /// </summary>
        /// <returns>_itemsの要素番号です。</returns>
        private void SearchRowNumber()
        {
            // TODO: index を取得したい場合は、for の方がよいでしょう。
            // TODO: LINQとタプルを使う方法もあるので、考えてみましょう。別メソッドとして作ってみてください。

            for(int i = 0; i < this._items.Count; i++)
            {
                var item = this._items[i];
                if (item.Id == _id)
                {
                    _rowNumber = i;
                }
            }
        }

        /// <summary>
        /// 変更を保存ボタンをクリックすると、現在テキストボックスなどに入力されている内容に応じてSQLにUPDATEを実行します。
        /// その後、ToDoEditWindowを閉じます。
        /// </summary>
        private void ChangeButton_Click(object sender, RoutedEventArgs e)
        {
            // TODO: 下記の処理の中で this.Close() があるのは、あまりよくありません。
            // それ以外をメソッド化してみてください。
            try
            {
                // TODO: WHERE のあとは改行してください。
                // TODO: AND の後のスペースはひとつにしてください。
                // TODO: WHERE の次の AND のインデントが足りません。
                // TODO: 基本的にPK列を使って更新します。
                // TODO: 変数化
                // TODO: this
                var sql = $@"
UPDATE todo_items SET
    check_done = {this.CheckDone.IsChecked}
  , title = '{this.ToDoTitle.Text}'
  , date_end = '{this.DateEnd.SelectedDate.Value}'
  , memo = '{this.Memo.Text}'
  , date_update = current_timestamp
WHERE id = {this._id}
 AND  date_update > '{_items[_rowNumber].DateUpdate}'
";

                {
                    using var connection = new NpgsqlConnection(Constants.ConnectionString);

                    // TODO: try-catch
                    connection.Open();
                    var command = new NpgsqlCommand(sql, connection);
                    // TODO: 返り値は何になりますか？更新されなかった場合はどうなりますか？
                    var result = command.ExecuteNonQuery();
                }

                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.ToString());
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
