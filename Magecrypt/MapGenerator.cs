using RogueSharp;
using RogueSharp.MapCreation;
using System.Collections.Generic;
using System.Linq;
using Point = SadRogue.Primitives.Point;

namespace Magecrypt;

public class MapGenerator
{
    public static MapGenerator Instance { get; private set; }

    public MapGenerator()
    {
        if (Instance != null) throw new System.Exception("Only one instance of MapGenerator is allowed.");

        Instance = this;
    }
    
    private const int MonsterCount = 10;
    private const int TreasureCount = 10;
    private const int MinorHealingPotionCount = 10;

    private const int CaveMapGenFillProbability = 50;
    private const int CaveMapGenTotalIterations = 5;
    private const int CaveMapGenCutOffOfBigAreaFill = 7;
    
    public void GenerateMap(Map map, List<GameObject> mapObjects, List<GameObject>[,] mapObjectsByPosition, ScreenSurface mapSurface, int mapWidth, int mapHeight)
    {
        // Clear the map
        mapObjects.Clear();
        mapSurface.Clear();

        // Clear the map objects by position
        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                if (mapObjectsByPosition[x, y] == null)
                    mapObjectsByPosition[x, y] = new List<GameObject>();
                else
                    mapObjectsByPosition[x, y].Clear();
            }
        }

        // Generate a new map
        RogueSharp.Map generatedMap = RogueSharp.Map.Create(
            new CaveMapCreationStrategy<RogueSharp.Map>(
                mapSurface.Width,
                mapSurface.Height,
                CaveMapGenFillProbability,
                CaveMapGenTotalIterations,
                CaveMapGenCutOffOfBigAreaFill));

        // Create walls
        for (int x = 0; x < mapSurface.Width; x++)
        {
            for (int y = 0; y < mapSurface.Height; y++)
            {
                if (!generatedMap.GetCell(x, y).IsWalkable)
                    map.AddGameObject(new GameObject("Wall", new ColoredGlyph(Color.DarkGray, Color.Black, 35), new Point(x, y), map, blocksMovement: true));
            }
        }

        // Create treasures
        for (int i = 0; i < TreasureCount; i++)
        {
            CreateTreasure(map);
        }

        // Create magic particles
        for (int i = 0; i < 50; i++)
        {
            CreateMagicParticle(map);
        }

        // Create monsters
        for (int i = 0; i < MonsterCount; i++)
        {
            CreateMonster(map);
        }

        // Create random armor
        CreateRandomArmor(map);

        // Create minor healing potions
        for (int i = 0; i < MinorHealingPotionCount; i++)
        {
            TryPlaceGameObject(new HealingPotion("Minor Healing Potion", 50, Point.Zero, map), map);
        }

        // Create the player
        map.Player = new Player("Player", new ColoredGlyph(Color.White, Color.Transparent, '@'), mapSurface.Surface.Area.Center, map, blocksMovement: true, health: 100, mana: 0, maxMana: 10, attack: 10, defense: 0);
        map.AddGameObject(map.Player);
    }

    private void CreateMonster(Map map)
    {
        TryPlaceGameObject(new Monster("Goblin", new ColoredGlyph(Color.Green, Color.Transparent, 'G'), Point.Zero, map, attack: 10, defense: 0, health: 15), map);
    }

    private void CreateTreasure(Map map)
    {
        TryPlaceGameObject(new Item("Treasure", new ColoredGlyph(Color.Yellow, Color.Black, 'v'), Point.Zero, map, blocksMovement: false, goldValue: 10), map);
    }

    private void CreateMagicParticle(Map map)
    {
        TryPlaceGameObject(new MagicParticle(Point.Zero, map, manaValue: 1), map);
    }

    private void CreateRandomArmor(Map map)
    {
        TryPlaceGameObject(new Armor("Acolyte Robes", new ColoredGlyph(Color.AnsiBlueBright, Color.Transparent, 'A'), Point.Zero, map, slot: Armor.ArmorSlot.Chest, defense: 1), map);
        TryPlaceGameObject(
            new Armor("Acolyte Hood", new ColoredGlyph(Color.AnsiBlueBright, Color.Transparent, 'H'), Point.Zero, map,
                slot: Armor.ArmorSlot.Head, defense: 1), map);
        TryPlaceGameObject(new Armor("Acolyte Gloves", new ColoredGlyph(Color.AnsiBlueBright, Color.Transparent, 'G'), Point.Zero, map,
            slot: Armor.ArmorSlot.Hands, defense: 1), map);
        TryPlaceGameObject(new Armor("Acolyte Pants", new ColoredGlyph(Color.AnsiBlueBright, Color.Transparent, 'P'), Point.Zero, map, slot: Armor.ArmorSlot.Legs, defense: 1), map);
        TryPlaceGameObject(new Armor("Acolyte Shoes", new ColoredGlyph(Color.AnsiBlueBright, Color.Transparent, 'S'), Point.Zero, map, slot: Armor.ArmorSlot.Feet, defense: 1), map);
    }

    private bool TryPlaceGameObject(GameObject gameObject, Map map)
    {
        // Try 1000 times to get an empty map position
        for (int i = 0; i < 1000; i++)
        {
            // Get a random position
            Point randomPosition = new Point(Game.Instance.Random.Next(0, map.Width),
                Game.Instance.Random.Next(0, map.Height));

            // Check if any object is already positioned there, repeat the loop if found
            bool foundObject = map.MapObjectsByPosition[randomPosition.X, randomPosition.Y].Any(obj => obj.BlocksMovement);
            if (foundObject) continue;

            // If the code reaches here, we've got a good position, add the game object.
            map.AddGameObject(gameObject);
            map.MoveGameObject(gameObject, randomPosition);
            return true;
        }

        return false;
    }
}