
namespace Magecrypt;

public class Monster : Character
{
    public Monster(string id, ColoredGlyph appearance, Point position, Map map, bool blocksMovement = false) : base(id, appearance, position, map, blocksMovement)
    {
    }

    public Monster(string id, ColoredGlyph appearance, Point position, Map map, int health, int attack, int defense) : base(id, appearance, position, map, blocksMovement: true, health: health, attack: attack, defense: defense)
    {
    }

    public override void Tick(Map map)
    {
        base.Tick(map);

        // Move towards player
        Point playerPosition = map.Player.Position;
        if (playerPosition != Point.None)
        {
            Direction moveDirection = Direction.GetDirection(Position, playerPosition);
            if (map.TryMoveGameObject(this, Position + moveDirection))
            {
                
            }
        }
    }
}

public class MonsterTemplate : Template
{
    public string Name { get; set; }
    public ColoredGlyph Appearance { get; set; }
    public int Health { get; set; }
    public int Attack { get; set; }
    public int Defense { get; set; }

    public MonsterTemplate(string name, ColoredGlyph appearance, int health, int attack, int defense, int minLevel, int maxLevel, float chanceWeight) : base(minLevel, maxLevel, chanceWeight)
    {
        Name = name;
        Appearance = appearance;
        Health = health;
        Attack = attack;
        Defense = defense;
    }

    public Monster CreateMonster(Point position, Map map)
    {
        return new Monster(Name, Appearance, position, map, Health, Attack, Defense);
    }
}