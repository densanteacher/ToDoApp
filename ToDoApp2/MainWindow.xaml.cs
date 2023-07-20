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
        private readonly List<DataItem> items = new List<DataItem>();
        public MainWindow()
        {
            InitializeComponent();

            this.ShowToDoList();
        }

        /// <summary>
        /// ToDo入力ボタンを押すと、ToDoInputWindowを表示します。
        /// </summary>
        private void InputToDo_Click(object sender, RoutedEventArgs e)
        {
            var tdiw = new ToDoInputWindow();
            tdiw.Owner = this;
            tdiw.ShowDialog();
        }

        /// <summary>
        ///
        /// </summary>
        private void ShowToDoList()
        {


            var sql = " SELECT * FROM todo_items ORDER BY check_done, date_end";

            var Connection = new NpgsqlConnection(Constants.connectionString);
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
                            items.Add(new DataItem((int)reader["id"],
                                                (bool)reader["check_done"],
                                                (string)reader["title"],
                                                (string)reader["memo"],
                                                (DateTime)reader["date_start"],
                                                (DateTime)reader["date_end"],
                                                (int)reader["priority"],
                                                (DateTime)reader["date_update"]));
                        }
                        this.ToDoDataGrid.ItemsSource = items;
                    }
                    finally
                    {

                        Connection.Close();
                    }
                }
            }

        }

        // DONE: クラス名は基本的には複数形にはしません。
        // DONE: DataItem と同じなので、どちらかにしましょう。
        // DONE: 別ファイルに分離しましょう。


        /// <summary>
        /// 詳細ボタンをクリックすることで、DataGridで選択している行のToDoEditWindowを呼び出します。
        /// </summary>
        private void DetailButton_Click(object sender, RoutedEventArgs e)
        {
            this.ToDoDataGrid.UpdateLayout();
            this.ToDoDataGrid.ScrollIntoView(ToDoDataGrid.Columns[0]);

            var test = (TextBlock)this.ToDoDataGrid.Columns[0].GetCellContent(this.ToDoDataGrid.SelectedItem);
            string selectedRow = test?.Text;
            var isSuccess = int.TryParse(selectedRow, out var id);
            if (!isSuccess) return;

            var tdew = new ToDoEditWindow(id,items);
            tdew.Owner = this;
            tdew.ShowDialog();
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            ToDoDataGrid.Items.Refresh();
            this.ToDoDataGrid.UpdateLayout();
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
