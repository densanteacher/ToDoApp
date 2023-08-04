using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace ToDoApp2;

// DONE: IS NOT NULL 制約について考えてみましょう。

// NULLによりデータの整合性が取れなくなる可能性があるため基本的にはあった方がいいと考えられます。
// しかし、一時的に制約が崩れるような操作が必要な時に困ったり、意図せずNULLを登録しようとした際即エラーが起こるなどの問題も起こりうるようです。

/// <summary>
/// データベースから取得したToDoデータを保存しておくクラスです。
/// </summary>
public class ToDoData
{
    /// <summary>
    /// SQLから取得したIDです。プライマリキーです。
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// ToDoリストが実行済みであるかどうかを判定する値です。
    /// </summary>
    public bool IsFinished { get; set; }

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

    /// <summary>
    /// ToDoDataItemの各値が変更された場合true、変更されていない場合falseの値をとります。
    /// </summary>
    public bool IsChanged { get; set; } = false;

    /// <summary>
    /// コンストラクタです。IDが必須項目です。
    /// </summary>
    public ToDoData(int id)
    {
        this.Id = id;
    }

    /// <summary>
    /// 各値を一括で設定します。
    /// </summary>
    public void Set(
        bool isFinished,
        string title,
        string memo,
        DateTime dateStart,
        DateTime dateEnd,
        int priority,
        DateTime updatedAt)
    {
        this.IsFinished = isFinished;
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
    public void SetString(string title, string memo)
    {
        this.ToDoTitle = title;
        this.Memo = memo;
    }
}
