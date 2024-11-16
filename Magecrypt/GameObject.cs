namespace Magecrypt;

public class GameObject
{
    private ColoredGlyph _mapAppearance = new ColoredGlyph();

    public string Id { get; private set; }

    public Point Position { get; set; }

    public ColoredGlyph Appearance { get; set; }

    public bool BlocksMovement { get; private set; }

    public GameObject(string id, ColoredGlyph appearance, Point position, Map map, bool blocksMovement = false)
    {
        Id = id;
        Appearance = appearance;
        Position = position;
        BlocksMovement = blocksMovement;
    }

    public virtual void Tick(Map map)
    {

    }

    public virtual void DrawGameObject(Map map)
    {
        Appearance.CopyAppearanceTo(map.SurfaceObject.Surface[Position]);
        map.SurfaceObject.IsDirty = true;
    }
}