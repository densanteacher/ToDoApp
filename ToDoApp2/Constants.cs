using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ToDoApp2
{
    public static class Constants
    {
        // TODO: 命名規則はMSDNを参照するとよいです。
        // https://learn.microsoft.com/ja-jp/dotnet/standard/design-guidelines/naming-guidelines
        public static readonly string ConnectionString =
               @"Server=127.0.0.1;
               Port=5432;
               Database=todoapp_db;
               User ID=postgres;
               Password=postgres;";

        // TODO: コメント
        // TODO: static readonly にしたら、パスカルケースです。
        public static readonly List<string> fileExtensions = new List<string>()
        {
            ".bmp",
            ".jpg",
            ".png",
            ".tiff",
            ".gif",
            ".icon",
            // TODO: ちょっとしたテクニックなのですが、リストの項目を定義する場合は、最後のカンマも記述します。
            // そうすることで、次の行をコピーして作るときに、追加が楽になります。
            // 最後のカンマがなければ、次の行を追加して、戻ってカンマを付ける必要があるので、ちょっとだけ手間です。
            ".webp"
        };
    }

}