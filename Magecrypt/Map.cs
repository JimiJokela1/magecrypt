using System.Collections.Generic;
using System.Linq;
using RogueSharp;
using RogueSharp.MapCreation;
using Point = SadRogue.Primitives.Point;

namespace Magecrypt;

public class Map
{
    public static Map Instance { get; private set; }
    private ScreenSurface _mapSurface;
    private Player _player;
    private List<GameObject> _mapObjects;
    private List<GameObject> _mapObjectsTickIterationCopy;

    private List<GameObject>[,] _mapObjectsByPosition;
    private int _mapWidth;
    private int _mapHeight;

    private int _currentLevel = 1;

    public IReadOnlyList<GameObject> GameObjects => _mapObjects.AsReadOnly();

    public ScreenSurface SurfaceObject => _mapSurface;

    public IMap RogueSharpMap { get; private set; }

    public Player Player
    {
        get => _player;
        set => _player = value;
    }

    public int Width => _mapWidth;
    public int Height => _mapHeight;
    public List<GameObject>[,] MapObjectsByPosition => _mapObjectsByPosition;

    public Map(int mapWidth, int mapHeight)
    {
        if (Instance != null) throw new System.Exception("Only one instance of Map is allowed.");

        Instance = this;

        _mapWidth = mapWidth;
        _mapHeight = mapHeight;

        _mapObjects = new List<GameObject>();
        _mapSurface = new ScreenSurface(mapWidth, mapHeight);
        _mapSurface.UseMouse = false;

        _mapObjectsByPosition = new List<GameObject>[mapWidth, mapHeight];
        _mapObjectsTickIterationCopy = new List<GameObject>();

        new MapGenerator();
        MapGenerator.Instance.GenerateMap(_currentLevel, this, _mapObjects, _mapObjectsByPosition, _mapSurface, mapWidth, mapHeight);

        RogueSharpMap = new RogueSharp.Map(mapWidth, mapHeight);

        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                RogueSharpMap.SetCellProperties(x, y, true, true);
            }
        }

        foreach (GameObject gameObject in _mapObjects)
        {
            if (gameObject.Id == "Wall")
            {
                RogueSharpMap.SetCellProperties(gameObject.Position.X, gameObject.Position.Y, false, false);
            }
        }
    }

    public void Draw()
    {
        _mapSurface.Clear();

        foreach (GameObject gameObject in _mapObjects)
        {
            gameObject.DrawGameObject(this);
        }
    }

    public bool TryMoveGameObject(GameObject gameObject, Point newPosition)
    {
        // Check new position is valid
        if (!_mapSurface.Surface.IsValidCell(newPosition.X, newPosition.Y)) return false;

        // Check if the new position is blocked
        if (!IsMoveAllowed(newPosition, gameObject))
        {
            // Check for attack
            if (gameObject is Character character)
            {
                List<GameObject> objects = _mapObjectsByPosition[newPosition.X, newPosition.Y];
                foreach (GameObject obj in objects)
                {
                    if (obj is Character target)
                    {
                        character.Attack(target);
                        return true;
                    }
                }
            }

            return false;
        }

        MoveGameObject(gameObject, newPosition);

        return true;
    }

    public void MoveGameObject(GameObject gameObject, Point newPosition)
    {
        _mapObjectsByPosition[gameObject.Position.X, gameObject.Position.Y].Remove(gameObject);
        _mapObjectsByPosition[newPosition.X, newPosition.Y].Add(gameObject);
        gameObject.Position = newPosition;

        gameObject.DrawGameObject(this);
    }

    public bool TryMovePlayer(Direction direction)
    {
        bool success = TryMoveGameObject(_player, _player.Position + direction);

        if (success)
        {
            // Check if the player is standing on an item
            List<GameObject> items = _mapObjects.FindAll(obj => obj.Position == _player.Position && obj is Item);
            if (items != null && items.Count > 0)
            {
                foreach (GameObject item in items)
                {
                    _player.PickUpItem(item as Item);
                }
            }

            // Check if the player is standing on a magic particle
            List<GameObject> particles = _mapObjects.FindAll(obj => obj.Position == _player.Position && obj is MagicParticle);
            if (particles != null && particles.Count > 0)
            {
                foreach (GameObject particle in particles)
                {
                    _player.PickUpMagicParticle(particle as MagicParticle);
                }
            }
        }

        return success;
    }

    public bool IsMoveAllowed(Point newPosition, GameObject gameObject)
    {
        if (!_mapSurface.Surface.IsValidCell(newPosition.X, newPosition.Y)) return false;

        return _mapObjectsByPosition[newPosition.X, newPosition.Y].All(obj => !obj.BlocksMovement);
    }

    public void AddGameObject(GameObject gameObject)
    {
        _mapObjects.Add(gameObject);
        _mapObjectsByPosition[gameObject.Position.X, gameObject.Position.Y].Add(gameObject);
    }

    public void RemoveGameObject(GameObject gameObject)
    {
        _mapObjects.Remove(gameObject);
        _mapObjectsByPosition[gameObject.Position.X, gameObject.Position.Y].Remove(gameObject);
        _mapSurface.IsDirty = true;
    }

    public void Tick()
    {
        _mapObjectsTickIterationCopy.Clear();
        _mapObjectsTickIterationCopy.AddRange(_mapObjects);
        foreach (GameObject gameObject in _mapObjectsTickIterationCopy)
        {
            if (gameObject != null)
            {
                gameObject.Tick(this);
            }
        }
    }
}