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
        // TODO: コメント
        private readonly int _id;

        // TODO: コメント
        // TODO: コンストラクタで入れているなら、ここでの new は不要です。
        private readonly List<DataItem> _items = new List<DataItem>();

        // TODO: DataItem を引数に取れるようにしましょう。
        public ToDoEditWindow(int id, List<DataItem> items)
        {
            this.InitializeComponent();

            // TODO: this
            _id = id;
            _items = items;

            this.LoadRowColumns(_items);
        }

        // TODO: Load は呼び出して乗せるイメージなので、この処理では軽すぎます。rowNumber の検索はしていますが、set しかしていません。
        // TODO: _items のプレフィックスはフィールド変数の場合に使います。
        /// <summary>
        /// idに対応する行をSQLから取得し、表示します。
        /// </summary>
        private void LoadRowColumns(List<DataItem> _items)
        {

            // TODO: this
            var i = SearchRowNumber();
            // TODO: items[i] は一度変数に格納しましょう。

            this.CheckDone.IsChecked = _items[i].CheckDone;
            this.ToDoTitle.Text = _items[i].ToDoTitle;
            this.DateEnd.Text = _items[i].DateEnd.ToString();
            this.Memo.Text = _items[i].Memo;
        }


        // TODO: コメント

        /// <summary>
        /// メインウィンドウから渡された<see cref="_id"/>と一致するIdを持つ<see cref="_items">の要素を検索します。
        /// </summary>
        /// <returns>_itemsの要素番号です。</returns>
        private int SearchElementNumber()
        {
            // TODO: index を取得したい場合は、for の方がよいでしょう。
            // TODO: LINQとタプルを使う方法もあるので、考えてみましょう。別メソッドとして作ってみてください。
            int i = 0;
            // TODO: this
            foreach (DataItem item in _items)
            {
                if (item.Id != _id)
                {
                    i++;
                    // TODO: Ctrl + K, D
                } else
                {
                    return i;
                }
            }

            return i;
        }

        /// <summary>
        /// 変更を保存ボタンをクリックすると、現在テキストボックスなどに入力されている内容に応じてSQLにUPDATEを実行します。
        /// その後、ToDoEditWindowを閉じます。
        /// </summary>
        private void ChangeButton_Click(object sender, RoutedEventArgs e)
        {

            // TODO: SearchRowNumber は何度も使わないで、一度探してその値を保持しておけばよいでしょう。
            // TODO: this
            var i = SearchElementNumber();

            try
            {
                // TODO: = の前後にスペースがほしいです。
                // TODO: WHERE はインデントせず UPDATE の位置に揃えます。
                // TODO: AND の後のスペースはひとつにしてください。
                // TODO: this
                // TODO: 基本的にPK列を使って更新します。
                var sql = $@"
UPDATE todo_items SET
    check_done={this.CheckDone.IsChecked}
  , title='{this.ToDoTitle.Text}'
  , date_end='{this.DateEnd.SelectedDate.Value}'
  , memo='{this.Memo.Text}'
  , date_update=current_timestamp
    WHERE id = {_id}
  AND  date_update > '{_items[i].DateUpdate}'
";

                // TODO: using declaration
                using (var connection = new NpgsqlConnection(Constants.connectionString))
                {
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
                // TODO: 開いた元のインスタンスを指定しておいたほうがよい。
                MessageBox.Show(ex.ToString());
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
