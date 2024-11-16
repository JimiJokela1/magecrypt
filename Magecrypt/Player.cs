namespace Magecrypt;

public class Player : Character
{
    public Player(string id, ColoredGlyph appearance, Point position, Map map, bool blocksMovement = false) : base(id, appearance, position, map, blocksMovement)
    {
    }

    public Player(string id, ColoredGlyph appearance, Point position, Map map, int health, int mana, int maxMana, int attack, int defense, bool blocksMovement = true) : base(id, appearance, position, map, health, attack, defense, blocksMovement)
    {
        Mana = mana;
        MaxMana = maxMana;
    }

    public int Mana { get; set; }
    public int MaxMana { get; set; }

    public void ChangeMana(int amount)
    {
        Mana += amount;
        if (Mana > MaxMana) Mana = MaxMana;
        if (Mana < 0) Mana = 0;
    }

    public bool CanUseMana(int amount)
    {
        return Mana >= amount;
    }

    public void CastSpell(int manaCost)
    {
        // Show line of sight targeting UI
        RootScreen.Instance.ShowLineOfSightTargeting();

        // Cast spell
        if (CanUseMana(manaCost))
        {
            ChangeMana(-manaCost);
            RootScreen.Instance.ShowMessage($"{Id} casts a spell!");
        }
        else
        {
            RootScreen.Instance.ShowMessage($"{Id} does not have enough mana to cast a spell!");
        }
    }

    public void PickUpMagicParticle(MagicParticle particle)
    {
        ChangeMana(particle.ManaValue);
        particle.PickUp();
    }
}