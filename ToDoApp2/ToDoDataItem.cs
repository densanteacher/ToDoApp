using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace ToDoApp2;

// TODO: 「DataItem」に対して、「ToDoデータを・・・」というコメントになっています。
// コメントとソースコード上に差異が現れています。
// つまり、どちらかに寄せることができます。
// コメントを直すのか、クラス名を直すのか、どちらがよりコードを読みやすくなるでしょうか？
/// <summary>
/// SQLデータベースから取得したToDoデータを保存しておくクラスです。
/// </summary>
public class ToDoDataItem
{
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

    /// <summary>
    /// ToDoの終了日（期限）です。
    /// </summary>
    public DateTime? DateEnd { get; set; }

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
    public DateTime UpdateAt { get; set; }

    /// <summary>
    /// 画像ファイルです。
    /// </summary>
    public PngBitmapDecoder Image { get; set; }

    /// <summary>
    /// ToDoリストをリマインドするかを指定する値です。
    /// </summary>
    public bool IsRemind { get; set; }

    /// <summary>
    /// リマインドを行う日時を指定する値です。
    /// </summary>
    public DateTime RemindDate { get; set; }

    // TODO: インスタンスは、new された後の変数の中身を指します。
    // この new するための特殊なメソッドはコンストラクタと呼びます。
    // Constructor 省略は ctor. と書きます。
    /// <summary>
    /// インスタンスです。IDが必須項目です。
    /// </summary>
    public ToDoDataItem(int id)
    {
        this.Id = id;
    }

    /// <summary>
    /// id以外の各値を一括で設定します。
    /// </summary>
    public void SetDataItem(
        bool checkDone,
        string title,
        string memo,
        DateTime dateStart,
        DateTime dateEnd,
        int priority,
        DateTime updatedAt)
    {
        this.CheckDone = checkDone;
        this.ToDoTitle = title;
        this.Memo = memo;
        this.DateStart = dateStart;
        this.DateEnd = dateEnd;
        this.Priority = priority;
        this.UpdateAt = updatedAt;
    }

    /// <summary>
    /// string型の値を一括で設定します。
    /// </summary>
    public void SetValueString(string title, string memo)
    {
        this.ToDoTitle = title;
        this.Memo = memo;
    }
}
