using RogueSharp;
using RogueSharp.MapCreation;
using System.Collections.Generic;
using System.Linq;
using RogueSharp.Random;
using Point = SadRogue.Primitives.Point;
using Rectangle = RogueSharp.Rectangle;

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
    private const int PotionCount = 5;
    private const int ArmorCount = 3;

    private const int CaveMapGenFillProbability = 50;
    private const int CaveMapGenTotalIterations = 5;
    private const int CaveMapGenCutOffOfBigAreaFill = 7;

    private List<ArmorTemplate> _randomArmor = new List<ArmorTemplate>()
    {
        new ArmorTemplate("Acolyte Robes", new ColoredGlyph(Color.AnsiBlueBright, Color.Transparent, 'A'), Armor.ArmorSlot.Chest, 1, 1, 5, 1f),
        new ArmorTemplate("Acolyte Hood", new ColoredGlyph(Color.AnsiBlueBright, Color.Transparent, 'H'), slot: Armor.ArmorSlot.Head, defense: 1, 1, 5, 1f),
        new ArmorTemplate("Acolyte Gloves", new ColoredGlyph(Color.AnsiBlueBright, Color.Transparent, 'G'), slot: Armor.ArmorSlot.Hands, defense: 1, 1, 5, 1f),
        new ArmorTemplate("Acolyte Pants", new ColoredGlyph(Color.AnsiBlueBright, Color.Transparent, 'P'),  slot: Armor.ArmorSlot.Legs, defense: 1, 1, 5, 1f),
        new ArmorTemplate("Acolyte Shoes", new ColoredGlyph(Color.AnsiBlueBright, Color.Transparent, 'S'),  slot: Armor.ArmorSlot.Feet, defense: 1, 1, 5, 1f),
        new ArmorTemplate("Sorcerer Robes", new ColoredGlyph(Color.AnsiMagentaBright, Color.Transparent, 'A'), slot: Armor.ArmorSlot.Chest, defense: 2, 3, 8, 1f),
        new ArmorTemplate("Sorcerer Hat", new ColoredGlyph(Color.AnsiMagentaBright, Color.Transparent, 'H'), slot: Armor.ArmorSlot.Head, defense: 2, 3, 8, 1f),
        new ArmorTemplate("Sorcerer Gloves", new ColoredGlyph(Color.AnsiMagentaBright, Color.Transparent, 'G'), slot: Armor.ArmorSlot.Hands, defense: 2, 3, 8, 1f),
        new ArmorTemplate("Sorcerer Pants", new ColoredGlyph(Color.AnsiMagentaBright, Color.Transparent, 'P'), slot: Armor.ArmorSlot.Legs, defense: 2, 3, 8, 1f),
        new ArmorTemplate("Sorcerer Shoes", new ColoredGlyph(Color.AnsiMagentaBright, Color.Transparent, 'S'), slot: Armor.ArmorSlot.Feet, defense: 2, 3, 8, 1f),
    };

    private List<WeaponTemplate> _randomWeapons = new List<WeaponTemplate>()
    {
        new WeaponTemplate("Dagger", new ColoredGlyph(Color.AnsiWhite, Color.Transparent, 'd'), 1, 1, 5, 1f),
        new WeaponTemplate("Short Sword", new ColoredGlyph(Color.AnsiWhite, Color.Transparent, 's'), 2, 3, 8, 1f),
        new WeaponTemplate("Long Sword", new ColoredGlyph(Color.AnsiWhite, Color.Transparent, 'l'), 3, 5, 10, 1f),
        new WeaponTemplate("Great Sword", new ColoredGlyph(Color.AnsiWhite, Color.Transparent, 'g'), 5, 7, 15, 1f),
        new WeaponTemplate("Staff", new ColoredGlyph(Color.AnsiWhite, Color.Transparent, 'S'), 1, 1, 5, 1f),
        new WeaponTemplate("Wand", new ColoredGlyph(Color.AnsiWhite, Color.Transparent, 'W'), 1, 1, 5, 1f),
        new WeaponTemplate("Scepter", new ColoredGlyph(Color.AnsiWhite, Color.Transparent, 'C'), 1, 1, 5, 1f),
        new WeaponTemplate("Mace", new ColoredGlyph(Color.AnsiWhite, Color.Transparent, 'M'), 2, 3, 8, 1f),
        new WeaponTemplate("Axe", new ColoredGlyph(Color.AnsiWhite, Color.Transparent, 'A'), 3, 5, 10, 1f),
        new WeaponTemplate("Warhammer", new ColoredGlyph(Color.AnsiWhite, Color.Transparent, 'H'), 5, 7, 15, 0.25f),
    };

    private List<HealingPotionTemplate> _randomPotions = new List<HealingPotionTemplate>()
    {
        new HealingPotionTemplate("Minor Healing Potion", new ColoredGlyph(Color.AnsiRed, Color.Transparent, 'h'), 25, 1, 5, 1f),
        new HealingPotionTemplate("Lesser Healing Potion", new ColoredGlyph(Color.AnsiRed, Color.Transparent, 'H'), 50, 3, 8, 1f),
        new HealingPotionTemplate("Healing Potion", new ColoredGlyph(Color.AnsiRed, Color.Transparent, 'H'), 75, 5, 10, 1f),
        new HealingPotionTemplate("Greater Healing Potion", new ColoredGlyph(Color.AnsiRed, Color.Transparent, 'H'), 100, 7, 15, 1f),
        new HealingPotionTemplate("Major Healing Potion", new ColoredGlyph(Color.AnsiRed, Color.Transparent, 'H'), 150, 10, 20, 1f),
    };

    private List<ItemTemplate> _randomTreasures = new List<ItemTemplate>()
    {
        new ItemTemplate("Gold", new ColoredGlyph(Color.AnsiYellow, Color.Transparent, 'g'), 10, false, false, 1, 26, 10f),
        new ItemTemplate("Crown", new ColoredGlyph(Color.AnsiYellow, Color.Transparent, 'C'), 50, false, false, 2, 26, 0.5f),
        new ItemTemplate("Scepter", new ColoredGlyph(Color.AnsiYellow, Color.Transparent, 'S'), 100, false, false, 5, 26, 0.5f),
        new ItemTemplate("Jewel", new ColoredGlyph(Color.AnsiYellow, Color.Transparent, 'J'), 500, false, false, 5, 26, 0.25f),
        new ItemTemplate("Orb", new ColoredGlyph(Color.AnsiYellow, Color.Transparent, 'O'), 1000, false, false, 10, 26, 0.1f),
    };

    private List<MonsterTemplate> _randomMonsters = new List<MonsterTemplate>()
    {
        new MonsterTemplate("Goblin", new ColoredGlyph(Color.GreenYellow, Color.Transparent, 'G'), 15,10, 0, 0, 15, 2f),
        new MonsterTemplate("Orc", new ColoredGlyph(Color.DarkGreen, Color.Transparent, 'O'), 25, 15, 1, 3, 26, 1f),
        new MonsterTemplate("Troll", new ColoredGlyph(Color.DarkOliveGreen, Color.Transparent, 'T'), 60, 40, 0, 5, 26, 0.2f),
        new MonsterTemplate("Ogre", new ColoredGlyph(Color.Orange, Color.Transparent, 'O'), 60, 25, 3, 7, 26, 0.2f),
        new MonsterTemplate("Cyclops", new ColoredGlyph(Color.Yellow, Color.Transparent, 'C'), 75, 30, 0, 10, 26, 0.1f),
    };

    private List<RoomTemplate> _randomRooms = new List<RoomTemplate>()
    {
        new RoomTemplate(new string[5, 5]
        {
            { "W", "W", "F", "W", "W" },
            { "W", "F", "F", "F", "W" },
            { "F", "F", "W", "F", "F" },
            { "W", "F", "F", "F", "W" },
            { "W", "W", "F", "W", "W" },
        }, 35, 46, 150, 50, 0, 10, 100),
        new RoomTemplate(new string[7, 7]
        {
            { "W", "W", "W", "F", "W", "W", "W" },
            { "W", "F", "F", "F", "F", "F", "W" },
            { "W", "F", "W", "F", "W", "F", "W" },
            { "F", "F", "F", "F", "F", "F", "F" },
            { "W", "F", "W", "F", "W", "F", "W" },
            { "W", "F", "F", "F", "F", "F", "W" },
            { "W", "W", "W", "F", "W", "W", "W" },
        }, 35, 46, 300, 75, 0, 10, 100),
        new RoomTemplate(new string[9, 9]
        {
            { "W", "W", "W", "W", "F", "W", "W", "W", "W" },
            { "W", "F", "F", "F", "F", "F", "F", "F", "W" },
            { "F", "F", "F", "F", "F", "F", "F", "F", "F" },
            { "W", "F", "F", "F", "F", "F", "F", "F", "W" },
            { "W", "F", "F", "F", "F", "F", "F", "F", "W" },
            { "W", "F", "F", "F", "F", "F", "F", "F", "W" },
            { "F", "F", "F", "F", "F", "F", "F", "F", "F" },
            { "W", "F", "F", "F", "F", "F", "F", "F", "W" },
            { "W", "W", "W", "W", "F", "W", "W", "W", "W" },
        }, 35, 46, 200, 50, 0, 10, 100)
    };

    private T ChooseRandom<T>(List<T> templates, int level) where T : Template
    {
        List<T> validTemplates = templates.Where(t => t.MinLevel <= level && t.MaxLevel >= level).ToList();
        float totalWeight = validTemplates.Sum(t => t.ChanceWeight);
        float randomWeight = Game.Instance.Random.NextSingle() * totalWeight;
        foreach (T template in validTemplates)
        {
            randomWeight -= template.ChanceWeight;
            if (randomWeight <= 0)
            {
                return template;
            }
        }

        // Shouldn't happen
        return null;
    }

    public void GenerateMap(int level, Map map, List<GameObject> mapObjects, List<GameObject>[,] mapObjectsByPosition, ScreenSurface mapSurface, int mapWidth, int mapHeight)
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
        //RogueSharp.Map generatedMap = RogueSharp.Map.Create(
        //    new CaveMapCreationStrategy<RogueSharp.Map>(
        //        mapSurface.Width,
        //        mapSurface.Height,
        //        CaveMapGenFillProbability,
        //        CaveMapGenTotalIterations,
        //        CaveMapGenCutOffOfBigAreaFill));

        //RogueSharp.Map gen =
        //    RogueSharp.Map.Create(new RandomRoomsMapCreationStrategy<RogueSharp.Map>(mapWidth, mapHeight, 15, 10, 3, new DotNetRandom(1253)));

        RogueSharp.Map gen2 =
            RogueSharp.Map.Create(new BorderOnlyMapCreationStrategy<RogueSharp.Map>(mapWidth, mapHeight));

        // Create walls
        for (int x = 0; x < mapSurface.Width; x++)
        {
            for (int y = 0; y < mapSurface.Height; y++)
            {
                if (!gen2.GetCell(x, y).IsWalkable)
                    map.AddGameObject(new GameObject("Wall", new ColoredGlyph(Color.DarkGray, Color.Black, 35), new Point(x, y), map, blocksMovement: true));
            }
        }

        // Create random rooms
        for (int i = 0; i < 50; i++)
        {
            //Rectangle room = GetRandomRoom(map.Width, map.Height, 3, 8, 3, 8);
            //List<GameObject> roomFloors = CreateRoom(map, room, true);

            //// Distribute loot based on the room profile
            //foreach (GameObject floor in roomFloors)
            //{
            //    // 10% chance to place a treasure
            //    if (Game.Instance.Random.Next(0, 100) < 10)
            //    {
            //        CreateTreasure(map, level);
            //    }
            //    // 10% chance to place a monster
            //    if (Game.Instance.Random.Next(0, 100) < 10)
            //    {
            //        CreateMonster(map, level);
            //    }
            //}

            RoomTemplate roomTemplate = ChooseRandom(_randomRooms, level);
            Point randomPosition = new Point(Game.Instance.Random.Next(1, map.Width - roomTemplate.Layout.GetLength(0)), Game.Instance.Random.Next(1, map.Height - roomTemplate.Layout.GetLength(1)));
            Room room = roomTemplate.CreateRoom(randomPosition, map, _randomTreasures, _randomMonsters);


        }

        //// Create treasures
        //for (int i = 0; i < TreasureCount; i++)
        //{
        //    CreateTreasure(map, level);
        //}

        // Create magic particles
        for (int i = 0; i < 50; i++)
        {
            CreateMagicParticle(map);
        }

        //// Create monsters
        //for (int i = 0; i < MonsterCount; i++)
        //{
        //    CreateMonster(map, level);
        //}

        // Create random armor
        for (int i = 0; i < ArmorCount; i++)
        {
            CreateRandomArmor(map, level);
        }

        // Create minor healing potions
        for (int i = 0; i < PotionCount; i++)
        {
            CreateHealingPotion(map, level);
        }

        // Create the player
        map.Player = new Player("Player", new ColoredGlyph(Color.White, Color.Transparent, '@'), mapSurface.Surface.Area.Center, map, blocksMovement: true, health: 100, mana: 0, maxMana: 10, attack: 10, defense: 0);
        map.AddGameObject(map.Player);
    }

    private void CreateHealingPotion(Map map, int level)
    {
        HealingPotionTemplate healingPotionTemplate = ChooseRandom(_randomPotions, level);
        TryPlaceGameObjectInRandomPosition(healingPotionTemplate.CreateHealingPotion(Point.Zero, map), map);
    }

    private Rectangle GetRandomRoom(int mapWidth, int mapHeight, int minRoomWidth, int maxRoomWidth, int minRoomHeight, int maxRoomHeight)
    {
        int roomWidth = Game.Instance.Random.Next(minRoomWidth, maxRoomWidth);
        int roomHeight = Game.Instance.Random.Next(minRoomHeight, maxRoomHeight);
        int roomX = Game.Instance.Random.Next(0, mapWidth - roomWidth);
        int roomY = Game.Instance.Random.Next(0, mapHeight - roomHeight);

        return new Rectangle(roomX, roomY, roomWidth, roomHeight);
    }

    private List<GameObject> CreateRoom(Map map, Rectangle room, bool returnOnlyFloors = false)
    {
        List<GameObject> roomObjects = new List<GameObject>();

        for (int x = room.Left; x < room.Right; x++)
        {
            for (int y = room.Top; y < room.Bottom; y++)
            {
                if (x == room.Left || x == room.Right - 1 || y == room.Top || y == room.Bottom - 1)
                {
                    GameObject wall = new GameObject("Wall", new ColoredGlyph(Color.DarkGray, Color.Black, 35), new Point(x, y), map, blocksMovement: true);
                    if (!returnOnlyFloors)
                        roomObjects.Add(wall);
                    map.AddGameObject(wall);
                }
                else
                {
                    GameObject floor = new GameObject("Floor", new ColoredGlyph(Color.DarkGray, Color.AnsiBlackBright, 46),
                        new Point(x, y), map, blocksMovement: false);
                    roomObjects.Add(floor);
                    map.AddGameObject(floor);
                }
            }
        }

        return roomObjects;
    }

    private void CreateMonster(Map map, int level)
    {
        GameObject monster = ChooseRandom(_randomMonsters, level).CreateMonster(Point.Zero, map);
        TryPlaceGameObjectInRandomPosition(monster, map);
    }

    private void CreateTreasure(Map map, int level)
    {
        GameObject treasure = ChooseRandom(_randomTreasures, level).CreateItem(Point.Zero, map);
        TryPlaceGameObjectInRandomPosition(treasure, map);
    }

    private void CreateMagicParticle(Map map)
    {
        TryPlaceGameObjectInRandomPosition(new MagicParticle(Point.Zero, map, manaValue: Game.Instance.Random.Next(1, 3)), map);
    }

    private void CreateRandomArmor(Map map, int level)
    {
        GameObject armor = ChooseRandom(_randomArmor, level).CreateArmor(Point.Zero, map);
        TryPlaceGameObjectInRandomPosition(armor, map);
    }

    private bool TryPlaceGameObjectInRandomPosition(GameObject gameObject, Map map)
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