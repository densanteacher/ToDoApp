using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ToDoApp2
{
    public static class Constants
    {
        // TODO: const → static readonly
        // TODO: 命名規則はMSDNを参照するとよいです。
        // https://learn.microsoft.com/ja-jp/dotnet/standard/design-guidelines/naming-guidelines
        public const string connectionString =
               @"Server=127.0.0.1;
               Port=5432;
               Database=todoapp_db;
               User ID=postgres;
               Password=postgres;";
    }
}