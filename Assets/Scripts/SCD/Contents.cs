using System.Collections.Generic;

namespace SCD
{
    /// <summary>
    /// ゲームを構成するコンテンツ
    /// </summary>
    public class Contents
    {
        public class Record
        {
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

            public Record(List<Stats.Record> required, List<Stats.Record> conditions, List<Stats.Record> rewards)
            {
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
                    if (stats.Get(record.name) < record.value)
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
                    if (stats.Get(record.name) < record.value)
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
                    stats.Add(record.name, record.value);
                }
            }
        }
    }
}
