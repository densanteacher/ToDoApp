﻿using Npgsql;
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

namespace ToDoApp2;

/// <summary>
/// MainWindow.xaml の相互作用ロジック
/// </summary>
public partial class MainWindow : Window
{
    // TODO: コメントだけを読んで何をするか説明されているのが理想です。ここではSQLが何なのか読み取れません。
    /// <summary>
    /// <see cref="MainWindow"/>から渡されたSQLの読み取り結果を保持しておくリストです。
    /// </summary>
    private readonly List<DataItem> _items = new();

    public MainWindow()
    {
        this.InitializeComponent();

        this.ReloadToDoList();
    }

    #region DataGrid関連処理

    // TODO: Reload → Load でいいのでは？ Re は繰り返す処理につけたくなりますが、この場合はなくても伝わりそうです。
    /// <summary>
    /// <see cref="ToDoDataGrid"/>にToDoリストを表示します。
    /// </summary>
    private void ReloadToDoList()
    {
        var selectedItem = this.ToDoDataGrid.SelectedItem;
        this.ToDoDataGrid.ItemsSource = null;
        this.ToDoDataGrid.Items.Clear();

        this._items.Clear();

        this.ToDoDataGrid.Items.Refresh();

        #region SQL文

        var sql = $@"
SELECT
    id
  , check_done
  , title
  , memo
  , date_start
  , date_end
  , priority
  , date_update
FROM
    todo_items
ORDER BY
    check_done
  , priority DESC
  , date_end
;
";

        #endregion SQL文

        // TODO: この中括弧は不要でしょう。
        {
            using IDbConnection conn = new NpgsqlConnection(Constants.ConnectionString);
            // TODO: conn.CreateCommand() というものがあります。キャストは不要です。
            using IDbCommand command = new NpgsqlCommand(sql, (NpgsqlConnection)conn);

            try
            {
                conn.Open();
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message);
            }

            // TODO: try-catch の付け方があまり効果的ではありません。もう少し全体を見て、例外のフローを考えてみましょう。
            try
            {
                using var reader = command.ExecuteReader();

                try
                {
                    while (reader.Read())
                    {
                        var item = new DataItem(reader.GetInt32(0));
                        item.SetValues(
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

        }
    }

    private int SearchIdentification()
    {
        if (this.ToDoDataGrid.SelectedItem is null)
        {
            return -1;
        }

        this.ToDoDataGrid.ScrollIntoView(this.ToDoDataGrid.SelectedItem);

        if (this.ToDoDataGrid.SelectedItem is not DataItem item)
        {
            return -1;
        }

        // TODO: item に変換してるので、id は取れるはずです。
        if (this.ToDoDataGrid.Columns[0].GetCellContent(item) is not TextBlock cell)
        {
            return -1;
        }

        // TODO: var
        string selectedId = cell?.Text;

        if (!(int.TryParse(selectedId, out var id)))
        {
            return -1;
        }
        return id;
    }

    #endregion DataGrid関連処理

    #region Clickイベント

    /// <summary>
    /// ToDo入力ボタンを押すと、<see cref="ToDoInputWindow"/>を表示します。
    /// </summary>
    private void InputToDo_Click(object sender, RoutedEventArgs e)
    {
        var window = new ToDoInputWindow
        {
            Owner = this
        };
        window.ShowDialog();

        this.ReloadToDoList();
    }

    /// <summary>
    /// 詳細ボタンをクリックすることで、<see cref="ToDoDataGrid"/>で選択している行の<see cref="ToDoEditWindow"/>を呼び出します。
    /// </summary>
    private void DetailButton_Click(object sender, RoutedEventArgs e)
    {
        var id = this.SearchIdentification();

        if (id == -1)
        {
            return;
        }
        int row = this.ToDoDataGrid.Items.IndexOf(this.ToDoDataGrid.SelectedItem);
        var window = new ToDoEditWindow(id, this._items[row])
        {
            Owner = this
        };
        window.ShowDialog();

        this.ReloadToDoList();
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
        int row = this.ToDoDataGrid.Items.IndexOf(this.ToDoDataGrid.SelectedItem);
        var priority = this._items[row].Priority;
        if (priority < 5)
        {
            this._items[row].Priority++;
        }
        this.ReloadToDoList();
    }

    private void PriorityDownButton_Click(object obj, RoutedEventArgs e)
    {
        int row = this.ToDoDataGrid.Items.IndexOf(this.ToDoDataGrid.SelectedItem);
        var priority = this._items[row].Priority;
        if (priority > -5)
        {
            this._items[row].Priority--;
        }
        this.ReloadToDoList();
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

