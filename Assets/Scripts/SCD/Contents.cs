using System.Collections.Generic;

namespace SCD
{
    /// <summary>
    /// ゲームを構成するコンテンツ
    /// </summary>
    public class Contents
    {
        private readonly Dictionary<string, Record> records = new();

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

        public class Record
        {
            /// <summary>
            /// このコンテンツの名前
            /// </summary>
            public string Name { get; }

            /// <summary>
            /// 開始するために必要な統計データ
            /// </summary>
            public List<Stats.Record> Required { get; }

            /// <summary>
            /// 完了するために必要な統計データ
            /// </summary>
            public List<Stats.Record> Conditions { get; }

            /// <summary>
            /// 完了した際の報酬となる統計データ
            /// </summary>
            public List<Stats.Record> Rewards { get; }

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
