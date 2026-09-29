using System;
using System.Collections.Generic;
using System.Linq;

namespace SrpLab;

public class KitchenOrder
{
    private readonly List<(string Item, List<string> Ingredients, int PrepMinutes)> _items = new();

    public void AddItem(string item, IEnumerable<string> ingredients, int prepMinutes)
    {
        _items.Add((item, ingredients.Select(i => i.Trim().ToLowerInvariant()).ToList(), prepMinutes));
    }

    public IReadOnlyList<(string Item, List<string> Ingredients, int PrepMinutes)> Items => _items;
}

public class AllergenDetector
{
    public IReadOnlyList<string> DetectAllergens(KitchenOrder order)
    {
        var hits = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var (_, ingredients, _) in order.Items)
        {
            foreach (var ing in ingredients)
            {
                if (ing.Contains("milk") || ing.Contains("cheese") || ing.Contains("butter")) hits.Add("dairy");
                if (ing.Contains("wheat") || ing.Contains("flour") || ing.Contains("bread")) hits.Add("gluten");
                if (ing.Contains("peanut") || ing.Contains("almond") || ing.Contains("cashew")) hits.Add("nuts");
                if (ing.Contains("shrimp") || ing.Contains("prawn") || ing.Contains("crab")) hits.Add("shellfish");
            }
        }
        return hits.OrderBy(x => x).ToList();
    }
}

public class KitchenTimingHeuristics
{
    public int EstimatedReadyMinutes(KitchenOrder order, int openStations, int allergenCount)
    {
        if (openStations <= 0) openStations = 1;
        var sequential = order.Items.Sum(i => i.PrepMinutes);
        var parallel = (int)Math.Ceiling(sequential / (double)openStations);
        if (allergenCount > 0) parallel += 3; 
        var longest = order.Items.Count == 0 ? 0 : order.Items.Max(i => i.PrepMinutes);
        return Math.Max(parallel, longest);
    }

    public string ExpoLaneHint(int allergenCount, int eta)
    {
        return allergenCount > 0 ? "LANE-ALLERGY" : eta > 20 ? "LANE-SLOW" : "LANE-FAST";
    }
}

public class ThermalTicketFormatter
{
    public string RenderThermalTicket(int orderNumber, KitchenOrder order, int eta, IReadOnlyList<string> allergens)
    {
        var width = 32;
        var line = new string('=', width);
        var body = string.Join('\n', order.Items.Select(i => $"* {i.Item.ToUpperInvariant()} ({i.PrepMinutes}m)"));
        var allergyLine = allergens.Count == 0 ? "ALLERGENS: none" : "ALLERGENS: " + string.Join(",", allergens);
        return $"{line}\nORDER #{orderNumber}\nETA {eta} MIN\n{body}\n{allergyLine}\n{line}\n";
    }
}
