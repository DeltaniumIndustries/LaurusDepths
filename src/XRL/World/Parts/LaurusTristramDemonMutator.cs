using System;
using XRL.Names;
using XRL.World.Effects;

namespace XRL.World.Parts
{
    [Serializable]
    public class LaurusTristramDemonMutator : IPart
    {

        private static readonly bool DEBUG = true;
        public string ResultTable = "TristramDemonMutatingResults";
        public int MutationCount { get; set; } = -1;
        public int MutationChanceOneInX { get; set; } = 250;

        public override bool WantEvent(int ID, int cascade)
        {
            return base.WantEvent(ID, cascade) || ID == AfterObjectCreatedEvent.ID;
        }

        public override bool HandleEvent(AfterObjectCreatedEvent E)
        {
            GameObject demonHost = ParentObject;
            if (DEBUG || RandomUtils.NextInt(1, MutationChanceOneInX) == MutationChanceOneInX)
            {
                L.Info($"[LaurusTristramDemonMutator] {demonHost.DisplayName} has been spawned.");
                int mutationsApplied = ApplyMutations(demonHost);
                if (mutationsApplied > 0)
                {
                    ApplyAppearanceChanges(demonHost);
                }
                else
                {
                    L.Info($"[LaurusTristramDemonMutator] {demonHost.DisplayName} did not mutate.");
                }
            }
            if (ParentObject.RemovePart("LaurusTristramDemonMutator"))
            {
                L.Info($"[LaurusTristramDemonMutator] Removed Part from {demonHost.DisplayName}.");
            }


            return base.HandleEvent(E);
        }

        private int ApplyMutations(GameObject demonHost)
        {
            int mutationsToAdd;
            if (MutationCount >= 1)
            {
                mutationsToAdd = MutationCount;
            }
            else
            {
                mutationsToAdd = RandomUtils.NextIntWeighted(RandomUtils.NextIntWeighted(1, 3, 1), 8, RandomUtils.NextIntWeighted(3, 8, 4));
            }
            L.Info($"[LaurusTristramDemonMutator] {demonHost.DisplayName} attempting {mutationsToAdd} mutations.");
            if (demonHost.ApplyEffect(new TouchedByChaos(mutationsToAdd, ResultTable)))
            {
                L.Info($"[LaurusTristramDemonMutator] Mutation {mutationsToAdd}/{mutationsToAdd} applied to {demonHost.DisplayName}.");
            }

            return mutationsToAdd;
        }

        private void ApplyAppearanceChanges(GameObject demonHost)
        {
            string randomForeground = ColourUtils.GetRandomColour().Get();
            string randomDetail = ColourUtils.GetRandomColour().Get();

            string oldName = demonHost.DisplayNameOnly;
            demonHost.GiveProperName(Name: $"{NameUtils.MakeNameMinionsDarkness(demonHost)} ({oldName})", HasHonorific: true);
            demonHost.SetForegroundColor(randomForeground);
            demonHost.SetDetailColor(randomDetail);

            L.Info($"[LaurusTristramDemonMutator] {oldName} mutated into {demonHost.DisplayName} - Foreground: {randomForeground}, Detail: {randomDetail}.");
        }
    }
}
