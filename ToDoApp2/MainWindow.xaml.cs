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

namespace ToDoApp2;

/// <summary>
/// MainWindow.xaml の相互作用ロジック
/// </summary>
public partial class MainWindow : Window
{
    /// <summary>
    /// 読み取ったToDoリストを保持しておくリストです。
    /// </summary>
    private readonly List<ToDoDataItem> _items = new();


    public MainWindow()
    {
        this.InitializeComponent();

        this.LoadToDoList();
    }

    #region DataGrid関連処理

    /// <summary>
    /// <see cref="ToDoDataGrid"/>にToDoリストを表示します。
    /// </summary>
    private void LoadToDoList()
    {
        if (_isChanged)
        {
            UpdateToDoItems();
        }
        this.ToDoDataGrid.ItemsSource = null;
        this.ToDoDataGrid.Items.Clear();

        this._items.Clear();

        this.ToDoDataGrid.Items.Refresh();

        #region SQL文

        var sql = $@"
SELECT
    id
  , is_finished
  , title
  , memo
  , date_start
  , date_end
  , priority
  , updated_at
FROM
    todo_items
ORDER BY
    is_finished
  , priority DESC
  , date_end
;
";

        #endregion SQL文

        using IDbConnection conn = new NpgsqlConnection(Constants.ConnectionString);
        using IDbCommand command = conn.CreateCommand();
        command.CommandText = sql;
        command.CommandTimeout = 15;

        try
        {
            conn.Open();
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, ex.Message);
        }

        try
        {
            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                var item = new ToDoDataItem(reader.GetInt32(0));
                item.SetDataItem(
                    reader.GetBoolean(1),
                    reader.GetString(2),
                    reader.GetString(3),
                    reader.GetDateTime(4),
                    reader.GetDateTime(5),
                    reader.GetInt32(6),
                    reader.GetDateTime(7));
                this._items.Add(item);
            }

            this.ToDoDataGrid.ItemsSource = this._items;
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, ex.Message);
        }
    }

    // DONE: Identification は、省略形の Id の方が通りがよいでしょう。
    // Id はすでに一般的に使われていますので問題ありません。
    // Db も大丈夫です。
    // IO も大丈夫です。Input/Output で IO ですが、2文字の省略形として Io とされることもあります。
    // 表記に迷ったらプロジェクト内で多く使われている方に寄せましょう。
    /// <summary>
    /// <see cref="ToDoDataGrid"/>にて選択された行のIDの値を取得します。
    /// </summary>
    private int? SearchId()
    {
        if (this.ToDoDataGrid.SelectedItem is null)
        {
            return null;
        }

        this.ToDoDataGrid.ScrollIntoView(this.ToDoDataGrid.SelectedItem);

        if (this.ToDoDataGrid.SelectedItem is not ToDoDataItem item)
        {
            return null;
        }

        return item.Id;
    }


    /// <summary>
    /// (WIP)<see cref="_items"/>の内容を元にデータベースの更新を行います。
    /// </summary>
    private void UpdateToDoItems()
    {
        var sql = $@"
UPDATE todo_items SET
    is_finished = {}
  , priority = {}
  , updated_at = current_timestamp
WHERE
    id = {}
";
        using IDbConnection conn = new NpgsqlConnection(Constants.ConnectionString);
        using var command = conn.CreateCommand();
        command.CommandText = sql;
        command.CommandTimeout = 15;

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

        this.LoadToDoList();
    }

    /// <summary>
    /// 詳細ボタンをクリックすることで、<see cref="ToDoDataGrid"/>で選択している行の<see cref="ToDoEditWindow"/>を呼び出します。
    /// </summary>
    private void DetailButton_Click(object sender, RoutedEventArgs e)
    {
        var id = this.SearchId();
        if (id is null)
        {
            return;
        }

        var row = this.ToDoDataGrid.Items.IndexOf(this.ToDoDataGrid.SelectedItem);
        var window = new ToDoEditWindow((int)id, this._items[row])
        {
            Owner = this
        };
        window.ShowDialog();

        this.LoadToDoList();
    }


    /// <summary>
    /// 更新ボタンを押すと、ToDoリストを再読み込みします。
    /// </summary>
    private void RefreshButton_Click(object sender, RoutedEventArgs e)
    {
        this.LoadToDoList();
    }

    /// <summary>
    /// 削除ボタンを押すと、<see cref="ToDoDataGrid"/>で選択されているToDoを削除します。
    /// </summary>
    private void DeleteButton_Click(object sender, RoutedEventArgs e)
    {
        var id = this.SearchId();
        if (id is null)
        {
            return;
        }

        var sql = $@"DELETE FROM todo_items WHERE id = {id};";

        {
            using IDbConnection conn = new NpgsqlConnection(Constants.ConnectionString);
            using var command = conn.CreateCommand();
            command.CommandText = sql;
            command.CommandTimeout = 15;
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

        this.LoadToDoList();
    }

    /// <summary>
    /// 一括削除ボタンを押すと、実行済みのToDoリストを全て削除します。
    /// </summary>
    private void BulkDeleteButton_Click(object sender, RoutedEventArgs e)
    {
        MessageBox.Show(this, "実行済みのToDoリストを全て削除します。よろしいですか？", "確認", MessageBoxButton.OKCancel, MessageBoxImage.Question);

        var sql = $@"DELETE FROM todo_items WHERE is_finished = true;";

        {
            using IDbConnection conn = new NpgsqlConnection(Constants.ConnectionString);
            using var command = conn.CreateCommand();
            command.CommandText = sql;
            command.CommandTimeout = 15;

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

        this.LoadToDoList();
    }

    /// <summary>
    /// 優先度↑ボタンを押すと、<see cref="ToDoDataGrid"/>で選択されている行の優先度を上げます。
    /// </summary>
    private void PriorityUpButton_Click(object sender, RoutedEventArgs e)
    {
        var row = this.ToDoDataGrid.Items.IndexOf(this.ToDoDataGrid.SelectedItem);
        var priority = this._items[row].Priority;
        if (priority < 5)
        {
            priority++;
        }
        var sql = $@"
UPDATE todo_items SET
    priority = {priority}
  , updated_at = current_timestamp
WHERE
    id = {this._items[row].Id}
";
        using IDbConnection conn = new NpgsqlConnection(Constants.ConnectionString);
        using var command = conn.CreateCommand();
        command.CommandText = sql;
        command.CommandTimeout = 15;

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
        this.LoadToDoList();
        this.ToDoDataGrid.SelectedIndex = row;

    }

    /// <summary>
    /// 優先度↓ボタンを押すと、<see cref="ToDoDataGrid"/>で選択されている行の優先度を下げます。
    /// </summary>
    private void PriorityDownButton_Click(object obj, RoutedEventArgs e)
    {
        var row = this.ToDoDataGrid.Items.IndexOf(this.ToDoDataGrid.SelectedItem);
        
        var priority = this._items[row].Priority;
        if (priority > -5)
        {
            priority--;
        }
        var sql = $@"
UPDATE todo_items SET
    priority = {priority}
  , updated_at = current_timestamp
WHERE
    id = {this._items[row].Id}
";

        using IDbConnection conn = new NpgsqlConnection(Constants.ConnectionString);
        using var command = conn.CreateCommand();
        command.CommandText = sql;
        command.CommandTimeout = 15;

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
        this.LoadToDoList();
        this.ToDoDataGrid.SelectedIndex = row;
    }

    private void CheckBox_Checked(object sender, RoutedEventArgs e)
    {
        var row = this.ToDoDataGrid.Items.IndexOf(this.ToDoDataGrid.SelectedItem);

        _items[row].IsFinished = 
    }
    private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
    {
        throw new NotImplementedException();
    }

    #endregion Clickイベント
}

