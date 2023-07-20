using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace ToDoApp2
{
    // TODO: ファイル名

    /// <summary>
    /// SQLから取得したデータを保存しておくクラスです。
    /// </summary>
    public class DataItem
    {
        // TODO: コメント、PK の場合は明記しましょう。
        public int Id { get; set; }
        public bool CheckDone { get; set; }
        public string ToDoTitle { get; set; }
        // TODO: null許容なら、nullable にしましょう。
        public DateTime DateStart { get; set; }
        public DateTime DateEnd { get; set; }
        public string Memo { get; set; }
        public int Priority { get; set; }
        public DateTime DateUpdate { get; set; }
        public PngBitmapDecoder image { get; set; }
        public bool Remind { get; set; }
        public DateTime RemindDate { get; set; }
<<<<<<< HEAD

        // TODO: コンストラクタで全項目を入れる必要はありません。絶対に必要な id だけがよさそうです。
        public DataItem(int id, bool checkDone, string title, string memo, DateTime dateStart, DateTime dateEnd, int priority)
=======
        public DataItem(int id, bool checkDone, string title, string memo, DateTime dateStart, DateTime dateEnd, int priority, DateTime dateUpdate)
>>>>>>> ce562f38d28d62a39f0fbbbeeb495756dac1df23
        {
            this.Id = id;
            this.CheckDone = checkDone;
            this.ToDoTitle = title;
            this.Memo = memo;
            this.DateStart = dateStart;
            this.DateEnd = dateEnd;
            this.Priority = priority;
            this.DateUpdate = dateUpdate;
        }
    }

}
