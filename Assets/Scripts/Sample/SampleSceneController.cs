using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SCD.Sample
{
    public class SampleSceneController : MonoBehaviour
    {
        [SerializeField]
        private Contents testContents;

        [SerializeField]
        private List<Contents.Record> test;

        private readonly Stats stats = new();

        private readonly Contents quests = new(
            new[]
            {
                new Contents.Record(
                    "Quest1",
                    new List<Stats.Record>(),       // 条件が無いクエストも作れる
                    new List<Stats.Record>()
                    {
                        new("Quest.1.Cleared", 1),  // すでにクエスト1がクリアされている場合は無視される
                    },
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
                    new List<Stats.Record>()
                    {
                        new("Quest.2.Cleared", 1),  // すでにクエスト2がクリアされている場合は無視される
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
}
