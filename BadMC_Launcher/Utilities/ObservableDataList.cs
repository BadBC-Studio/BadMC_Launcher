using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BadMC_Launcher.Utilities;
public class ObservableDataList<T> : ObservableCollection<T> {
    public ObservableDataList(IEnumerable<T> initialData) : base(initialData) {

    }

    public ObservableDataList() {

    }

    public event EventHandler? OnAddedItemDuplication;

    protected override void InsertItem(int index, T item) {
        if (!Contains(item)) {
            base.InsertItem(index, item);
        }
        else {
            OnAddedItemDuplication?.Invoke(this, new EventArgs());
        }
    }
    protected override void SetItem(int index, T item) {
        if (!Contains(item)) {
            base.SetItem(index, item);
        }
        else {
            OnAddedItemDuplication?.Invoke(this, new EventArgs());
        }
    }

    public void MargeItems(IEnumerable<T> addList) {
        CheckReentrancy();

        Items.AddRange(addList);

        OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
    }
}
