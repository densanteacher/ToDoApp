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

namespace ToDoApp2
{


    /// <summary>
    /// ToDoEditWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class ToDoEditWindow : Window
    {
        private readonly int _id;
        private readonly List<DataItem> _items = new List<DataItem>();

        public ToDoEditWindow(int id, List<DataItem> items)
        {
            this.InitializeComponent();

            _id = id;
            _items = items;


            // DONE: Getという命名だと、戻り値があるイメージです。
            // また軽量な処理を想像しますので、DBアクセスするメソッドにGetは使わないほうがよいです。
            // Readは直接読み出すイメージ、Loadは読み出して乗せるイメージ、Fetchはサーバーから取得してくるイメージ・・・適切なメソッド名を考えてみましょう。
            this.LoadRowColumns(_items);
        }

        // DONE?: アプリケーションの設計によりますが、ToDoEditWindowを開いた時点でDBから再取得すると、画面上に表示されている間に変更があった場合、差異ができて、あれ？となります。
        // 一人でアクセスする場合は問題になりませんが、複数人で扱う場合はこの方法だとまずそうです。
        // RDBMSとしてSQLiteを使うならまだしも、PostgreSQLを使う場合は、複数アクセス前提で考えておきましょう。
        // また複数人で更新が重なった場合は、後からの人で上書きされてしまうことが予想されます。
        // 対策方法を考えてみましょう。

        // 保持しておいた_itemsリストと内容が一致するか確認して、一致したら更新を行う様に変更しました。

        /// <summary>
        /// idに対応する行をSQLから取得し、表示します。
        /// </summary>
        private void LoadRowColumns(List<DataItem> _items)
        {
            var i = SearchRowNumber();
            this.CheckDone.IsChecked = _items[i].CheckDone;
            this.ToDoTitle.Text = _items[i].ToDoTitle;
            this.DateEnd.Text = _items[i].DateEnd.ToString();
            this.Memo.Text = _items[i].Memo;
        }

        private int SearchRowNumber()
        {
            int i = 0;
            foreach (DataItem item in _items)
            {
                if (item.Id != _id)
                {
                    i++;
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
            var i = SearchRowNumber();
            try
            {
                var sql = $@"
UPDATE todo_items SET
    check_done={this.CheckDone.IsChecked}
  , title='{this.ToDoTitle.Text}'
  , date_end='{this.DateEnd.SelectedDate.Value}'
  , memo='{this.Memo.Text}'
    WHERE id = {_id}
  AND  check_done={_items[i].CheckDone}
  AND  title='{_items[i].ToDoTitle}'
  AND  date_end='{_items[i].DateEnd.ToString()}'
  AND  memo='{_items[i].Memo}'
";

                using (var connection = new NpgsqlConnection(Constants.connectionString))
                {
                    connection.Open();
                    var command = new NpgsqlCommand(sql, connection);
                    var result = command.ExecuteNonQuery();
                }

                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {

            this.Close();
        }
    }
}
