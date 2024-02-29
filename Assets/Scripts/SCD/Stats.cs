using System;
using System.Collections.Generic;

namespace SCD
{
    /// <summary>
    /// 統計データ
    /// </summary>
    public sealed class Stats
    {
        private readonly Dictionary<string, Record> records = new();

        /// <summary>
        /// 変更があった際に呼び出されるイベント
        /// </summary>
        public event Action<Record> OnChanged;

        /// <summary>
        /// 統計データを設定する
        /// </summary>
        public void Set(string name, int value)
        {
            if (records.TryGetValue(name, out var record))
            {
                record.Value = value;
            }
            else
            {
                records.Add(name, new Record(name, value));
            }
            OnChanged?.Invoke(record);
        }

        /// <summary>
        /// 統計データを加算する
        /// </summary>
        public void Add(string name, int value)
        {
            if (records.TryGetValue(name, out var record))
            {
                record.Value += value;
            }
            else
            {
                records.Add(name, new Record(name, value));
            }
            OnChanged?.Invoke(record);
        }

        /// <summary>
        /// 統計データを取得する
        /// </summary>
        public int Get(string name)
        {
            return records.TryGetValue(name, out var record) ? record.Value : 0;
        }

        public class Record
        {
            public string Name { get; }

            public int Value { get; set; }

            public Record(string name, int value)
            {
                Name = name;
                Value = value;
            }
        }
    }
}
