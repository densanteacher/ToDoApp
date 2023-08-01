using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ToDoApp2
{
    public static class Constants
    {
        /// <summary>
        /// SQLサーバーと接続するための情報をまとめた文字列です。
        /// </summary>
        public static readonly string ConnectionString =
               @"Server=127.0.0.1;
               Port=5432;
               Database=todoapp_db;
               User ID=postgres;
               Password=postgres;";

        // TODO: コメント
        // TODO: パスカルケース
        // TODO: 変数名に単位を含めるとわかりやすくなります。
        // sec, ms, min なのか迷うので、明示してあげるとよいでしょう。
        // 合わせてコメントにも単位を記述してあげてください。
        public static readonly int timeout = 5;

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

        /// <summary>
        /// ToDoリストの優先度の範囲を示したリストです。
        /// </summary>
        public static readonly List<int> Priorities = new() { 5, 4, 3, 2, 1, 0, -1, -2, -3, -4, -5 };
    }

}