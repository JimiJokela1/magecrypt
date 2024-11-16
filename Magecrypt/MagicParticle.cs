using Microsoft.Xna.Framework;
using Color = SadRogue.Primitives.Color;
using Game = SadConsole.Game;
using Point = SadRogue.Primitives.Point;

namespace Magecrypt;

public class MagicParticle : GameObject
{
    public class SpeedVector
    {
        public Direction Direction;
        public float Speed;

        public Vector2 ToVector2()
        {
            return new Vector2(Direction.DeltaX, Direction.DeltaY) * Speed;
        }
    }

    private Vector2 _cellSubPosition;
    private SpeedVector _speed;

    public MagicParticle(string id, ColoredGlyph appearance, Point position, Map map, bool blocksMovement = false) : base(id, appearance, position, map, blocksMovement)
    {
    }

    public MagicParticle(Point position, Map map, int manaValue) : base("MagicParticle", new ColoredGlyph(Color.Yellow, Color.Black, 42), position, map, blocksMovement: false)
    {
        _speed = new SpeedVector()
        {
            Direction = Direction.GetDirection(Game.Instance.Random.Next(-1, 2), Game.Instance.Random.Next(-1, 2)),
            Speed = Game.Instance.Random.NextSingle(),
        };
        _cellSubPosition = Vector2.Zero;
        ManaValue = manaValue;
    }

    public int ManaValue { get; set; }

    public override void DrawGameObject(Map map)
    {
        Appearance.CopyAppearanceTo(map.SurfaceObject.Surface[Position]);
        map.SurfaceObject.IsDirty = true;
    }

    public override void Tick(Map map)
    {
        Point newPosition = new Point(Position.X + Game.Instance.Random.Next(-1, 2), Position.Y + Game.Instance.Random.Next(-1, 2));
        if (map.IsMoveAllowed(newPosition, this) && map.TryMoveGameObject(this, newPosition))
        {

        }
    }

    public void PickUp()
    {
        // Remove the item from the map
        Map.Instance.RemoveGameObject(this);
    }
}