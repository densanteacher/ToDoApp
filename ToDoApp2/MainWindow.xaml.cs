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
        // DONE: _ のプレフィックス
        private readonly List<DataItem> _items = new List<DataItem>();

        public MainWindow()
        {
            // DONE: this
            this.InitializeComponent();

            this.ReloadToDoList();
        }



        // DONE: コメント
        // DONE?: Show は Window を表示するときに使われる単語です。より適切なメソッド名を考えましょう。
        /// <summary>
        /// <see cref="this.ToDoDataGrid"/>にToDoリストを表示します。
        /// </summary>
        private void ReloadToDoList()
        {
            this.ToDoDataGrid.ItemsSource = null;
            this.ToDoDataGrid.Items.Clear();

            _items.Clear();

            this.ToDoDataGrid.Items.Refresh();

            // DONE: SQL文の終わりは、;(セミコロン) で終わりましょう。
            // ただし、Oracleの場合はセミコロンがあるとエラーになります。複数DBMSをターゲットにする場合は考慮します。
            // DONE: SELECT の列名は * で省略せずに書きましょう。
            // SELECT * は実務ではあまり使わないほうがよいです。
            // テーブル変更があった場合、問題となる場合があります。

            var sql = $@"
SELECT id
  , check_done
  , title
  , memo
  , date_start
  , date_end
  , priority
  , date_update 
FROM todo_items 
ORDER BY check_done
  , date_end;";

            // DONE: using
            // DONE: キャメルケース
            // DONE: インターフェースで受けましょう

            // DONE: using declaration
            {
                using IDbConnection connection = new NpgsqlConnection(Constants.ConnectionString);
                using IDbCommand command = new NpgsqlCommand(sql, (NpgsqlConnection)connection);

                // DONE: try-catch
                // Open だけで try-catch した方がエラー理由がわかりやすくなります。
                try
                {
                    connection.Open();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, ex.Message);
                }

                // DONE: try-catch
                try
                {
                    using var reader = command.ExecuteReader();

                    // DONE: try-catch
                    try
                    {
                        while (reader.Read())
                        {
                            // DONE: this
                            // DONE: 一度 var item に入れましょう。
                            // DONE: キャストではないデータの取得方法を行ってみましょう。GetXxx
                            /*var item = new DataItem((int)reader["id"],
                                                (bool)reader["check_done"],
                                                (string)reader["title"],
                                                (string)reader["memo"],
                                                (DateTime)reader["date_start"],
                                                (DateTime)reader["date_end"],
                                                (int)reader["priority"],
                                                (DateTime)reader["date_update"]);*/
                            var item = new DataItem(reader.GetInt32(0),
                                                reader.GetBoolean(1),
                                                reader.GetString(2),
                                                reader.GetString(3),
                                                reader.GetDateTime(4),
                                                reader.GetDateTime(5),
                                                reader.GetInt32(6),
                                                reader.GetDateTime(7));
                            this._items.Add(item);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(this, ex.Message);
                    }
                    // DONE: this
                    this.ToDoDataGrid.ItemsSource = this._items;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, ex.Message);
                }
                finally
                {
                    // TODO: finally が必要になるのは、Openした場合です。そこをカバーするような範囲で try-catch-finally としましょう。
                    connection.Close();
                }
            }
        }

        private int SearchIdentification()
        {
            // DONE: この位置の UpdateLayout は不要です。
            // DONE: 使い方が間違っています。
            if (this.ToDoDataGrid.SelectedItem is null)
            {
                return -1;
            }

            this.ToDoDataGrid.ScrollIntoView(this.ToDoDataGrid.SelectedItem);

            // DONE: var a = this.ToDoDataGrid.SelectedItem as DataItem; のようにキャストしてみましょう。as より is のほうが安全です。
            if (!(this.ToDoDataGrid.SelectedItem is DataItem item))
            {
                return -1;
            }
            // DONE: Text は Row ではないので、命名がフェイクになっています。
            var cell = (TextBlock)this.ToDoDataGrid.Columns[0].GetCellContent(item);
            string selectedId = cell?.Text;
            // DONE: { } が必要です。

            if (!(int.TryParse(selectedId, out var id))) { return -1; }
            return id;
        }

        // TODO: region と配置

        #region Clickイベント

        // DONE: コメント

        /// <summary>
        /// ToDo入力ボタンを押すと、<see cref="ToDoInputWindow"/>を表示します。
        /// </summary>

        private void InputToDo_Click(object sender, RoutedEventArgs e)
        {
            // DONE: tdiw の略語は何かわからないので、もう少しわかる略語の方がいいです。
            // 変数名の略語として w, win, window で検討してみます。
            // w だけだと width と見分けがつきません。
            // win だと windows と見分けが付きません。
            // window まで書けば、迷うことはありません。
            // todoInputWindow まで書くとさすがに冗長です。
            var tdiWindow = new ToDoInputWindow();
            tdiWindow.Owner = this;
            tdiWindow.ShowDialog();
        }

        /// <summary>
        /// 詳細ボタンをクリックすることで、DataGridで選択している行のToDoEditWindowを呼び出します。
        /// </summary>
        private void DetailButton_Click(object sender, RoutedEventArgs e)
        {
            var id = this.SearchIdentification();

            if (id == -1)
            {
                return;
            }
            // DONE: Ctrl + K, D
            var tdeWindow = new ToDoEditWindow(id, _items);
            tdeWindow.Owner = this;
            tdeWindow.ShowDialog();
        }


        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            this.ReloadToDoList();
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            var id = this.SearchIdentification();
            if (id == -1)
            {
                return;
            }

            var sql = $@" DELETE FROM todo_items WHERE id = {id};";

            {
                using IDbConnection conn = new NpgsqlConnection(Constants.ConnectionString);
                using IDbCommand command = new NpgsqlCommand(sql, (NpgsqlConnection)conn);
                try
                {
                    conn.Open();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }

                try
                {
                    command.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
                finally
                {
                    conn.Close();
                }
            }

            this.ReloadToDoList();
        }

        private void BulkDeleteButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(this, "実行済みのToDoリストを全て削除します。よろしいですか？", "確認", MessageBoxButton.OKCancel, MessageBoxImage.Question);

            var sql = $@" DELETE FROM todo_items WHERE check_done = true;";

            {
                using IDbConnection conn = new NpgsqlConnection(Constants.ConnectionString);
                using IDbCommand command = new NpgsqlCommand(sql, (NpgsqlConnection)conn);

                try
                {
                    conn.Open();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }

                try
                {
                    command.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
                finally
                {
                    conn.Close();
                }
            }

            this.ReloadToDoList();
        }


        private void PriorityUpButton_Click(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void PriorityDownButton_Click(object obj, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }
        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        #endregion Clickイベント

    }
}
