namespace Magecrypt;

public class Item : GameObject
{
    public bool GoldItem { get; set; }
    public int GoldValue { get; set; }
    public bool Equippable { get; set; }
    public bool Usable { get; set; }

    public Item(string id, ColoredGlyph appearance, Point position, Map map, bool blocksMovement = false, int goldValue = 0, bool equippable = false, bool usable = false) : base(id, appearance, position, map, blocksMovement)
    {
        if (goldValue > 0)
        {
            GoldItem = true;
            GoldValue = goldValue;
        }

        Equippable = equippable;
        Usable = usable;
    }

    public virtual void Equip(Character character)
    {

    }

    public virtual void Use(Character character)
    {

    }

    public virtual void PickUp()
    {
        // Remove the item from the map
        Map.Instance.RemoveGameObject(this);
    }
}