using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magecrypt;

public class RoomTemplate : Template
{
    public string[,] Layout;
    public int WallGlyph;
    public int FloorGlyph;
    public int TreasureWorth;
    public int Danger;

    public RoomTemplate(string[,] layout, int wallGlyph, int floorGlyph, int treasureWorth, int danger, int minLevel, int maxLevel, float chanceWeight) : base(minLevel, maxLevel, chanceWeight)
    {
        Layout = layout;
        WallGlyph = wallGlyph;
        FloorGlyph = floorGlyph;
        TreasureWorth = treasureWorth;
        Danger = danger;
    }

    public Room CreateRoom(Point position, Map map, int level, List<ItemTemplate> treasureTemplates, List<MonsterTemplate> monsterTemplates)
    {
        Dictionary<Point, List<GameObject>> roomContents = new Dictionary<Point, List<GameObject>>();
        for (int y = 0; y < Layout.GetLength(1); y++)
        {
            for (int x = 0; x < Layout.GetLength(0); x++)
            {
                string cell = Layout[x, y];
                Point cellPosition = new Point(position.X + x, position.Y + y);
                if (cell == "W") // Wall
                {
                    var wall = new GameObject("Wall", new ColoredGlyph(Color.Gray, Color.Black, WallGlyph), cellPosition, map, true);
                    map.AddGameObject(wall);
                    if (!roomContents.ContainsKey(cellPosition))
                        roomContents.Add(cellPosition, new List<GameObject>() { wall });
                    else
                        roomContents[cellPosition].Add(wall);
                }
                else if (cell == "F") // Floor
                {
                    var floor = new GameObject("Floor", new ColoredGlyph(Color.LightGray, Color.Black, FloorGlyph), cellPosition, map, false);
                    map.AddGameObject(floor);
                    if (!roomContents.ContainsKey(cellPosition))
                        roomContents.Add(cellPosition, new List<GameObject>() { floor });
                    else
                        roomContents[cellPosition].Add(floor);
                }
            }
        }

        // Treasure
        int remainingTreasureWorth = TreasureWorth;
        while (remainingTreasureWorth > 0)
        {
            var cellPosition = roomContents.Keys.Where(p => Layout[p.X - position.X, p.Y - position.Y] == "F").OrderBy(_ => Game.Instance.Random.Next()).First();
            var treasureTemplate = MapGenerator.ChooseRandom(treasureTemplates, level);
            var treasure = treasureTemplate.CreateItem(cellPosition, map);
            map.AddGameObject(treasure);
            roomContents[cellPosition].Add(treasure);
            remainingTreasureWorth -= treasureTemplate.GoldValue;
        }

        // Monsters
        int remainingDanger = Danger;
        while (remainingDanger > 0)
        {
            var cellPosition = roomContents.Keys.Where(p => Layout[p.X - position.X, p.Y - position.Y] == "F").OrderBy(_ => Game.Instance.Random.Next()).First();
            var enemyTemplate = MapGenerator.ChooseRandom(monsterTemplates, level);
            var enemy = enemyTemplate.CreateMonster(cellPosition, map);
            map.AddGameObject(enemy);
            roomContents[cellPosition].Add(enemy);
            remainingDanger -= enemyTemplate.Danger;
        }

        return new Room(roomContents);
    }
}

public class Room
{
    public Dictionary<Point, List<GameObject>> Contents;

    public Room(Dictionary<Point, List<GameObject>> contents)
    {
        Contents = contents;
    }
}
