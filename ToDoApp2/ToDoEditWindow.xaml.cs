using Npgsql;
using System;
using System.Collections.Generic;
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
        public ToDoEditWindow(int id)
        {
            this.InitializeComponent();

            this.GetRowColumns(id);
        }
        private void GetRowColumns(int id)
        {
            var sql = $@"SELECT * FROM todo_items WHERE id = {id}";
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
                            this.DataContext = new ToDoItems( check_done = true,
                            title = (string)reader["title"],
                            memo = (string)reader["memo"],
                            date_end = (DateTime)reader["date"]
                            );
                        }
                    }
                    finally
                {

                    Connection.Close();
                }
            }
        }
    }

    public class ToDoItems
    {
        public ToDoItems( bool check_done, string title, string memo, DateTime date_end)
        {
            this.check_done = check_done;
            this.title = title;
            this.memo = memo;
            this.date_end = date_end;
        }
        public bool check_done { get; set; }
        public string title { get; set; }
        public string memo { get; set; }
        public DateTime date_end { get; set; }
    }
}
}
