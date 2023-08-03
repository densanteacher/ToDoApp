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
        // DONE: Loadという処理の中にSave(Update)という動作が入っているのはよくありません。

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
        command.CommandTimeout = Constants.TimeoutSecond;

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
                // DONE: 少し冗長になりますが、reader.GetXxx() したものは
                // 一度変数に入れて名前をつけた方が読む人に優しいです。
                var id         = reader.GetInt32(0);
                var isFinished = reader.GetBoolean(1);
                var title      = reader.GetString(2);
                var memo       = reader.GetString(3);
                var dateStart  = reader.GetDateTime(4);
                var dateEnd    = reader.GetDateTime(5);
                var priority   = reader.GetInt32(6);
                var updateAt   = reader.GetDateTime(7);

                var item = new ToDoData(id);
                item.SetToDoData(
                    isFinished,
                    title,
                    memo,
                    dateStart,
                    dateEnd,
                    priority,
                    updateAt);
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
        // TODO: このメソッドの表記はもっとシンプルにできます。
        var selectedItem = this.ToDoDataGrid.SelectedItem;
        if (selectedItem is null)
        {
            return null;
        }


        this.ToDoDataGrid.ScrollIntoView(selectedItem);

        if (selectedItem is not ToDoData item)
        {
            return null;
        }

        return item.Id;
    }

    /// <summary>
    /// <see cref="_items"/>の内容を元にデータベースの更新を行います。
    /// </summary>
    private void UpdateToDoItem(int row)
    {
        var item = this._items[row];
        // DONE: _items[row] は変数にしてしまいましょう。
        // DONE: id の参照が違います。
        var sql = $@"
UPDATE todo_items SET
    is_finished = {item.IsFinished}
  , updated_at = current_timestamp
WHERE
    id = {item.Id}
";


        this.ExecuteSqlCommand(sql);
    }

    /// <summary>
    /// (WIP)<see cref="_items"/>の中で内容が変更されているものを、データベース上で更新します。
    /// </summary>
    private void UpdateIsChanged()
    {
        // DONE: this
        // TODO: bool 型は true と比較しない
        // DONE: 配列の命名は複数形を用いる
        var changedItems = this._items.Where(x => x.IsChanged);
        foreach (var item in changedItems)
        {
            // DONE: this
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
        // DONE: this
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

        var sql = $@"DELETE FROM todo_items WHERE id = {id};";

        this.ExecuteSqlCommand(sql);

        this.LoadToDoList();
    }

    /// <summary>
    /// 一括削除ボタンを押すと、実行済みのToDoリストを全て削除します。
    /// </summary>
    private void BulkDeleteButton_Click(object sender, RoutedEventArgs e)
    {
        MessageBox.Show(this, "実行済みのToDoリストを全て削除します。よろしいですか？", "確認", MessageBoxButton.OKCancel, MessageBoxImage.Question);

        var sql = $@"DELETE FROM todo_items WHERE is_finished = true;";

        this.ExecuteSqlCommand(sql);

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
        var sql = $@"
UPDATE todo_items SET
    priority = {priority}
  , updated_at = current_timestamp
WHERE
    id = {this._items[row].Id}
";

        this.ExecuteSqlCommand(sql);

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
        var sql = $@"
UPDATE todo_items SET
    priority = {priority}
  , updated_at = current_timestamp
WHERE
    id = {this._items[row].Id}
";

        this.ExecuteSqlCommand(sql);
        this.LoadToDoList();
        this.ToDoDataGrid.SelectedIndex = row;
    }

    #endregion Clickイベント

    /// <summary>
    /// 完了ボタンを押すと、<see cref="ToDoDataGrid"/>で選択した値が完了状態になります。
    /// </summary>
    private void FinishButton_Click(object sender, RoutedEventArgs e)
    {
        var row = this.ToDoDataGrid.Items.IndexOf(this.ToDoDataGrid.SelectedItem);

        var sql = $@"
UPDATE todo_items SET
    is_finished = true
  , updated_at = current_timestamp
WHERE
    id = {this._items[row].Id}
";
        this.ExecuteSqlCommand(sql);

        this.LoadToDoList();
        this.ToDoDataGrid.SelectedIndex = row;
    }

    private void BulkFinishButton_Click(object sender, RoutedEventArgs e)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// SQLコマンドを実行します。
    /// </summary>
    /// <param name="sql">SQLコマンドです。</param>
    private void ExecuteSqlCommand(string sql)
    {
        using IDbConnection conn = new NpgsqlConnection(Constants.ConnectionString);
        using var command = conn.CreateCommand();
        command.CommandText = sql;
        command.CommandTimeout = Constants.TimeoutSecond;

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
            command.ExecuteNonQuery();
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, ex.Message);
        }
        finally
        {
            conn.Close();
        }
    }
}

