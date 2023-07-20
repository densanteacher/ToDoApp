using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ToDoApp2
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        // TODO: _ のプレフィックス
        private readonly List<DataItem> items = new List<DataItem>();

        public MainWindow()
        {
            // TODO: this
            InitializeComponent();

            this.ShowToDoList();
        }

        // TODO: region と配置

        // TODO: コメント
        private void InputToDo_Click(object sender, RoutedEventArgs e)
        {
            // TODO: tdiw の略語は何かわからないので、もう少しわかる略語の方がいいです。
            // 変数名の略語として w, win, window で検討してみます。
            // w だけだと width と見分けがつきません。
            // win だと windows と見分けが付きません。
            // window まで書けば、迷うことはありません。
            // todoInputWindow まで書くとさすがに冗長です。
            var tdiw = new ToDoInputWindow();
            tdiw.Owner = this;
            tdiw.ShowDialog();
        }

        // TODO: コメント
        // TODO: Show は Window を表示するときに使われる単語です。より適切なメソッド名を考えましょう。
        private void ShowToDoList()
        {
            // DONE: SQL文の終わりは、;(セミコロン) で終わりましょう。
            // ただし、Oracleの場合はセミコロンがあるとエラーになります。複数DBMSをターゲットにする場合は考慮します。
            // TODO: SELECT の列名は * で省略せずに書きましょう。
            // SELECT * は実務ではあまり使わないほうがよいです。
            // テーブル変更があった場合、問題となる場合があります。
            var sql = "SELECT * FROM todo_items ORDER BY check_done, date_end;";

            // TODO: using
            // TODO: キャメルケース
            // TODO: インターフェースで受けましょう。
            var Connection = new NpgsqlConnection(Constants.connectionString);
            // TODO: using declaration
            using (var command = new NpgsqlCommand(sql, Connection))
            {
                // TODO: try-catch
                // Open だけで try-catch した方がエラー理由がわかりやすくなります。
                Connection.Open();

                // TODO: try-catch
                using (var reader = command.ExecuteReader())
                {
                    try
                    {
                        // TODO: try-catch
                        while (reader.Read())
                        {
                            // TODO: this
                            // TODO: 一度 var item に入れましょう。
                            // TODO: キャストではないデータの取得方法を行ってみましょう。GetXxx
                            items.Add(new DataItem((int)reader["id"],
                                                (bool)reader["check_done"],
                                                (string)reader["title"],
                                                (string)reader["memo"],
                                                (DateTime)reader["date_start"],
                                                (DateTime)reader["date_end"],
                                                (int)reader["priority"]));
                        }
                        // TODO: this
                        this.ToDoDataGrid.ItemsSource = items;
                    }
                    finally
                    {
                        // TODO: finally が必要になるのは、Openした場合です。そこをカバーするような範囲で try-catch-finally としましょう。
                        Connection.Close();
                    }
                }
            }
        }

        /// <summary>
        /// 詳細ボタンをクリックすることで、DataGridで選択している行のToDoEditWindowを呼び出します。
        /// </summary>
        private void DetailButton_Click(object sender, RoutedEventArgs e)
        {
            // TODO: この位置の UpdateLayout は不要です。
            this.ToDoDataGrid.UpdateLayout();
            // TODO: 使い方が間違っています。
            this.ToDoDataGrid.ScrollIntoView(ToDoDataGrid.Columns[0]);

            // TODO: var a = this.ToDoDataGrid.SelectedItem as DataItem; のようにキャストしてみましょう。as より is のほうが安全です。
            var test = (TextBlock)this.ToDoDataGrid.Columns[0].GetCellContent(this.ToDoDataGrid.SelectedItem);

            // TODO: Text は Row ではないので、命名がフェイクになっています。
            string selectedRow = test?.Text;
            var isSuccess = int.TryParse(selectedRow, out var id);
            // TODO: { } が必要です。
            if (!isSuccess) return;

            // TODO: Ctrl + K, D
            var tdew = new ToDoEditWindow(id,items);
            tdew.Owner = this;
            tdew.ShowDialog();
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
