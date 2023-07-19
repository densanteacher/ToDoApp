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

        public ToDoEditWindow(int id)
        {
            this.InitializeComponent();


            _id = id;
            this.GetRowColumns();
        }

        /// <summary>
        /// idに対応する行をSQLから取得し、表示します。
        /// </summary>
        private void GetRowColumns()
        {
            var sql = $@"SELECT * FROM todo_items WHERE id = {_id}";
            var Connection = new NpgsqlConnection(Constants.ConnectionString);
            using (var command = new NpgsqlCommand(sql, Connection))
            {

                // 接続開始
                Connection.Open();

                // sql実行
                using (var reader = command.ExecuteReader())
                {
                    try
                    {
                        if (reader.Read())
                        {

                            this.CheckDone.IsChecked = (bool)reader["check_done"];
                            this.ToDoTitle.Text = (string)reader["title"];
                            this.DateEnd.Text = reader["date_end"].ToString();
                            this.Memo.Text = (string)reader["memo"];
                        }
                    }
                    finally
                    {

                        Connection.Close();
                    }
                }
            }
        }


        /// <summary>
        /// 変更を保存ボタンをクリックすると、現在テキストボックスなどに入力されている内容に応じてSQLにUPDATEを実行します。
        /// その後、ToDoEditWindowを閉じます。        /// </summary>
        private void ChangeButton_Click(object sender, RoutedEventArgs e)
        {
            var sql = $@"UPDATE todo_items SET check_done={this.CheckDone.IsChecked},
                                            title='{this.ToDoTitle.Text}',
                                            date_end='{this.DateEnd.SelectedDate.Value}',
                                            memo='{this.Memo.Text}'
                                            WHERE id = {_id}";

            using (var connection = new NpgsqlConnection(Constants.ConnectionString))
            {
                connection.Open();
                var command = new NpgsqlCommand(sql, connection);
                var result = command.ExecuteNonQuery();
            }
            this.Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
