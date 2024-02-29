using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SCD.Sample
{
    public class SampleSceneController : MonoBehaviour
    {
        private readonly Stats stats = new();

        private readonly Contents quests = new(
            new[]
            {
                new Contents.Record(
                    "Quest1",
                    new List<Stats.Record>(),      // 条件が無いクエストも作れる
                    new List<Stats.Record>         // 完了条件は木材というアイテムを5個集めること
                    {
                        new("Item.Wood", 5),
                    },
                    new List<Stats.Record>
                    {
                        new("Quest.1.Cleared", 1),
                        new("Unlock.Item.Ore", 1),
                    }),
                new Contents.Record(
                    "Quest2",
                    new List<Stats.Record>
                    {
                        new("Quest.1.Cleared", 1),
                    },
                    new List<Stats.Record>
                    {
                        new("Item.Ore", 8),
                    },
                    new List<Stats.Record>
                    {
                        new("Quest.2.Cleared", 1),
                    }),
            }
        );

        private List<Contents.Record> availableQuests;

        private List<Contents.Record> completedQuests = new();

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

            if (availableQuests == null)
            {
                return;
            }

            foreach (var quest in availableQuests)
            {
                if (quest.IsCompleted(stats))
                {
                    completedQuests.Add(quest);
                }
            }
            availableQuests.RemoveAll(x => x.IsCompleted(stats));
        }
    }
}
