using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magecrypt;

public class LevelTemplate
{
    public string LevelName;
    public int Level;
    public List<RoomTemplate> RoomTemplates;
    public int MinRooms;
    public int MaxRooms;

    public LevelTemplate(string levelName, int level, List<RoomTemplate> roomTemplates, int minRooms, int maxRooms)
    {
        LevelName = levelName;
        Level = level;
        RoomTemplates = roomTemplates;
        MinRooms = minRooms;
        MaxRooms = maxRooms;
    }
}
