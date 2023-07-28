﻿using Npgsql;
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

namespace ToDoApp2;

/// <summary>
/// ToDoタスクの詳細表示・編集を行うウィンドウです。
/// </summary>
public partial class ToDoEditWindow : Window
{
    /// <summary>
    /// 表示する際に渡されたToDoリストのデータベースにおけるIDです。
    /// </summary>
    private readonly int _id;

    /// <summary>
    /// ウィンドウ呼び出し時に渡されたToDoリストの読み取り結果です。
    /// </summary>
    private readonly ToDoData _item;

    public ToDoEditWindow(int id, ToDoData item)
    {
        this.InitializeComponent();

        // TODO: ToDoData は既にidを持っているはずです。
        this._id = id;
        this._item = item;

        this.PriorityComboBox.ItemsSource = Constants.Priorities;

        this.SetValues(this._item);
    }

    /// <summary>
    /// ウィンドウ呼び出し時に渡された値を表示します。
    /// </summary>
    private void SetValues(ToDoData item)
    {
        this.IsFinished.IsChecked = item.IsFinished;
        this.ToDoTitle.Text = item.ToDoTitle;
        this.DateEnd.Text = item.DateEnd.ToString();
        this.Memo.Text = item.Memo;
        this.PriorityComboBox.Text = item.Priority.ToString();
    }

    /// <summary>
    /// 「変更を保存」ボタンをクリックすると、選択されたToDoの内容を更新します。
    /// その後、ToDoEditWindowを閉じます。
    /// </summary>
    private void ChangeButton_Click(object sender, RoutedEventArgs e)
    {
        this.UpdateToDoItem();

        this.Close();
    }

    /// <summary>
    /// Cancelボタンを押すと、ウィンドウを閉じます。
    /// </summary>
    private void CancelButton_Click(object sender, RoutedEventArgs e)
    {
        this.Close();
    }

    /// <summary>
    /// UPDATEコマンドを用いて、現在テキストボックスなどに入力されている内容に応じてToDoの内容を更新します。
    /// </summary>
    private void UpdateToDoItem()
    {
        try
        {
            #region SQL文

            var isFinished = this.IsFinished.IsChecked ?? false;
            var dateEnd = this.DateEnd.SelectedDate.Value;
            if (!(int.TryParse(this.PriorityComboBox.Text, out var priority)))
            {
                return;
            }
            var sql = $@"
UPDATE todo_items SET
    is_finished = {isFinished}
  , title = '{this.ToDoTitle.Text}'
  , date_end = '{dateEnd}'
  , memo = '{this.Memo.Text}'
  , priority = {priority}
  , updated_at = current_timestamp
WHERE
    id = {this._id}
";

            #endregion SQL文

            using IDbConnection conn = new NpgsqlConnection(Constants.ConnectionString);

            try
            {
                conn.Open();
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message);
            }

            using var command = conn.CreateCommand();
            command.CommandText = sql;
            // TODO: 同じ値を変更するのに複数箇所を変えたはずです。Constantsに変数を定義してみましょう。
            // 自分で同じコードを書いてるな、同じものを変更しているなって思ったら共通化を検討してみましょう。
            // 必ず共通化しなければならないわけではありません。しないほうがよい場合もあります。
            command.CommandTimeout = 5;
            var result = command.ExecuteNonQuery();
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, ex.Message);
        }
    }
}
