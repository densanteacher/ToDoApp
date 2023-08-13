using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using static Dropbox.Api.Files.ListRevisionsMode;

namespace ToDoApp2
{
    // TODO: DbConnect は DbConnection と紛らわしいので変えたほうがよさそうです。
    // こういうDB関連クラスの場合は、DbClient, DbManager, DbHelper, etc... などが考えられます。
    internal class DbConnect
    {
        /// <summary>
        /// SQLコマンドを用意します。
        /// </summary>
        /// <param name="sql">SQL文です。</param>
        /// <returns>SQLコマンドです。</returns>
        public static NpgsqlCommand PrepareSqlCommand(string sql)
        {
            // TODO: conn は Close しなければならないので、どこかで保持しておく必要があります。
            // また、一回の接続(Open)で、複数のSQLを実行したい場合もあるでしょう。
            var conn = new NpgsqlConnection(Constants.ConnectionString);
            try
            {
                conn.Open();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            var command = new NpgsqlCommand(sql, conn);
            return command;
        }

        /// <summary>
        /// SQLコマンドを実行します。
        /// </summary>
        /// <param name="sql">SQL文です。</param>
        public static void ExecuteSqlCommand(string sql)
        {
            // TODO: DBのコネクションを作るところと、コマンドを作るところは、ある程度共通化できそうです。
            // 別システムのものを参考にしてよいので、DB用のクラスを用意してみましょう。
            // DIできるようになれば更に良いですが、まずは段階を踏むのがよいでしょう。
            using var command = DbConnect.PrepareSqlCommand(sql);

            try
            {
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// SQLコマンドを実行し、データベースからデータを読み取ります。
        /// </summary>
        /// <param name="sql">SQL文です。</param>
        /// <returns>読み取ったデータです。</returns>
        public static System.Data.IDataReader ExecuteSqlReader(string sql)
        {
            using var command = DbConnect.PrepareSqlCommand(sql);

            try
            {
                return command.ExecuteReader();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return null;
        }

        /// <summary>
        /// ToDoリストを完了状態にします。
        /// </summary>
        /// <param name="id">完了したいToDoリストのIDです。</param>
        public static void FinishToDoCommand(int id)
        {
            try
            {
                var sql = $@"
UPDATE todo_items SET
    is_finished = true
  , updated_at = @UPDATED_AT
WHERE
    id = @ID
;";
                using var command = DbConnect.PrepareSqlCommand(sql);

                command.Parameters.AddWithValue("@UPDATED_AT", DateTime.Now);
                command.Parameters.AddWithValue("@ID", id);

                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                MessageBox.Show( ex.Message);
            }
        }

        /// <summary>
        /// 優先度を更新します。
        /// </summary>
        /// <param name="id">更新したいToDoリストのIDです。</param>
        /// <param name="priority">優先度です。</param>
        public static void UpdatePriority(int id, int priority)
        {
            var sql = $@"
UPDATE todo_items SET
    priority = @PRIORITY
  , updated_at = @UPDATED_AT
WHERE
    id = @ID
;";

            using var command = DbConnect.PrepareSqlCommand(sql);

            command.Parameters.AddWithValue("@PRIORITY", priority);
            command.Parameters.AddWithValue("@UPDATED_AT", DateTime.Now);
            command.Parameters.AddWithValue("@ID", id);
        }
    }
}
