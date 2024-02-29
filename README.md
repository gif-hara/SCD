# Stats Contents Design

# これはなに？
- Stats Contents Designとは統計データとコンテンツによってゲームを構築する設計手法です。
- 設計は非常にシンプルで、以下の2つの要素で成り立っています。
    - Stats
        - ゲームの統計データ。
        - ゲームを開始した回数、クリアした回数、アイテム所持数、死んだ回数などを記録します。
    - Contents
        - ゲームのコンテンツ。
        - つまり「クエスト」や「アイテムの加工」などのゲームの要素です。

# Stats
- Statsはゲームの全ての統計データを記録します。
- Stats.Recordクラスを管理しており、このRecordは以下の要素を持ちます。
    - Name
        - 統計データの名前。
    - Value
        - int型の値。

# Contents
- Contentsはゲームのコンテンツを表します。
- Contents.Recordクラスを管理しており、このRecordは以下の要素を持ちます。
    - Name
        - コンテンツの名前。
    - Required
        - コンテンツの開放条件。
    - Conditions
        - コンテンツの完了条件。
    - Rewards
        - コンテンツの報酬。

# 使い方
- SampleSceneController.csにサンプルコードを記載しています。
```csharp
public class SampleSceneController : MonoBehaviour
{
    private readonly Stats stats = new();

    private readonly Contents quests = new(
        new[]
        {
            new Contents.Record(
                "Quest1",
                new List<Stats.Record>(),       // 条件が無いクエストも作れる
                new List<Stats.Record>
                {
                    new("Item.Wood", 5),        // 完了条件は木材というアイテムを5個集めること
                },
                new List<Stats.Record>
                {
                    new("Quest.1.Cleared", 1),  // クエスト1がクリアされたことを示す統計データを設定
                    new("Unlock.Item.Ore", 1),  // 鉱石を採掘できるようにするための統計データを設定
                }),
            new Contents.Record(
                "Quest2",
                new List<Stats.Record>
                {
                    new("Quest.1.Cleared", 1),  // Quest1がクリアされていることがクエスト2の開始条件
                },
                new List<Stats.Record>
                {
                    new("Item.Ore", 8),         // 完了条件は鉱石というアイテムを8個集めること
                },
                new List<Stats.Record>
                {
                    new("Quest.2.Cleared", 1),  // クエスト2がクリアされたことを示す統計データを設定
                }),
        }
    );

    private List<Contents.Record> availableQuests;

    private readonly List<Contents.Record> completedQuests = new();

    private void Start()
    {
        stats.OnChanged += OnStatsChanged;
        availableQuests = quests.GetAvailable(stats).ToList();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            stats.Add("Item.Wood", 1);
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            if (stats.Contains("Unlock.Item.Ore"))
            {
                stats.Add("Item.Ore", 1);
            }
            else
            {
                Debug.Log("あなたはまだ鉱石を採掘することができません");
            }
        }

        // スペースキーを押すとクエストを完了する
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("Completed Quests Count: " + completedQuests.Count);
            if (completedQuests.Count > 0)
            {
                var temp = new List<Contents.Record>(completedQuests);
                completedQuests.Clear();
                foreach (var quest in temp)
                {
                    quest.ApplyRewards(stats);
                    quests.Complete(quest);
                }
                availableQuests = quests.GetAvailable(stats).ToList();
            }
        }
    }

    private void OnStatsChanged(Stats.Record record)
    {
        Debug.Log($"Stats Changed: {record.Name} = {record.Value}");

        // クエストの進行状況をチェック
        foreach (var quest in availableQuests)
        {
            if (quest.IsCompleted(stats))
            {
                completedQuests.Add(quest);
            }
        }
        // 完了済みのクエストを除外
        availableQuests.RemoveAll(x => x.IsCompleted(stats));
    }
}
```
- このサンプルは、StatsとContentsを使ってゲームを構築する手法を示しています。
- Contentsとしてquestsを定義しており、Quest1、Quest2があります。
    - Quest1は以下の構成で作られています
        - Required: なし
        - Conditions: Item.Woodの統計データが5以上
        - Rewards: Quest1をクリアした統計データとItem.Oreを取得出来る条件の統計データ
    - Quest2は以下の構成で作られています
        - Required: Quest1をクリアした統計データ
        - Conditions: Item.Oreの統計データが8以上
        - Rewards: Quest2をクリアした統計データ
- Qキーを押すことで"Item.Wood"の統計データを1増やすことができます。
- Wキーを押すことで"Item.Ore"の統計データを1増やすことができます。
    - ただし、"Unlock.Item.Ore"の統計データが存在しないと"Item.Ore"の統計データを増やすことができません。
- スペースキーを押すことでクエストを完了することができます。
- 最初は"Item.Wood"の統計データを5以上にすることでQuest1が完了し、Quest1を完了することで"Unlock.Item.Ore"の統計データが増え、Quest2が開放されます。

# まとめ
- StatsとContentsを使うことで、ゲームの統計データとコンテンツを管理することができます。
- この手法は上記に示したクエストシステムや実績システムなどなど、様々なゲームシステムに適用することができます。
