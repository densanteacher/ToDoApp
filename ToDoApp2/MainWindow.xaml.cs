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
    /// SQLから取得したデータを保存しておくクラスです。
    /// </summary>
    public class DataItem
    {
        public int Id { get; set; }
        public bool CheckDone { get; set; }
        public int Priority { get; set; }
        public string ToDoTitle {get; set;}
        public DateTime DateStart { get; set; }
        public DateTime DateEnd { get; set;}
        public string Memo { get; set; }
        public PngBitmapDecoder image { get; set; }
        public bool Remind { get; set; }
        public DateTime RemindDate { get; set; }
    }





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

        private void InputToDo_Click(object sender, RoutedEventArgs e)
        {
            var tdiw = new ToDoInputWindow();
            tdiw.Owner = this;
            tdiw.ShowDialog();
        }

        private void ShowToDoList()
        {
            var sql = " SELECT * FROM todo_items ORDER BY check_done, date_end";
            var items = new List<DataGridItems>();


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
                        while (reader.Read())
                        {
                            items.Add(new DataGridItems((int)reader["id"],(bool)reader["check_done"], (string)reader["title"], (string)reader["memo"], (DateTime)reader["date_end"]));
                        }
                        this.ToDoList.ItemsSource = items;
                    }
                    finally
                    {

                        Connection.Close();
                    }
                }
            }

        }
        public class DataGridItems
        {
            public DataGridItems(int id, bool check_done, string title, string memo, DateTime date_end)
            {
                this.id = id;
                this.check_done = check_done;
                this.title = title;
                this.memo = memo;
                this.date_end = date_end;
            }
            public int id { get; set; }
            public bool check_done { get; set; }
            public string title { get; set; }
            public string memo { get; set; }
            public DateTime date_end { get; set; }
        }

        /// <summary>
        /// 詳細ボタンをクリックすることで、DataGridで選択している行のToDoEditWindowを呼び出します。
        /// </summary>
        private void DetailButton_Click(object sender, RoutedEventArgs e)
        {
            this.ToDoList.UpdateLayout();
            this.ToDoList.ScrollIntoView(ToDoList.Columns[0]);

            var test = (TextBlock)this.ToDoList.Columns[0].GetCellContent(this.ToDoList.SelectedItem);
            string selectedRow = test?.Text;
            var isSuccess = int.TryParse(selectedRow, out var id);
            if (!isSuccess) return;

            var tdew = new ToDoEditWindow(id);
            tdew.Owner = this;
            tdew.ShowDialog();
        }
    }
}
