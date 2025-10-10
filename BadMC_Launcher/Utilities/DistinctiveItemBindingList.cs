using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace BadMC_Launcher.Utilities;

public class DistinctiveItemBindingList<T> : BindingList<T> {
    private readonly IEqualityComparer<T> _comparer;

    public DistinctiveItemBindingList(IList<T> initialData, IEqualityComparer<T>? comparer = null) : base(initialData) {
        if (comparer == null) {
            _comparer = EqualityComparer<T>.Default;
        }
        else {
            _comparer = comparer;
        }
    }

    public DistinctiveItemBindingList(IEqualityComparer<T> comparer) {
        _comparer = comparer ?? throw new ArgumentNullException(nameof(comparer));
    }

    public DistinctiveItemBindingList() : this(EqualityComparer<T>.Default) {

    }

    public string? PropertyName { get; set; }

    protected override void InsertItem(int index, T item) {
        if (!Contains(item)) {
            base.InsertItem(index, item);
        }
    }
    protected override void SetItem(int index, T item) {
        if (!Contains(item)) {
            base.SetItem(index, item);
        }
    }

    public void MargeItems(IEnumerable<T> addList) {
        this.RaiseListChangedEvents = false;

        this.AddRange(addList);

        this.RaiseListChangedEvents = true;

        OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1));
    }

    protected override void OnListChanged(ListChangedEventArgs e) {
        // Check if the property has the IgnoreListChangedAttribute
        if (e.PropertyDescriptor?.Attributes[typeof(IgnoreListChangedAttribute)] != null) { return; }

        base.OnListChanged(e);
    }

    public new bool Contains(T item) {
        return this.Any(existingItem => _comparer.Equals(existingItem, item));
    }
}

[AttributeUsage(AttributeTargets.Property)]
public class IgnoreListChangedAttribute : Attribute {

}
