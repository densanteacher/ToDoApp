using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
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
using static Dropbox.Api.Files.ListRevisionsMode;

namespace ToDoApp2;

/// <summary>
/// MainWindow.xaml の相互作用ロジック
/// </summary>
public partial class MainWindow : Window
{
    /// <summary>
    /// 読み取ったToDoリストを保持しておくリストです。
    /// </summary>
    private readonly List<ToDoData> _items = new();

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
        this.ToDoDataGrid.ItemsSource = null;
        this.ToDoDataGrid.Items.Clear();

        this._items.Clear();

        this.ToDoDataGrid.Items.Refresh();

        #region SQL文

        // TODO: DB関連の処理を別クラスにしたので、ToDoDataControllerクラスを作成して、そちらに処理をまとめてみましょう。
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
  , image_path
FROM
    todo_items
WHERE
    is_deleted = false
ORDER BY
    is_finished
  , priority DESC
  , date_end
;
";

        #endregion SQL文

        try
        {
            using var reader = DbConnect.ExecuteSqlReader(sql);

            if (reader is null)
            {
                return;
            }

            while (reader.Read())
            {
                var id = reader.GetInt32(0);
                var isFinished = reader.GetBoolean(1);
                var title = reader.GetString(2);
                var memo = reader.GetString(3);
                var dateStart = reader.GetDateTime(4);
                var dateEnd = reader.GetDateTime(5);
                var priority = reader.GetInt32(6);
                var updateAt = reader.GetDateTime(7);
                var imagePath = reader.GetString(8) ?? "";

                var item = new ToDoData(id);
                item.Set(
                    isFinished,
                    title,
                    memo,
                    dateStart,
                    dateEnd,
                    priority,
                    updateAt,
                    imagePath);
                this._items.Add(item);
            }

            this.ToDoDataGrid.ItemsSource = this._items;
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, ex.Message);
        }
    }

    /// <summary>
    /// <see cref="ToDoDataGrid"/>にて選択された行のIDの値を取得します。
    /// </summary>
    private int? SearchId()
    {
        // DONE: is を有効に使ってみました。
        var selectedItem = this.ToDoDataGrid.SelectedItem;
        if (selectedItem is not ToDoData todoData)
        {
            return null;
        }

        this.ToDoDataGrid.ScrollIntoView(todoData);
        return todoData.Id;
    }

    /// <summary>
    /// <see cref="_items"/>の内容を元にデータベースの更新を行います。
    /// </summary>
    private void UpdateToDoItem(int row)
    {
        var item = this._items[row];
        var sql = $@"
UPDATE todo_items SET
    is_finished = @IS_FINISHED
  , updated_at = @UPDATED_AT
WHERE
    id = @ID
;";

        using var command = DbConnect.PrepareSqlCommand(sql);

        command.Parameters.AddWithValue("@IS_FINISHED", item.IsFinished);
        command.Parameters.AddWithValue("@UPDATED_AT", DateTime.Now);
        command.Parameters.AddWithValue("@ID", item.Id);
    }

    /// <summary>
    /// (WIP)<see cref="_items"/>の中で内容が変更されているものを、データベース上で更新します。
    /// </summary>
    private void UpdateIsChanged()
    {
        var changedItems = this._items.Where(x => x.IsChanged);
        foreach (var item in changedItems)
        {
            this.UpdateToDoItem(item.Id);
        }
    }


    #endregion DataGrid関連処理

    #region Clickイベント

    /// <summary>
    /// ToDo入力ボタンを押すと、<see cref="ToDoInputWindow"/>を表示します。
    /// </summary>
    private void InputToDo_Click(object sender, RoutedEventArgs e)
    {
        var maxId = this._items.Any()
            ? _items.Max(x => x.Id)
            : 0;
        var window = new ToDoInputWindow(maxId)
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
        this.UpdateIsChanged();

        var id = this.SearchId();
        if (id is null)
        {
            return;
        }

        var row = this.ToDoDataGrid.Items.IndexOf(this.ToDoDataGrid.SelectedItem);
        var window = new ToDoEditWindow(this._items[row])
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

        var sql = $@"UPDATE todo_items SET is_deleted = true WHERE id = @ID;";

        using var command = DbConnect.PrepareSqlCommand(sql);

        command.Parameters.AddWithValue("@ID", id);

        this.LoadToDoList();
    }

    /// <summary>
    /// 一括削除ボタンを押すと、実行済みのToDoリストを全て削除します。
    /// </summary>
    private void BulkDeleteButton_Click(object sender, RoutedEventArgs e)
    {
        MessageBox.Show(this, "実行済みのToDoリストを全て削除します。よろしいですか？", "確認", MessageBoxButton.OKCancel, MessageBoxImage.Question);

        var sql = $@"UPDATE todo_items SET is_deleted = true WHERE is_finished = true";

        DbConnect.ExecuteSqlCommand(sql);

        this.LoadToDoList();
    }

    /// <summary>
    /// 優先度↑ボタンを押すと、<see cref="ToDoDataGrid"/>で選択されている行の優先度を上げます。
    /// </summary>
    private void PriorityUpButton_Click(object sender, RoutedEventArgs e)
    {
        var row = this.ToDoDataGrid.Items.IndexOf(this.ToDoDataGrid.SelectedItem);
        var priority = this._items[row].Priority;
        if (priority < Constants.Priorities.Max())
        {
            priority++;
        }

        DbConnect.UpdatePriority(priority, this._items[row].Id);

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
        if (priority > Constants.Priorities.Min())
        {
            priority--;
        }

        DbConnect.UpdatePriority(priority, this._items[row].Id);

        this.LoadToDoList();
        this.ToDoDataGrid.SelectedIndex = row;
    }

    #endregion Clickイベント

    /// <summary>
    /// 完了ボタンを押すと、<see cref="ToDoDataGrid"/>で選択した値が完了状態になります。
    /// </summary>
    private void FinishButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            var row = this.ToDoDataGrid.Items.IndexOf(this.ToDoDataGrid.SelectedItem);
            var id = this._items[row].Id;

            DbConnect.FinishToDoCommand(id);

            this.LoadToDoList();
            this.ToDoDataGrid.SelectedIndex = row;
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, ex.Message);
        }
    }

    /// <summary>
    /// 一括完了ボタンを押すと、<see cref="ToDoDataGrid"/>で選択した複数の行を一括で完了状態にします。
    /// </summary>
    private void BulkFinishButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            foreach (var selectedItem in this.ToDoDataGrid.SelectedItems)
            {
                var row = this.ToDoDataGrid.Items.IndexOf(selectedItem);

                DbConnect.FinishToDoCommand(this._items[row].Id);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, ex.Message);
        }
        this.LoadToDoList();
    }
}

