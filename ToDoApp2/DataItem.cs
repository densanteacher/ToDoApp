using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

// DONE: namespace
namespace ToDoApp2;

// DONE: SQL とは何かを明示しましょう。
/// <summary>
/// SQLデータベースから取得したToDoデータを保存しておくクラスです。
/// </summary>
public class DataItem
{
    // DONE: 適切な改行を入れてください。
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

    // DONE: 更新日時を記録しておく列名として、よく updated_at という列名が使われます。日付の場合は updated_on となります。
    // これはたしかRuby言語の文化だったと思いますが、わかりやすいので是非使ってください。
    /// <summary>
    /// ToDoリストが更新された日時を保持しておく値です。
    /// </summary>
    public DateTime Update_at { get; set; }

    /// <summary>
    /// 画像ファイルです。
    /// </summary>
    public PngBitmapDecoder Image { get; set; }

    // DONE: bool 側は is から始めましょう。DB側の列名にも is_ を使ってかまいません。
    /// <summary>
    /// ToDoリストをリマインドするかを指定する値です。
    /// </summary>
    public bool IsRemind { get; set; }

    /// <summary>
    /// リマインドを行う日時を指定する値です。
    /// </summary>
    public DateTime RemindDate { get; set; }

    /// <summary>
    /// インスタンスです。IDが必須項目です。
    /// </summary>
    /// <param name="id"></param>
    public DataItem(int id)
    {
        this.Id = id;

    }

    // DONE: chackタイポ
    // DONE: 少し横に長いです。( の次と各 , の後ろで改行しましょう。
    /// <summary>
    /// id以外の各値を一括で設定します。
    /// </summary>
    public void SetValues(
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
        this.Update_at = updatedAt;
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
