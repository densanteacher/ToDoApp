using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace ToDoApp2
{

    /// <summary>
    /// SQLから取得したデータを保存しておくクラスです。
    /// </summary>
    public class DataItem
    {
        public int Id { get; set; }
        public bool CheckDone { get; set; }
        public string ToDoTitle { get; set; }
        public DateTime DateStart { get; set; }
        public DateTime DateEnd { get; set; }
        public string Memo { get; set; }
        public int Priority { get; set; }
        public DateTime DateUpdate { get; set; }
        public PngBitmapDecoder image { get; set; }
        public bool Remind { get; set; }
        public DateTime RemindDate { get; set; }
        public DataItem(int id, bool checkDone, string title, string memo, DateTime dateStart, DateTime dateEnd, int priority, DateTime dateUpdate)
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
