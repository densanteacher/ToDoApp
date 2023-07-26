using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ToDoApp2
{
    public static class Constants
    {
        // DONE: コメント
        /// <summary>
        /// SQLサーバーと接続するための情報をまとめた文字列です。
        /// </summary>
        public static readonly string ConnectionString =
               @"Server=127.0.0.1;
               Port=5432;
               Database=todoapp_db;
               User ID=postgres;
               Password=postgres;";

        // DONE: new() という表記にできます。
        /// <summary>
        /// 画像ファイルの拡張子をまとめたリストです。
        /// </summary>
        public static readonly List<string> FileExtensions = new()
        {
            ".bmp",
            ".jpg",
            ".png",
            ".tiff",
            ".gif",
            ".icon",
            ".webp",
        };

        // DONE: コメント
        // DONE: new() という表記にできます。
        // DONE: Dataは冗長だと思います。Priorityの複数形でよいのでは？
        /// <summary>
        /// ToDoリストの優先度の範囲を示したリストです。
        /// </summary>
        public static readonly List<int> Priorities = new() { 5, 4, 3, 2, 1, 0, -1, -2, -3, -4, -5 };
    }

}