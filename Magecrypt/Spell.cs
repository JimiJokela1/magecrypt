using System.Collections.Generic;
using System.Linq;

namespace Magecrypt;

public class Spell
{
    public string Name { get; protected set; }
    public int ManaCost { get; protected set; }
    
    public Spell(string name, int manaCost)
    {
        Name = name;
        ManaCost = manaCost;
    }

    public virtual bool Cast(Player player, Point targetPoint)
    {
        // Cast spell
        if (player.CanUseMana(ManaCost))
        {
            player.ChangeMana(-ManaCost);
            RootScreen.Instance.ShowMessage($"{player.Id} casts {Name}!");
            return true;
        }
        else
        {
            RootScreen.Instance.ShowMessage($"{player.Id} does not have enough mana to cast {Name}!");
            return false;
        }
    }
}

public class DirectDamageSpell : Spell
{
    public DirectDamageSpell(string name, int manaCost, int damage) : base(name, manaCost)
    {
        Damage = damage;
    }

    public int Damage { get; private set; }

    public override bool Cast(Player player, Point targetPoint)
    {
        if (base.Cast(player, targetPoint))
        {
            List<GameObject> characters = new List<GameObject>(Map.Instance.MapObjectsByPosition[targetPoint.X, targetPoint.Y].Where(x => x is Character));
            characters.ForEach(obj =>
            {
                if (obj is Character character)
                {
                    character.TakeDamage(Damage, player);
                    RootScreen.Instance.ShowMessage($"{player.Id} hits {character.Id} with {Name} for {Damage} damage!");
                }
            });
            return true;
        }
        return false;
    }
}

public class HealSpell : Spell
{
    public HealSpell(string name, int manaCost, int healAmount) : base(name, manaCost)
    {
        HealAmount = healAmount;
    }

    public int HealAmount { get; private set; }

    public override bool Cast(Player player, Point targetPoint)
    {
        if (base.Cast(player, targetPoint))
        {
            List<GameObject> characters = new List<GameObject>(Map.Instance.MapObjectsByPosition[targetPoint.X, targetPoint.Y].Where(x => x is Character));
            characters.ForEach(obj =>
            {
                if (obj is Character character)
                {
                    character.Heal(HealAmount);
                    RootScreen.Instance.ShowMessage($"{player.Id} heals {character.Id} for {HealAmount} health!");
                }
            });
            return true;
        }
        return false;
    }
}

public class RevealSpell : Spell
{
    public RevealSpell(string name, int manaCost, int radius, int turns) : base(name, manaCost)
    {
        Radius = radius;
        Turns = turns;
    }

    public int Radius { get; private set; }
    public int Turns { get; private set; }

    public override bool Cast(Player player, Point targetPoint)
    {
        if (base.Cast(player, targetPoint))
        {
            // Reveal the map
            RootScreen.Instance.RevealMap(targetPoint, Radius, Turns);
            RootScreen.Instance.ShowMessage($"{player.Id} reveals the map!");
            return true;
        }
        return false;
    }
}