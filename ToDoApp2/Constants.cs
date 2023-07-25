using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ToDoApp2
{
    public static class Constants
    {
        // TODO: コメント
        public static readonly string ConnectionString =
               @"Server=127.0.0.1;
               Port=5432;
               Database=todoapp_db;
               User ID=postgres;
               Password=postgres;";

        // TODO: new() という表記にできます。
        /// <summary>
        /// 画像ファイルの拡張子をまとめたリストです。
        /// </summary>
        public static readonly List<string> FileExtensions = new List<string>()
        {
            ".bmp",
            ".jpg",
            ".png",
            ".tiff",
            ".gif",
            ".icon",
            ".webp",
        };

        // TODO: コメント
        // TODO: new() という表記にできます。
        // TODO: Dataは冗長だと思います。Priorityの複数形でよいのでは？
        public static readonly List<int> PriorityDataList = new List<int>() { 5, 4, 3, 2, 1, 0, -1, -2, -3, -4, -5 };
    }

}