using ConsoleLib.Console;
using System;
using System.Collections.Generic;
using Wintellect.PowerCollections;
using XRL.Rules;

#nullable disable
namespace XRL.World.Parts.Mutation
{
    [Serializable]
    public class LaurusSporePuffer : BaseMutation
    {
        public int Chance = 100;
        public int EnergyCost = 1000;
        public int nCooldown;
        public string FungalGas;
        public string ColorString = "&G";

        public LaurusSporePuffer() => DisplayName = "Spore Fume";

        public override string GetLevelText(int Level) => "You exhale spores with precision and power.\n";

        public override bool WantEvent(int ID, int cascade)
        {
            return base.WantEvent(ID, cascade) || ID == SingletonEvent<BeginTakeActionEvent>.ID || ID == GetAdjacentNavigationWeightEvent.ID;
        }

        public override bool HandleEvent(BeginTakeActionEvent E)
        {
            // Use energy if the ParentObject is not the player
            if (!ParentObject.IsPlayer())
                UseEnergy(EnergyCost, "Fungus Puff");

            // Decrease the cooldown
            --nCooldown;

            // Check if the cooldown is over, and if the action should trigger based on chance
            if (nCooldown <= 0 && Chance > 0 && (Chance >= 100 || Stat.Random(1, 100) < Chance))
            {
                bool flag = false;
                List<Cell> localAdjacentCells = ParentObject.Physics.CurrentCell.GetLocalAdjacentCells();

                // Check adjacent cells for non-allied brains
                if (localAdjacentCells != null)
                {
                    foreach (Cell cell in localAdjacentCells)
                    {
                        if (cell.HasObjectWithPart("Brain"))
                        {
                            foreach (GameObject Object in cell.GetObjectsWithPart("Brain"))
                            {
                                if (ParentObject.Brain == null || !ParentObject.Brain.IsAlliedTowards(Object))
                                {
                                    flag = true;
                                    break;
                                }
                            }
                        }
                    }
                }

                // If flag is set, apply the spores to adjacent cells
                if (flag)
                {
                    ParentObject.ParticleBlip("&W*");

                    // Add the selected puff object to each adjacent cell
                    for (int index = 0; index < localAdjacentCells.Count; ++index)
                    {
                        // Use the PuffObject integer as an index for InfectionObjectList and PufferList
                        Gas part = localAdjacentCells[index].AddObject(FungalGas).GetPart<Gas>();
                        part.ColorString = ColorString;
                        part.Creator = ParentObject;
                    }

                    // Reset cooldown
                    nCooldown = 30 + RandomUtils.NextInt(10, 20);
                }
            }

            // Return true if energy cost is 0 or the parent is the player
            return (EnergyCost == 0 || ParentObject.IsPlayer()) && base.HandleEvent(E);
        }


        public override bool HandleEvent(GetAdjacentNavigationWeightEvent E)
        {
            if (!ParentObject.IsAlliedTowards(E.Actor))
                E.MinWeight(97);
            return base.HandleEvent(E);
        }

        public override void Register(GameObject Object, IEventRegistrar Registrar)
        {
            Registrar.Register("ApplySpores");
            base.Register(Object, Registrar);
        }

        public override bool FireEvent(Event E)
        {
            if (E.ID == "ApplySpores")
                return false;
            return base.FireEvent(E);
        }

        public override bool ChangeLevel(int NewLevel) => base.ChangeLevel(NewLevel);

        public override bool Mutate(GameObject GO, int Level)
        {
            ColorString = GO.Render.ColorString;
            return base.Mutate(GO, Level);
        }

        public override bool Unmutate(GameObject GO) => base.Unmutate(GO);
    }
}
