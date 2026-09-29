using System.Diagnostics.Contracts;

namespace PatternsLab.Problems.Prototype;

public class Weapon
{
    public string Name { get; set; }
    public int Damage { get; set; }
}

public abstract class Enemy
{
    private string _modelData;

    public string Name { get; set; }
    public int Health { get; set; }
    public Weapon Weapon { get; set; }
    public List<string> Abilities { get; set; } = new();
    public string ModelId => _modelData;

    protected Enemy()
    {
        Console.WriteLine("   ...loading 3D model (slow)...");
        Thread.Sleep(500);
        _modelData = "MODEL_" + Guid.NewGuid().ToString("N")[..6];
    }

    public abstract Enemy Clone();
}

public class Orc : Enemy
{
    public Orc()
    {
        Name = "Orc";
        Health = 100;
        Weapon = new Weapon { Name = "Axe", Damage = 25 };
        Abilities.Add("Rage");
    }

    public override Enemy Clone()
    {
        var clone = (Orc)this.MemberwiseClone();
        clone.Weapon = new Weapon {Name = this.Weapon.Name, Damage = this.Weapon.Damage};
        clone.Abilities = [.. this.Abilities];
        return clone;
    }
}

public class Elf : Enemy
{
    public Elf()
    {
        Name = "Elf";
        Health = 70;
        Weapon = new Weapon { Name = "Bow", Damage = 18 };
        Abilities.Add("Stealth");
    }

    public override Enemy Clone()
    {
        var clone = (Elf)this.MemberwiseClone();
        clone.Weapon = new Weapon {Name = this.Weapon.Name, Damage = this.Weapon.Damage};
        clone.Abilities = [.. this.Abilities];
        return clone;
    }
}

public static class EnemyCopyHelper
{
    public static Enemy CopyEnemy(Enemy e)
    {
        // Enemy c;
        // if (e is Orc) c = new Orc();
        // else if (e is Elf) c = new Elf();
        // else throw new NotSupportedException("Unknown enemy type");

        // c.Name = e.Name;
        // c.Health = e.Health;
        // c.Weapon = e.Weapon;
        // c.Abilities = e.Abilities;
        // return c;
        return e.Clone();
    }
}
