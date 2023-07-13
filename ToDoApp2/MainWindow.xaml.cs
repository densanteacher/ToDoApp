using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
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
        public MainWindow()
        {
            InitializeComponent();

            this.ShowToDoList();
        }

        private void ClickInputToDo(object sender, RoutedEventArgs e)
        {
            ToDoInputWindow tdw = new ToDoInputWindow();
            tdw.Owner = this;
            tdw.ShowDialog();
        }

        private void ShowToDoList()
        {
            string ConnectionString =
               "Server=127.0.0.1;"
               + "Port=5432;"
               + "Database=todoapp_db;"
               + "User ID=postgres;"
               + "Password=postgres;";
            string sql = " SELECT * FROM todo_items";
            var Connection = new NpgsqlConnection(ConnectionString);
            using (var command = new NpgsqlCommand(sql, Connection))
            {
                // 接続開始
                Connection.Open();

                // sql実行
                using (var reader = command.ExecuteReader())
                {
                    try
                    {
                        while (reader.Read())
                        {
                            this.test.Children.Add(new TextBlock() { Text = $"{reader["title"]}" });

                        }
                    }
                    finally
                    {

                        Connection.Close();
                    }
                }
            }
        }
    }
}
