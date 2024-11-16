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