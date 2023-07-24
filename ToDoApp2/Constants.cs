using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ToDoApp2
{
    public static class Constants
    {
        // DONE: const → static readonly
        // TODO: 命名規則はMSDNを参照するとよいです。
        // https://learn.microsoft.com/ja-jp/dotnet/standard/design-guidelines/naming-guidelines
        public static readonly string ConnectionString =
               @"Server=127.0.0.1;
               Port=5432;
               Database=todoapp_db;
               User ID=postgres;
               Password=postgres;";


        public static readonly List<string> fileExtensions = new List<string>()
        {
            ".bmp",
            ".jpg",
            ".png",
            ".tiff",
            ".gif",
            ".icon",
            ".webp"
        };
    }

}