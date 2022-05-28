using Dryer_Server.Interfaces;
using System;
using System.Collections.Generic;

namespace Dryer_Server.Persistance
{
    internal partial class HistoricalManager
    {
        private sealed class Item<T> where T : class, IHistoricValue
        {
            public static Dictionary<int, Item<T>> Items = new Dictionary<int, Item<T>>();

            public Item(T value)
            {
                this.value = value;

                lock (Items)
                {
                    Items.TryGetValue(value.ChamberId, out last);
                    Items[value.ChamberId] = this;
                }
                lastSaved = last?.lastSaved ?? DateTime.MinValue;
            }

            public bool saved = false;
            public T value;
            public Item<T> last;
            public DateTime lastSaved;

            internal bool IsDifferent()
            {
                return !value.DataEquals(last?.value);
            }
        }
    }
}