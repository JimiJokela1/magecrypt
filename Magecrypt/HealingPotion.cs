namespace Magecrypt;

public class HealingPotion : Item
{
    private int _healAmount;

    public HealingPotion(string id, ColoredGlyph appearance, Point position, Map map, bool blocksMovement = false, int goldValue = 0) : base(id, appearance, position, map, blocksMovement, goldValue, usable: true)
    {
    }

    public HealingPotion(string id, int healAmount, Point position, Map map) : base(id, new ColoredGlyph(Color.Red, Color.Black, 33), position, map, usable: true)
    {
        _healAmount = healAmount;
    }

    public override void Use(Character character)
    {
        // Heal the player
        character.Heal(_healAmount);
        RootScreen.Instance.ShowMessage("You drink the healing potion and feel better!");

        character.Inventory.Remove(this);
    }
}

public class HealingPotionTemplate : ItemTemplate
{
    private int _healAmount;

    public HealingPotionTemplate(string name, ColoredGlyph appearance, int healAmount, int minLevel, int maxLevel, float chanceWeight) : base(name, appearance, 0, false, true, minLevel, maxLevel, chanceWeight)
    {
        _healAmount = healAmount;
    }

    public HealingPotion CreateHealingPotion(Point position, Map map)
    {
        return new HealingPotion(Name, _healAmount, position, map);
    }
}