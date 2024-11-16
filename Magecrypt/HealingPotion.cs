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