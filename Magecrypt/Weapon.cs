namespace Magecrypt;

public class Weapon : Item
{
    public Weapon(string id, ColoredGlyph appearance, Point position, Map map) : base(id, appearance, position, map, false, 0, true, false)
    {
    }

    public Weapon(string id, ColoredGlyph appearance, Point position, Map map, int attack) : base(id, appearance, position, map, false, 0, true, false)
    {
        Attack = attack;
    }

    public int Attack { get; set; }
}

public class WeaponTemplate : ItemTemplate
{
    public int Attack { get; set; }

    public WeaponTemplate(string name, ColoredGlyph appearance, int attack, int minLevel, int maxLevel, float chanceWeight) : base(name, appearance, 0, true, false, minLevel, maxLevel, chanceWeight)
    {
        Attack = attack;
    }

    public Weapon CreateWeapon(Point position, Map map)
    {
        return new Weapon(Name, Appearance, position, map, Attack);
    }
}