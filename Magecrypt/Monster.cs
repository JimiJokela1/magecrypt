
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