using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

// TODO: namespace
namespace ToDoApp2
{
    // TODO: SQL とは何かを明示しましょう。
    /// <summary>
    /// SQLから取得したデータを保存しておくクラスです。
    /// </summary>
    public class DataItem
    {
        // TODO: 適切な改行を入れてください。
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
        /// <summary>
        /// ToDoの開始日です。
        /// </summary>
        public DateTime? DateStart { get; set; }

        // TODO: DateStart を nullable にしたらこちらも。DB側の日付列も忘れずに null 許容としましょう。
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

        // TODO: 更新日時を記録しておく列名として、よく updated_at という列名が使われます。日付の場合は updated_on となります。
        // これはたしかRuby言語の文化だったと思いますが、わかりやすいので是非使ってください。
        /// <summary>
        /// ToDoリストが更新された日時を保持しておく値です。
        /// </summary>
        public DateTime DateUpdate { get; set; }
        /// <summary>
        /// 画像ファイルです。
        /// </summary>
        public PngBitmapDecoder image { get; set; }

        // TODO: bool 側は is から始めましょう。DB側の列名にも is_ を使ってかまいません。
        /// <summary>
        /// ToDoリストをリマインドするかを指定する値です。
        /// </summary>
        public bool Remind { get; set; }

        /// <summary>
        /// リマインドを行う日時を指定する値です。
        /// </summary>
        public DateTime RemindDate { get; set; }

        public DataItem(int id)
        {
            this.Id = id;

        }

        // TODO: chackタイポ
        // TODO: 少し横に長いです。( の次と各 , の後ろで改行しましょう。
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
