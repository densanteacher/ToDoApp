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
        // TODO: コメント
        private readonly List<DataItem> _items = new List<DataItem>();

        public MainWindow()
        {
            this.InitializeComponent();

            this.ReloadToDoList();
        }



        // DONE?: Show は Window を表示するときに使われる単語です。より適切なメソッド名を考えましょう。
        // TODO: see ではインスタンスを指定できないので、this は使えません。
        /// <summary>
        /// <see cref="this.ToDoDataGrid"/>にToDoリストを表示します。
        /// </summary>
        private void ReloadToDoList()
        {
            this.ToDoDataGrid.ItemsSource = null;
            this.ToDoDataGrid.Items.Clear();

            // TODO: this
            _items.Clear();

            this.ToDoDataGrid.Items.Refresh();

            // TODO: SELECT, FROM, ORDER BY の箇所で改行してください。
            // TODO: date_update と todo_items の後ろに不要なスペースが入っています。
            // TODO: ; (セミコロン) は別行にしてください。
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

            {
                // TODO: conn
                using IDbConnection connection = new NpgsqlConnection(Constants.ConnectionString);
                using IDbCommand command = new NpgsqlCommand(sql, (NpgsqlConnection)connection);

                try
                {
                    connection.Open();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, ex.Message);
                }

                try
                {
                    using var reader = command.ExecuteReader();

                    try
                    {
                        while (reader.Read())
                        {
                            // TODO: できればインデントは、スペース4つに統一しましょう。
                            // この場合は最初の DataItem( で改行してしまった方が揃うと思います。
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
            if (this.ToDoDataGrid.SelectedItem is null)
            {
                return -1;
            }

            this.ToDoDataGrid.ScrollIntoView(this.ToDoDataGrid.SelectedItem);

            if (!(this.ToDoDataGrid.SelectedItem is DataItem item))
            {
                return -1;
            }

            // TODO: キャストではなく・・・？
            var cell = (TextBlock)this.ToDoDataGrid.Columns[0].GetCellContent(item);
            string selectedId = cell?.Text;

            // TODO: 改行を入れてください。
            if (!(int.TryParse(selectedId, out var id))) { return -1; }
            return id;
        }

        // TODO: region と配置

        #region Clickイベント

        /// <summary>
        /// ToDo入力ボタンを押すと、<see cref="ToDoInputWindow"/>を表示します。
        /// </summary>
        private void InputToDo_Click(object sender, RoutedEventArgs e)
        {
            // TODO: tdiWindow → window でよいです。
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

            // TODO: this
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

            var sql = $@"DELETE FROM todo_items WHERE id = {id};";

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

            var sql = $@"DELETE FROM todo_items WHERE check_done = true;";

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
