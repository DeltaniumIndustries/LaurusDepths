using System;
using XRL.World.Anatomy;

namespace XRL.World.Parts
{

    [Serializable]
    public class LaurusFungalInfectionVoidspore : LaurusBaseMushroomInfection
    {
        public LaurusFungalInfectionVoidspore()
        {
            InfectionType = "VoidsporeInfection";
        }

        protected override string GetManagerID()
        {
            return ParentObject?.ID + "::VoidsporeInfection";
        }

        protected override int GetNewShroomCounter()
        {
            return RandomUtils.NextInt(200, 400);
        }

        protected override void TryDoFungalInfectionLogic()
        {
            BodyPart equippedItem = ParentObject?.Equipped?.Body?.FindEquippedItem(ParentObject);
            if (equippedItem == null)
                return;
            try
            {
                BodyPart byManager = equippedItem.FindByManager(ManagerID);
                if (byManager == null || byManager.Equipped != null)
                    return;
                TryGrowMushroom(byManager);
            }
            catch (Exception ex)
            {
                MetricsManager.LogException(nameof(TryGrowMushroom), ex);
            }
        }

        public void TryGrowMushroom(BodyPart byManager)
        {
            string MushroomType = "Voidspore";
            GameObject unmodified = GameObject.CreateUnmodified("LaurusFungusPufferVoidspore");
            byManager.Equip(unmodified, new int?(0), true, SemiForced: true);
            if (byManager.Equipped == unmodified)
            {
                // Apply infection or mushroom-specific parts, like curses
                unmodified.RequirePart<Cursed>();
                unmodified.RequirePart<RemoveCursedOnUnequip>();

                if (!ParentObject.Equipped.IsPlayer())
                    return;

                IComponent<GameObject>.AddPlayerMessage($"You sprout a {{C|{MushroomType}}}.");
            }
            else
            {
                unmodified.Obliterate();
            }
        }
    }

    [Serializable]
    public class LaurusFungalInfectionBlightcap : LaurusBaseMushroomInfection
    {
        public LaurusFungalInfectionBlightcap()
        {
            InfectionType = "BlightcapInfection";
        }

        protected override string GetManagerID()
        {
            return ParentObject?.ID + "::BlightcapInfection";
        }

        protected override int GetNewShroomCounter()
        {
            return RandomUtils.NextInt(100, 200);
        }

        protected override void TryDoFungalInfectionLogic()
        {

        }
    }

    [Serializable]
    public class LaurusFungalInfectionBurnspore : LaurusBaseMushroomInfection
    {
        public LaurusFungalInfectionBurnspore()
        {
            InfectionType = "BurnsporeInfection";
        }

        protected override string GetManagerID()
        {
            return ParentObject?.ID + "::BurnsporeInfection";
        }

        protected override int GetNewShroomCounter()
        {
            return RandomUtils.NextInt(600, 800);
        }

        protected override void TryDoFungalInfectionLogic()
        {

        }
    }

    [Serializable]
    public class LaurusFungalInfectionMooncap : LaurusBaseMushroomInfection
    {
        public LaurusFungalInfectionMooncap()
        {
            InfectionType = "MooncapInfection";
        }

        protected override string GetManagerID()
        {
            return ParentObject?.ID + "::MooncapInfection";
        }

        protected override int GetNewShroomCounter()
        {
            return RandomUtils.NextInt(1000, 2000);
        }

        protected override void TryDoFungalInfectionLogic()
        {

        }
    }

    [Serializable]
    public class LaurusFungalInfectionDreamroot : LaurusBaseMushroomInfection
    {
        public LaurusFungalInfectionDreamroot()
        {
            InfectionType = "DreamrootInfection";
        }

        protected override string GetManagerID()
        {
            return ParentObject?.ID + "::DreamrootInfection";
        }

        protected override int GetNewShroomCounter()
        {
            return RandomUtils.NextInt(2000, 4000);
        }

        protected override void TryDoFungalInfectionLogic()
        {

        }
    }
}