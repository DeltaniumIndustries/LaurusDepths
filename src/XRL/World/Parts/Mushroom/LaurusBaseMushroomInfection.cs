using System;
using XRL.Rules;
using XRL.World.Anatomy;

#nullable disable
namespace XRL.World.Parts
{
    [Serializable]
    public abstract class LaurusBaseMushroomInfection : IPart
    {
        public int ShroomCounter;
        public string InfectionType { get; set; }

        public string ManagerID;

        public LaurusBaseMushroomInfection()
        {
            ShroomCounter = GetNewShroomCounter();
            ManagerID =  GetManagerID();
        }

        public override bool SameAs(IPart p) => p is LaurusBaseMushroomInfection other;

        public override bool WantEvent(int ID, int cascade)
        {
            return base.WantEvent(ID, cascade) || ID == SingletonEvent<EndTurnEvent>.ID || ID == EquippedEvent.ID || ID == UnequippedEvent.ID || ID == OnDestroyObjectEvent.ID;
        }

        public override bool HandleEvent(EndTurnEvent E)
        {
            --ShroomCounter;
            if (ShroomCounter < 0)
            {
                TryDoFungalInfectionLogic();
                ShroomCounter = GetNewShroomCounter();
            }
            return base.HandleEvent(E);
        }

        public override bool HandleEvent(EquippedEvent E)
        {
            BodyPart bodyPart1 = ParentObject?.EquippedOn();
            if (bodyPart1 != null)
            {
                BodyPart bodyPart2 = bodyPart1;
                string managerId = ManagerID;
                bool? nullable = new bool?(true);
                string[] strArray = new string[2] { "Feet", "Roots" };
                int? Category = new int?();
                int? RequiresLaterality = new int?();
                int? Mobility = new int?();
                bool? Appendage = new bool?();
                bool? Integral = new bool?();
                bool? Mortal = new bool?();
                bool? Abstract = new bool?();
                bool? Extrinsic = nullable;
                bool? Dynamic = new bool?();
                bool? Plural = new bool?();
                bool? Mass = new bool?();
                bool? Contact = new bool?();
                bool? IgnorePosition = new bool?();
                string[] OrInsertBefore = strArray;
                bodyPart2.AddPartAt("Icy Outcrop", Manager: managerId, Category: Category, RequiresLaterality: RequiresLaterality, Mobility: Mobility, Appendage: Appendage, Integral: Integral, Mortal: Mortal, Abstract: Abstract, Extrinsic: Extrinsic, Dynamic: Dynamic, Plural: Plural, Mass: Mass, Contact: Contact, IgnorePosition: IgnorePosition, InsertAfter: "Icy Outcrop", OrInsertBefore: OrInsertBefore);
                TryDoFungalInfectionLogic();
            }
            return base.HandleEvent(E);
        }

        public override bool HandleEvent(UnequippedEvent E)
        {
            E.Actor.RemoveBodyPartsByManager(ManagerID);
            return base.HandleEvent(E);
        }

        public override bool HandleEvent(OnDestroyObjectEvent E)
        {
            ParentObject?.Equipped?.RemoveBodyPartsByManager(ManagerID, true);
            return base.HandleEvent(E);
        }

        protected abstract void TryDoFungalInfectionLogic();
        protected abstract int GetNewShroomCounter();

        protected abstract string GetManagerID();







    }
}
