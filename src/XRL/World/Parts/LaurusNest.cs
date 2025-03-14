using System;
using XRL.World.AI;

#nullable disable
namespace XRL.World.Parts
{
    [Serializable]
    public class LaurusSpawnNest : IPart
    {
        public int NumberToSpawn = 30;
        public int ChancePerSpawn = 100;
        public float XPFactor = 0.25f;
        public bool CollapseAfterSpawn = false;
        public string TurnsPerSpawn = "15-20";
        public string NumberSpawned = "1";
        public string SpawnMessage = "A giant centipede crawls out of the nest.";
        public string CollapseMessage = "The nest collapses.";
        public string SpawnParticle = "&w.";
        public string BlueprintSpawned = "Giant Centipede";
        private int SpawnCooldown = int.MinValue;

        public override bool WantTurnTick() => true;

        public override bool AllowStaticRegistration() => true;

        public override void Register(GameObject Object, IEventRegistrar Registrar)
        {
            base.Register(Object, Registrar);
        }

        public override void TurnTick(long TimeTick, int Amount)
        {
            if (NumberToSpawn <= 0)
            {
                HandleNestCollapse();
                return;
            }

            SpawnCooldown = SpawnCooldown == int.MinValue ? TurnsPerSpawn.RollCached() : SpawnCooldown - 1;

            if (SpawnCooldown <= 0)
                SpawnCreatures();
        }

        private void HandleNestCollapse()
        {
            if (CollapseAfterSpawn)
            {
                if (Visible())
                {
                    AddPlayerMessage(CollapseMessage);
                }
                ParentObject.Destroy();
            }
            else
            {
                ParentObject.RemovePart(this);
            }
        }

        private void SpawnCreatures()
        {
            for (int i = 0; i < NumberSpawned.RollCached(); ++i)
            {
                var localCell = ParentObject.CurrentCell.GetRandomLocalAdjacentCell();
                if (localCell != null && localCell.IsEmpty() && ChancePerSpawn.in100())
                {
                    if (NumberToSpawn > 0)
                    {
                        --NumberToSpawn;
                    }
                    CreateSpawnedObject(localCell);
                    if (Visible())
                    {
                        AddPlayerMessage(SpawnMessage);
                    }
                    ParentObject.Slimesplatter(false, SpawnParticle);
                    SpawnCooldown = TurnsPerSpawn.RollCached();
                }
            }
        }

        private GameObject CreateSpawnedObject(Cell localCell)
        {
            var spawnedObject = GameObject.Create(BlueprintSpawned);
            if (spawnedObject.HasStat("XPValue"))
            {
                var xpValue = spawnedObject.GetStat("XPValue");
                xpValue.BaseValue = (int)Math.Round(xpValue.BaseValue * XPFactor / 5.0) * 5;
            }

            spawnedObject.TakeAllegiance<AllyBirth>(ParentObject);
            spawnedObject.MakeActive();
            localCell.AddObject(spawnedObject);
            return spawnedObject;
        }
    }
}
