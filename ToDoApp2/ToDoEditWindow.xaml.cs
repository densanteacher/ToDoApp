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

            // TODO: Getという命名だと、戻り値があるイメージです。
            // また軽量な処理を想像しますので、DBアクセスするメソッドにGetは使わないほうがよいです。
            // Readは直接読み出すイメージ、Loadは読み出して乗せるイメージ、Fetchはサーバーから取得してくるイメージ・・・適切なメソッド名を考えてみましょう。
            this.GetRowColumns();
        }

        // TODO: アプリケーションの設計によりますが、ToDoEditWindowを開いた時点でDBから再取得すると、画面上に表示されている間に変更があった場合、差異ができて、あれ？となります。
        // 一人でアクセスする場合は問題になりませんが、複数人で扱う場合はこの方法だとまずそうです。
        // RDBMSとしてSQLiteを使うならまだしも、PostgreSQLを使う場合は、複数アクセス前提で考えておきましょう。
        // また複数人で更新が重なった場合は、後からの人で上書きされてしまうことが予想されます。
        // 対策方法を考えてみましょう。
        /// <summary>
        /// idに対応する行をSQLから取得し、表示します。
        /// </summary>
        private void GetRowColumns()
        {
            // TODO: _id はメソッドの引数として渡せるようにしましょう。
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
        /// その後、ToDoEditWindowを閉じます。
        /// </summary>
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
