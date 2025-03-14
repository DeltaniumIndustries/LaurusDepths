using XRL.World;
using XRL.World.ZoneBuilders;

public class FactionAffiliatedFortMaker
{
    protected string FactionOwner { get; }
    protected bool ClearCombatObjectsFirst { get; }
    protected string WallType { get; }
    protected string ZoneTable { get; }
    protected string Widgets { get; }

    public FactionAffiliatedFortMaker(string factionOwner, bool clearCombatObjectsFirst, string wallType, string zoneTable, string widgets)
    {
        FactionOwner = factionOwner;
        ClearCombatObjectsFirst = clearCombatObjectsFirst;
        WallType = wallType;
        ZoneTable = zoneTable;
        Widgets = widgets;
    }

    public virtual bool BuildZone(Zone zone)
    {
        return new LaurusBaseFortMaker().BuildZone(zone, ClearCombatObjectsFirst, WallType, ZoneTable, Widgets);
    }
}
