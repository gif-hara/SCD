using System;
using System.Collections.Generic;
using UnityEngine;

namespace SCD
{
    /// <summary>
    /// ゲームを構成するコンテンツ
    /// </summary>
    [Serializable]
    public class Contents
    {
        [SerializeField]
        private List<Record> serializedRecords;

        private Dictionary<string, Record> records = null;

        private readonly HashSet<string> completed = new();

        /// <summary>
        /// コンテンツの一覧
        /// </summary>
        public IReadOnlyDictionary<string, Record> Records => records;

        /// <summary>
        /// 完了済みのコンテンツ
        /// </summary>
        public IReadOnlyCollection<string> Completed => completed;

        public Contents(IEnumerable<Record> records)
        {
            foreach (var record in records)
            {
                this.records.Add(record.Name, record);
            }
        }

        /// <summary>
        /// コンテンツを完了する
        /// </summary>
        public void Complete(Record record)
        {
            completed.Add(record.Name);
        }

        /// <summary>
        /// 利用可能なコンテンツを取得する
        /// </summary>
        public IReadOnlyList<Record> GetAvailable(Stats stats)
        {
            InitializeIfNeed();
            var list = new List<Record>();
            foreach (var record in records.Values)
            {
                if (!completed.Contains(record.Name) && record.IsAvailable(stats))
                {
                    list.Add(record);
                }
            }
            return list;
        }

        private void InitializeIfNeed()
        {
            if (records == null)
            {
                records = new Dictionary<string, Record>();
                foreach (var record in serializedRecords)
                {
                    records.Add(record.Name, record);
                }
            }
        }

        [Serializable]
        public class Record
        {
            /// <summary>
            /// このコンテンツの名前
            /// </summary>
            [field: SerializeField]
            public string Name { get; private set; }

            /// <summary>
            /// 開始するために必要な統計データ
            /// </summary>
            [field: SerializeField]
            public List<Stats.Record> Required { get; private set; }

            /// <summary>
            /// 完了するために必要な統計データ
            /// </summary>
            [field: SerializeField]
            public List<Stats.Record> Conditions { get; private set; }

            /// <summary>
            /// 完了した際の報酬となる統計データ
            /// </summary>
            [field: SerializeField]
            public List<Stats.Record> Rewards { get; private set; }

            public Record(string name, List<Stats.Record> required, List<Stats.Record> conditions, List<Stats.Record> rewards)
            {
                Name = name;
                Required = required;
                Conditions = conditions;
                Rewards = rewards;
            }

            /// <summary>
            /// 利用可能かどうかを判定する
            /// </summary>
            public bool IsAvailable(Stats stats)
            {
                foreach (var record in Required)
                {
                    if (stats.Get(record.Name) < record.Value)
                    {
                        return false;
                    }
                }
                return true;
            }

            /// <summary>
            /// 完了したかどうかを判定する
            /// </summary>
            public bool IsCompleted(Stats stats)
            {
                foreach (var record in Conditions)
                {
                    if (stats.Get(record.Name) < record.Value)
                    {
                        return false;
                    }
                }
                return true;
            }

            /// <summary>
            /// 報酬を適用する
            /// </summary>
            public void ApplyRewards(Stats stats)
            {
                foreach (var record in Rewards)
                {
                    stats.Add(record.Name, record.Value);
                }
            }
        }
    }
}
