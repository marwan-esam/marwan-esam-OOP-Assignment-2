using System;
using System.Collections.Generic;
using System.Linq;

namespace SrpLab;

public class PickListTracker
{
    private readonly List<(string Sku, string Aisle, int Bin, int QtyNeeded, int QtyOnHand)> _lines = new();

    public void AddNeed(string sku, string aisle, int bin, int qtyNeeded, int qtyOnHand)
    {
        _lines.Add((sku, aisle, bin, qtyNeeded, qtyOnHand));
    }

    public IReadOnlyList<(string Sku, string Aisle, int Bin, int QtyNeeded, int QtyOnHand)> Lines => _lines;
}

public class InventoryAllocator
{
    public IReadOnlyList<(string Sku, int Allocated)> Allocate(PickListTracker tracker)
    {
        var result = new List<(string, int)>();
        foreach (var line in tracker.Lines)
        {
            var alloc = Math.Min(line.QtyNeeded, line.QtyOnHand);
            result.Add((line.Sku, alloc));
        }
        return result;
    }
}

public class RoutingHeuristics
{
    public IReadOnlyList<(string Aisle, int Bin, string Sku, int Qty)> WalkingOrder(PickListTracker tracker)
    {
        return tracker.Lines
            .OrderBy(l => l.Aisle)
            .ThenBy(l => l.Bin)
            .Select(l => (l.Aisle, l.Bin, l.Sku, Math.Min(l.QtyNeeded, l.QtyOnHand)))
            .Where(x => x.Item4 > 0)
            .ToList();
    }
}

public class PickerScriptFormatter
{
    public string PickerScript(RoutingHeuristics routing, InventoryAllocator allocator, PickListTracker tracker)
    {
        var steps = routing.WalkingOrder(tracker)
            .Select((s, i) => $"{i + 1}. Go aisle {s.Aisle} bin {s.Bin}: pick {s.Qty} × {s.Sku}");
        var shortfalls = allocator.Allocate(tracker).Where(a =>
        {
            var need = tracker.Lines.First(l => l.Sku == a.Sku).QtyNeeded;
            return a.Allocated < need;
        });
        var warn = shortfalls.Any()
            ? "SHORTAGES: " + string.Join(", ", shortfalls.Select(s => s.Sku))
            : "SHORTAGES: none";
        return string.Join('\n', steps) + "\n" + warn;
    }
}

public class WmsXmlExporter
{
    public string WmsXmlBatch(string batchId, InventoryAllocator allocator, PickListTracker tracker)
    {
        var parts = allocator.Allocate(tracker).Select(a => $"<line sku=\"{a.Sku}\" qty=\"{a.Allocated}\" />");
        return $"<batch id=\"{batchId}\">{string.Join("", parts)}</batch>";
    }
}
