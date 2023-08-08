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

        /// <summary>
        /// Dropboxのアクセストークンです。
        /// </summary>
        public static readonly string DropboxAccessToken = "sl.Bjtw40N44DR-x-FKky-OiJtS2vY6lRni1AO3PGYLASPsmr4xfYGXFtzICYvM9MG2C8hcFK31dzYWcDgDqCAp5twvkaJiBcNo_jj3OfZnghv9Zqz7ndi4jgfDoQUCSd80dBAfpiogHy93KW9GXmf18Mg";

        public static readonly string DropboxImagePath = "/ToDoItems/";

        public static readonly string DropboxImageName = "ToDoItem";

        /// <summary>
        /// データベースと接続する際のタイムアウトまでの時間（秒）です。
        /// </summary>
        public static readonly int TimeoutSecond = 5;

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