using System.Collections.Generic;
using XRL;
using XRL.Rules;
using XRL.World;
using XRL.World.ZoneBuilders;

public class LaurusBaseFortMaker : ZoneBuilderSandbox
{
    public bool BuildZone(Zone zone, bool clearCombatObjects, string wallObject, string zoneTable, string widgets)
    {
        var fortArea = GenerateFortArea();
        if (fortArea == null)
            return false;

        // Clear and prepare fort area
        zone.ClearBox(fortArea);
        if (clearCombatObjects)
            ClearCombatObjects(zone, fortArea);

        zone.FillBox(fortArea, "DirtFloor");
        zone.FillHollowBox(fortArea, wallObject);
        RemoveCanyonMarkers(zone, fortArea);

        // Create rooms inside the fort
        var rooms = GenerateRooms(zone, fortArea, wallObject);
        BuildRoomStructures(zone, rooms, wallObject);
        AddDoorsToRooms(zone, rooms, fortArea);

        // Populate zone with objects from tables
        PopulateZone(zone, zoneTable);
        PlaceWidgets(zone, widgets);

        // Add main fort doors
        AddFortDoors(zone, fortArea);
        AddCampfiresAndSeating(zone, rooms);
        AddStorageCrates(zone, rooms);

        return true;
    }

    private static Box GenerateFortArea()
    {
        var boxes = Tools.GenerateBoxes(BoxGenerateOverlap.Irrelevant, new Range(1, 1), new Range(30, 50), new Range(16, 24), null, new Range(1, 78), new Range(1, 23));
        return (boxes == null || boxes.Count == 0) ? null : boxes[0].Grow(-1);
    }

    private static void ClearCombatObjects(Zone zone, Box area)
    {
        for (int x = area.x1; x <= area.x2; x++)
        {
            for (int y = area.y1; y <= area.y2; y++)
            {
                Cell cell = zone.GetCell(x, y);
                var combatObjects = cell.GetObjectsWithPartReadonly("Combat");
                foreach (var obj in combatObjects)
                {
                    cell.RemoveObject(obj);
                }
            }
        }
    }

    private static void RemoveCanyonMarkers(Zone zone, Box area)
    {
        for (int x = area.x1; x <= area.x2; x++)
        {
            for (int y = area.y1; y <= area.y2; y++)
            {
                Cell cell = zone.GetCell(x, y);
                if (cell.HasObjectWithBlueprint("CanyonMarker"))
                {
                    cell.ClearWalls();
                }
            }
        }
    }


    private static List<Box> GenerateRooms(Zone zone, Box fortArea, string wallObject)
    {
        var rooms = new List<Box>();
        var possibleRooms = Tools.GenerateBoxes(new List<Box>(), BoxGenerateOverlap.NeverOverlap, new Range(1, 8), new Range(9, 40), new Range(8, 14), new Range(6, 999), new Range(fortArea.x1, fortArea.x2), new Range(fortArea.y1, fortArea.y2));

        foreach (var room in possibleRooms)
        {
            if (room.Valid && room.Grow(-1).Valid)
            {
                Box finalRoom = (room.x1 == fortArea.x1 || room.x2 == fortArea.x2 || room.y1 == fortArea.y1 || room.y2 == fortArea.y2) ? room : room.Grow(-1);
                zone.FillHollowBox(finalRoom, wallObject);
                rooms.Add(finalRoom);
            }
        }
        return rooms;
    }

    private static void BuildRoomStructures(Zone zone, List<Box> rooms, string wallObject)
    {
        foreach (var room in rooms)
        {
            if (room.Volume <= 25 || room.Height <= 4 || room.Width <= 4) continue;

            var template = new BuildingTemplate(room.Width, room.Height, 1, FullSquare: true);
            for (int x = 1; x < template.Width - 1; x++)
            {
                for (int y = 1; y < template.Height - 1; y++)
                {
                    Cell cell = zone.GetCell(x + room.x1, y + room.y1);
                    if (template.Map[x, y] == BuildingTemplateTile.Wall)
                        cell.AddObject(wallObject);
                    else if (template.Map[x, y] == BuildingTemplateTile.Door)
                        cell.AddObject("Door");
                }
            }
        }
    }

    private static void AddDoorsToRooms(Zone zone, List<Box> rooms, Box fortArea)
    {
        foreach (var room in rooms)
        {
            for (int attempt = 0; attempt < 1000; attempt++)
            {
                int x, y;
                if (Stat.Random(0, 1) == 0)
                {
                    x = Stat.Random(room.x1 + 1, room.x2 - 1);
                    y = (Stat.Random(0, 1) != 0) ? room.y2 : room.y1;
                }
                else
                {
                    y = Stat.Random(room.y1 + 1, room.y2 - 1);
                    x = (Stat.Random(0, 1) != 0) ? room.x2 : room.x1;
                }

                if (x != fortArea.x1 && x != fortArea.x2 && y != fortArea.y1 && y != fortArea.y2 &&
                    ((!zone.GetCell(x - 1, y).HasWall() && !zone.GetCell(x + 1, y).HasWall()) ||
                     (!zone.GetCell(x, y - 1).HasWall() && !zone.GetCell(x, y + 1).HasWall())))
                {
                    zone.GetCell(x, y).Clear();
                    zone.GetCell(x, y).AddObject("Door");
                    break;
                }
            }
        }
    }

    private static void PopulateZone(Zone zone, string zoneTable)
    {
        if (string.IsNullOrEmpty(zoneTable))
            return;

        foreach (string entry in zoneTable.Split(','))
        {
            foreach (var result in PopulationManager.Generate(entry, "zonetier", zone.NewTier.ToString()))
            {
                ZoneBuilderSandbox.PlaceObjectInArea(zone, zone.area, GameObjectFactory.Factory.CreateObject(result.Blueprint), 0, 0, result.Hint);
            }
        }
    }

    private static void PlaceWidgets(Zone zone, string widgets)
    {
        if (string.IsNullOrEmpty(widgets))
            return;

        foreach (string blueprint in widgets.Split(','))
        {
            zone.GetCell(0, 0).AddObject(GameObjectFactory.Factory.CreateObject(blueprint));
        }
    }

    private static void AddStorageCrates(Zone zone, List<Box> rooms)
    {
        foreach (var room in rooms)
        {
            int crateCount = Stat.Random(1, 3);
            for (int i = 0; i < crateCount; i++)
            {
                int x = Stat.Random(room.x1 + 1, room.x2 - 1);
                int y = Stat.Random(room.y1 + 1, room.y2 - 1);
                string variantName = "Chest" + (RandomUtils.NextBool() ? RandomUtils.NextInt(1, 8).ToString() : "");
                zone.GetCell(x, y).AddObject(variantName);
            }
        }
    }

    private static void AddCampfiresAndSeating(Zone zone, List<Box> rooms)
    {
        foreach (var room in rooms)
        {
            if (room.Width < 6 || room.Height < 6) continue; // Skip small rooms

            int fireX = (room.x1 + room.x2) / 2;
            int fireY = (room.y1 + room.y2) / 2;

            zone.GetCell(fireX, fireY).AddObject("Campfire");

            List<(int, int)> seatingOffsets = new()
        {
            (-1, -1), (-1, 1), (1, -1), (1, 1)
        };

            foreach (var (dx, dy) in seatingOffsets)
            {
                zone.GetCell(fireX + dx, fireY + dy).AddObject("Floor Cushion");
            }
        }
    }

    private static void AddFortDoors(Zone zone, Box fortArea)
    {
        int doorFlags = Stat.Random(1, 15);
        TryAddDoor(zone, fortArea.x1, fortArea.x2, fortArea.y1, doorFlags & 1);
        TryAddDoor(zone, fortArea.x1, fortArea.x2, fortArea.y2, doorFlags & 2);
        TryAddDoor(zone, fortArea.y1, fortArea.y2, fortArea.x1, doorFlags & 4);
        TryAddDoor(zone, fortArea.y1, fortArea.y2, fortArea.x2, doorFlags & 8);
    }

    private static void TryAddDoor(Zone zone, int min, int max, int fixedCoord, int flag)
    {
        if (flag == 0) return;

        int pos = Stat.Random(min + 1, max - 2);
        if (!zone.GetCell(pos, fixedCoord + 1).HasWall())
        {
            zone.GetCell(pos, fixedCoord).Clear();
            zone.GetCell(pos, fixedCoord).AddObject("Door");
        }
        if (!zone.GetCell(pos + 1, fixedCoord + 1).HasWall())
        {
            zone.GetCell(pos + 1, fixedCoord).Clear();
            zone.GetCell(pos + 1, fixedCoord).AddObject("Door");
        }
    }
}
