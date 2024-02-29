using System;
using System.Collections.Generic;
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
                    new List<Stats.Record>
                    {
                        new("Game.Begin", 1),
                    },
                    new List<Stats.Record>
                    {
                        new("Item.Wood", 5),
                    },
                    new List<Stats.Record>
                    {
                        new("Quest.1.Cleared", 1),
                    }),
            }
        );

        private IReadOnlyList<Contents.Record> availableQuests;

        private void Start()
        {
            stats.OnChanged += OnStatsChanged;
            stats.Set("Game.Begin", 1);
            availableQuests = quests.GetAvailable(stats);
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                stats.Add("Item.Wood", 1);
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
                    Debug.Log($"Completed Quest: {quest.Name}");
                }
            }
        }
    }
}
