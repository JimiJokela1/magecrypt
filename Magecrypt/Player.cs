using System.Collections.Generic;

namespace Magecrypt;

public class Player : Character
{
    public Player(string id, ColoredGlyph appearance, Point position, Map map, int health, int mana, int maxMana, int attack, int defense, bool blocksMovement = true) : base(id, appearance, position, map, health, attack, defense, blocksMovement)
    {
        Mana = mana;
        MaxMana = maxMana;
        Spells = new List<Spell>()
        {
            new DirectDamageSpell("Fireball", 5, 50),
            new HealSpell("Heal", 2, 25),
            new RevealSpell("Reveal", 1, 12, 12),
            new RevealSpell("Major Reveal", 0, 100, 100)
        };
    }

    public int Mana { get; set; }
    public int MaxMana { get; set; }

    public List<Spell> Spells { get; set; } = new List<Spell>();

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

    public void StartCastingSpell()
    {
        RootScreen.Instance.ShowSpellMenu();
    }

    public void PickUpMagicParticle(MagicParticle particle)
    {
        ChangeMana(particle.ManaValue);
        RootScreen.Instance.ShowMessage($"{Id} picks up a magic particle and gains mana!");
        particle.PickUp();
    }
}