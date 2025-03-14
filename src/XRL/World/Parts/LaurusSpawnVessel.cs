using System;
using XRL.World.AI;

#nullable disable
namespace XRL.World.Parts
{
    [Serializable]
    public class LaurusSpawnVessel : IPart
    {
        public int SpawnTurns { get; set; } = int.MinValue;
        public string SpawnBlueprint { get; set; }
        public string ReplaceBlueprint { get; set; }
        public string SpawnMessage { get; set; }
        public string SpawnVerb { get; set; }
        public string SpawnAmount { get; set; } = "1";
        public string SpawnTime { get; set; } = "10";
        public string SpawnSound { get; set; }
        public GameObject SpawnedBy { get; set; }
        public bool AdjustAttitude { get; set; }
        public bool SlimesplatterOnSpawn { get; set; }
        public bool SpawnOnEmpty { get; set; }
        public int SpawnChance { get; set; } = 100;

        public override bool WantTurnTick() => true;

        public override void TurnTick(long TimeTick, int Amount)
        {
            if (ParentObject.CurrentCell is Cell currentCell)
            {
                Spawn(currentCell);
            }
        }

        private void Spawn(Cell cell)
        {
            if (SpawnTurns == int.MinValue)
            {
                SpawnTurns = SpawnTime.RollCached();
            }

            if (--SpawnTurns > 0) return;

            PlaySpawnSound(cell);
            EmitSpawnMessage();
            PerformSpawnVerb();
            ReplaceObject(cell);
            HandleSlimesplatter();

            ParentObject.Destroy();

            SpawnObjects(cell);
        }

        private void PlaySpawnSound(Cell cell)
        {
            if (!string.IsNullOrEmpty(SpawnSound))
            {
                cell.PlayWorldSound(SpawnSound);
            }
        }

        private void EmitSpawnMessage()
        {
            if (!string.IsNullOrEmpty(SpawnMessage))
            {
                EmitMessage(GameText.VariableReplace(SpawnMessage, ParentObject, SpawnedBy));
            }
        }

        private void PerformSpawnVerb()
        {
            if (!string.IsNullOrEmpty(SpawnVerb))
            {
                DidX(SpawnVerb);
            }
        }

        private void ReplaceObject(Cell cell)
        {
            if (!string.IsNullOrEmpty(ReplaceBlueprint))
            {
                cell.AddObject(ReplaceBlueprint);
            }
        }

        private void HandleSlimesplatter()
        {
            if (SlimesplatterOnSpawn)
            {
                ParentObject.Slimesplatter(false);
            }
        }

        private void SpawnObjects(Cell cell)
        {
            int spawnAmount = SpawnAmount.RollCached();

            for (int i = 0; i < spawnAmount; i++)
            {
                if (SpawnChance.in100())
                {
                    var spawnedObject = CreateSpawnedObject();
                    AddObjectToCell(cell, spawnedObject);
                }
            }
        }

        private GameObject CreateSpawnedObject()
        {
            var spawnedObject = GameObject.Create(SpawnBlueprint);
            GameObject spawnedBy = SpawnedBy;
            if (AdjustAttitude && GameObject.Validate(ref spawnedBy))
            {
                spawnedObject.TakeAllegiance<AllyBirth>(spawnedBy);
            }

            return spawnedObject;
        }

        private void AddObjectToCell(Cell cell, GameObject spawnedObject)
        {
            var spawnLocation = cell.GetConnectedSpawnLocation();
            if (spawnLocation == null) return;

            spawnLocation.AddObject(spawnedObject);

            if (spawnedObject.IsValid())
            {
                spawnedObject.PlayWorldSoundTag("AmbientIdleSound");
                spawnedObject.MakeActive();
            }
        }
    }
}
