using System.Collections.Generic;

namespace SCD
{
    /// <summary>
    /// ゲームを構成するコンテンツ
    /// </summary>
    public class Contents
    {
        private readonly Dictionary<string, Record> records = new();

        private readonly HashSet<string> consumed = new();

        /// <summary>
        /// コンテンツの一覧
        /// </summary>
        public IReadOnlyDictionary<string, Record> Records => records;

        /// <summary>
        /// 消費済みのコンテンツ
        /// </summary>
        public IReadOnlyCollection<string> Consumed => consumed;

        public Contents(IEnumerable<Record> records)
        {
            foreach (var record in records)
            {
                this.records.Add(record.Name, record);
            }
        }

        public void Consume(Record record)
        {
            consumed.Add(record.Name);
        }

        public IReadOnlyList<Record> GetAvailable(Stats stats)
        {
            var list = new List<Record>();
            foreach (var record in records.Values)
            {
                if (!consumed.Contains(record.Name) && record.IsAvailable(stats))
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
            /// このコンテンツを開始するために必要な統計データ
            /// </summary>
            public List<Stats.Record> Required { get; }

            /// <summary>
            /// このコンテンツを完了するために必要な統計データ
            /// </summary>
            public List<Stats.Record> Conditions { get; }

            /// <summary>
            /// このコンテンツを完了した際の報酬となる統計データ
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
            /// このコンテンツを利用可能かどうかを判定する
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
            /// このコンテンツを完了したかどうかを判定する
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
            public void Apply(Stats stats)
            {
                foreach (var record in Rewards)
                {
                    stats.Add(record.Name, record.Value);
                }
            }
        }
    }
}
