using XRL.World;

public class JoppaFortMaker : FactionAffiliatedFortMaker
{
    public JoppaFortMaker() 
        : base("Joppa", clearCombatObjectsFirst: true, wallType: "WoodWall", zoneTable: "Joppa Outskirts", widgets: "Watervine") {}

    public override bool BuildZone(Zone zone)
    {
        bool built = base.BuildZone(zone);
        if (built)
        {
            AddRobotDefenses(zone);
            InstallPowerCores(zone);
        }
        return built;
    }

    private void AddRobotDefenses(Zone zone)
    {
        // Place turrets and defensive structures
        foreach (var cell in zone.GetCells())
        {
            if (cell.IsEdge() && RandomUtils.NextBool())  // Place turrets at the perimeter
            {
                cell.AddObject("ChaingunTurret");
            }
        }
    }

    private void InstallPowerCores(Zone zone)
    {
        // Place energy sources in key areas
        foreach (var cell in zone.GetCells())
        {
            if (RandomUtils.NextInt(0, 100) < 5) // 5% chance per cell to have a power core
            {
                cell.AddObject("Sconce");
            }
        }
    }
}
