using System.Collections.Generic;
using System.Linq;

namespace Magecrypt;

public class Character : GameObject
{
    public int Health { get; private set; }
    public int MaxHealth { get; private set; }
    public int AttackStat { get; init; }
    public int DefenseStat { get; init; }
    public int AttackValue => AttackStat + (Weapon?.Attack ?? 0);
    public int DefenseValue => DefenseStat + Armor?.Values.Sum(a => a.Defense) ?? 0;

    public List<Item> Inventory { get; private set; } = new List<Item>();
    public int Gold { get; private set; }
    public Dictionary<Armor.ArmorSlot, Armor> Armor { get; set; }
    public Weapon Weapon { get; set; }

    public Character(string id, ColoredGlyph appearance, Point position, Map map, bool blocksMovement = false) : base(id, appearance, position, map, blocksMovement)
    {
    }

    public Character(string id, ColoredGlyph appearance, Point position, Map map, int health, int attack, int defense, bool blocksMovement = true) : base(id, appearance, position, map, blocksMovement)
    {
        MaxHealth = health;
        Health = health;
        AttackStat = attack;
        DefenseStat = defense;
    }

    public void SetHealth(int health)
    {
        Health = health;
    }

    public void Attack(Character target)
    {
        int damage = AttackValue - target.DefenseValue;
        if (damage < 0) damage = 0;
        target.TakeDamage(damage, this);
    }

    public void TakeDamage(int damage, GameObject source)
    {
        Health -= damage;
        if (Health <= 0)
        {
            Health = 0;
            RootScreen.Instance.ShowMessage($"{Id} dies from {source.Id}!");
            Die();
        }
        else
        {
            RootScreen.Instance.ShowMessage($"{Id} takes {damage} damage from {source.Id}!");
        }
    }

    public void Heal(int amount)
    {
        Health += amount;
        if (Health > MaxHealth)
        {
            Health = MaxHealth;
        }
    }

    public virtual void Die()
    {
        // Remove armor
        if (Armor != null)
        {
            RemoveArmor(Magecrypt.Armor.ArmorSlot.Chest);
            RemoveArmor(Magecrypt.Armor.ArmorSlot.Feet);
            RemoveArmor(Magecrypt.Armor.ArmorSlot.Hands);
            RemoveArmor(Magecrypt.Armor.ArmorSlot.Head);
            RemoveArmor(Magecrypt.Armor.ArmorSlot.Legs);
            Armor.Clear();
        }

        // Drop all items
        foreach (Item item in Inventory)
        {
            Map.Instance.AddGameObject(item);
            Map.Instance.MoveGameObject(item, Position);
        }
        Inventory.Clear();

        // Drop gold
        if (Gold > 0)
        {
            Item gold = new Item("Gold", new ColoredGlyph(Color.Yellow, Color.Black, 36), Position, Map.Instance, goldValue: Gold);
            Map.Instance.AddGameObject(gold);
            Gold = 0;
        }

        // Remove the character from the map
        Map.Instance.RemoveGameObject(this);

        if (this is Player)
        {
            RootScreen.Instance.ShowMessage("You died!");
            RootScreen.Instance.ShowMessage("Press any key to restart.");
        }
    }

    public void PickUpItem(Item item)
    {
        if (item == null) return;

        if (item.GoldItem)
        {
            Gold += item.GoldValue;
            RootScreen.Instance.ShowMessage($"You picked up {item.GoldValue} gold.");
        }
        else
        {
            if (Inventory.Count >= 9)
            {
                RootScreen.Instance.ShowMessage("Your inventory is full, couldn't pick up item.");
                return;
            }

            Inventory.Add(item);
            RootScreen.Instance.ShowMessage($"You picked up {item.Id}.");
        }
        
        item.PickUp();
    }

    public void SetWeapon(Weapon weapon)
    {
        if (weapon == null) return;

        // Remove the previous weapon
        if (Weapon != null)
        {
            Inventory.Add(Weapon);
        }

        Weapon = weapon;
    }

    public void RemoveWeapon()
    {
        if (Weapon == null) return;

        Inventory.Add(Weapon);
        Weapon = null;
    }

    public void SetArmor(Armor armor)
    {
        if (Armor == null)
        {
            Armor = new Dictionary<Armor.ArmorSlot, Armor>();
        }

        Inventory.Remove(armor);

        if (Armor.ContainsKey(armor.Slot))
        {
            Armor previousArmor = Armor[armor.Slot];
            Inventory.Add(previousArmor);
            Armor[armor.Slot] = armor;
        }
        else
        {
            Armor.Add(armor.Slot, armor);
        }
    }

    public void RemoveArmor(Armor.ArmorSlot armorSlot)
    {
        if (Armor == null) return;

        if (Armor.ContainsKey(armorSlot))
        {
            Inventory.Add(Armor[armorSlot]);
            Armor.Remove(armorSlot);
        }
    }
}
