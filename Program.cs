using System;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;
using ReactiveMarbles.ObservableEvents;

namespace DynamicData.BindingListRefreshRepro;

internal record class SomeClass(int Id, string Something);

internal static class Program
{
    /// <summary>
    /// The main entry point for the application.
    /// </summary>
    static void Main()
    {
        Console.WriteLine("=== Adding item ===");
        SourceCache<SomeClass, int> cache = new(x => x.Id);
        BindingList<SomeClass> bindingList = new();
        BindingList<SomeClass> sortedBindingList = new();

        // Unsorted

        cache.Connect()
            .Do(changes => changes.ToList().ForEach(change => Console.WriteLine($"Change: {change.Reason}")))
            .Bind(bindingList)
            .Subscribe();

        bindingList.Events().ListChanged
            .Do(e => Console.WriteLine($"bindingList: {e.ListChangedType}"))
            .Subscribe();

        // Sorted

        cache.Connect()
            .SortBy(x => x.Id)
            .Do(changes => changes.ToList().ForEach(change => Console.WriteLine($"Sorted change: {change.Reason}")))
            .Bind(sortedBindingList)
            .Subscribe();

        sortedBindingList.Events().ListChanged
            .Do(e => Console.WriteLine($"sortedBindingList: {e.ListChangedType}"))
            .Subscribe();

        SomeClass item = new(1, "Some value");
        cache.AddOrUpdate(item);

        Console.WriteLine("=== Refreshing item ===");
        cache.Refresh(item);
    }
}