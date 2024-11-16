namespace Magecrypt;

public class Spell
{
    public string Name { get; protected set; }
    public int ManaCost { get; protected set; }

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

public class Fireball : Spell
{
    public Fireball() : base()
    {
        Name = "Fireball";
        ManaCost = 5;
    }

    public override bool Cast(Player player, Point targetPoint)
    {
        if (base.Cast(player, targetPoint))
        {
            // Do fireball stuff
            return true;
        }
        return false;
    }
}

public class Heal : Spell
{
    public Heal() : base()
    {
        Name = "Heal";
        ManaCost = 3;
    }

    public override bool Cast(Player player, Point targetPoint)
    {
        if (base.Cast(player, targetPoint))
        {
            player.Heal(5);
            return true;
        }
        return false;
    }
}

public class RevealSpell : Spell
{
    public RevealSpell() : base()
    {
        Name = "Reveal";
        ManaCost = 2;
    }

    public override bool Cast(Player player, Point targetPoint)
    {
        if (base.Cast(player, targetPoint))
        {
            // Reveal the map
            RootScreen.Instance.ShowMessage($"{player.Id} reveals the map!");
            return true;
        }
        return false;
    }
}