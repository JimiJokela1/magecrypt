namespace Magecrypt;

public class Armor : Item
{
    public Armor(string id, ColoredGlyph appearance, Point position, Map map) : base(id, appearance, position, map, false, 0, true, false)
    {
    }

    public Armor(string id, ColoredGlyph appearance, Point position, Map map, ArmorSlot slot, int defense) : base(id, appearance, position, map, false, 0, true, false)
    {
        Slot = slot;
        Defense = defense;
    }

    public enum ArmorSlot
    {
        Head,
        Chest,
        Hands,
        Legs,
        Feet
    }

    public ArmorSlot Slot { get; set; }
    public int Defense { get; set; }

    public override void Equip(Character character)
    {
        character.SetArmor(this);
    }
}