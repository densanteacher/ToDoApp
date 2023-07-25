using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace ToDoApp2
{
    // DONE: ファイル名

    /// <summary>
    /// SQLから取得したデータを保存しておくクラスです。
    /// </summary>
    public class DataItem
    {
        // DONE: コメント、PK の場合は明記しましょう。
        /// <summary>
        /// SQLから取得したIDです。プライマリキーです。
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// ToDoリストが実行済みであるかどうかを判定する値です。
        /// </summary>
        public bool CheckDone { get; set; }
        /// <summary>
        /// ToDoのタイトルです。
        /// </summary>
        public string ToDoTitle { get; set; }
        // DONE: null許容なら、nullable にしましょう。
        /// <summary>
        /// ToDoの開始日です。
        /// </summary>
        public DateTime? DateStart { get; set; }
        /// <summary>
        /// ToDoの終了日（期限）です。
        /// </summary>
        public DateTime DateEnd { get; set; }
        /// <summary>
        /// ToDoに関するメモ、備考欄です。
        /// </summary>
        public string Memo { get; set; }
        /// <summary>
        /// ToDoの優先度を判定する値です。
        /// </summary>
        public int Priority { get; set; }
        /// <summary>
        /// ToDoリストが更新された日時を保持しておく値です。
        /// </summary>
        public DateTime DateUpdate { get; set; }
        /// <summary>
        /// 画像ファイルです。
        /// </summary>
        public PngBitmapDecoder image { get; set; }
        /// <summary>
        /// ToDoリストをリマインドするかを指定する値です。
        /// </summary>
        public bool Remind { get; set; }
        /// <summary>
        /// リマインドを行う日時を指定する値です。
        /// </summary>
        public DateTime RemindDate { get; set; }

        // DONE: コンストラクタで全項目を入れる必要はありません。絶対に必要な id だけがよさそうです。

        public DataItem(int id)
        {
            this.Id = id;

        }
        public void SetValues(bool chackDone, string title, string memo, DateTime dateStart, DateTime dateEnd,int priority, DateTime dateUpdate)
        {
            this.CheckDone = chackDone;
            this.ToDoTitle = title;
            this.Memo = memo;
            this.DateStart = dateStart;
            this.DateEnd = dateEnd;
            this.Priority = priority;
            this.DateUpdate = dateUpdate;
        }
        public void SetValueString(string title, string memo)
        {
            this.ToDoTitle = title;
            this.Memo = memo;
        }
    }

}
