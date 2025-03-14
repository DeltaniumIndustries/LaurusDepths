using System;
using System.Text;
using XRL.Core;
using XRL.Rules;
using XRL.World;
using XRL.World.Parts;

namespace XRL.Liquids
{
    [IsLiquid]
    [Serializable]
    public abstract class LaurusFluidBase : BaseLiquid
    {

        public LaurusFluidBase(string FluidID)
          : base(FluidID)
        {
            FlameTemperature = GetFluidFlameTemp();
            VaporTemperature = GetFluidVapourTemp();
            Combustibility = GetFluidCombuestibility();
            ThermalConductivity = GetFluidThermalConductivity();
            Evaporativity = GetFluidEvaporativity();
            if (GetFluidVapourObject() != null)
            {
                VaporObject = GetFluidVapourObject();
            }
            InterruptAutowalk = DoesFluidInterruptAutoWalk();
            ConsiderDangerousToContact = IsFluidDangerousToContact();
            ConsiderDangerousToDrink = IsFluidDangerousToDrink();
        }
        public override string GetName(LiquidVolume Liquid) => "{{" + GetShaderString() + "|" + GetFluidName() + "}}";
        public override string GetAdjective(LiquidVolume Liquid) => "{{" + GetShaderString() + "|" + GetFluidAdjective() + "}}";
        public override string GetSmearedAdjective(LiquidVolume Liquid) => "{{" + GetShaderString() + "|" + GetFluidSmearedName() + "}}";
        public override string GetSmearedName(LiquidVolume Liquid) => "{{" + GetShaderString() + "|" + GetFluidSmearedName() + "}}";
        public override string GetStainedName(LiquidVolume Liquid) => "{{" + GetShaderString() + "|" + GetFluidName() + "}}";
        public override string GetWaterRitualName() => GetFluidID();
        public override string GetPreparedCookingIngredient() => GetFluidPreparedCookingIngredient();
        public abstract override bool SafeContainer(GameObject GO);
        public abstract override bool Drank(
          LiquidVolume Liquid,
          int Volume,
          GameObject Target,
          StringBuilder Message,
          ref bool ExitInterface);

        public override void RenderBackgroundPrimary(LiquidVolume Liquid, RenderEvent eRender)
        {
            if (!eRender.ColorsVisible)
                return;
            eRender.ColorString = "^" + GetTileColour() + eRender.ColorString;
        }

        public override void BaseRenderPrimary(LiquidVolume Liquid)
        {
            Liquid.ParentObject.Render.ColorString = "&" + GetTileColour() + "^" + GetColourStringPrimary();
            Liquid.ParentObject.Render.TileColor = "&" + GetTileColour();
            Liquid.ParentObject.Render.DetailColor = GetDetailColour();
        }

        public override void BaseRenderSecondary(LiquidVolume Liquid)
        {
            Liquid.ParentObject.Render.ColorString += "&" + GetColourStringSecondary();
        }

        public override void RenderPrimary(LiquidVolume Liquid, RenderEvent eRender)
        {
            if (!Liquid.IsWadingDepth())
                return;
            if (Liquid.ParentObject.IsFrozen())
            {
                eRender.RenderString = "~";
                eRender.TileVariantColors("&" + GetTileColour() + "^" + GetDetailColour(), "&" + GetTileColour(), "" + GetDetailColour());
            }
            else
            {
                Render render = Liquid.ParentObject.Render;
                int num = (XRLCore.CurrentFrame + Liquid.FrameOffset) % 60;
                if (Stat.RandomCosmetic(1, 600) == 1)
                {
                    eRender.RenderString = "\u000F";
                    eRender.TileVariantColors("&" + GetTileColour() + "^" + GetDetailColour(), "&" + GetTileColour(), GetDetailColour());
                }
                if (Stat.RandomCosmetic(1, 60) != 1)
                    return;
                render.RenderString = num >= 15 ? (num >= 30 ? (num >= 45 ? "~" : "\t") : "~") : "÷";
                render.ColorString = "&" + GetTileColour() + "^" + GetDetailColour();
                render.TileColor = "&" + GetDetailColour();
                render.DetailColor = GetDetailColour();
            }
        }

        public override void RenderSecondary(LiquidVolume Liquid, RenderEvent eRender)
        {
            if (!eRender.ColorsVisible)
                return;
            eRender.ColorString += "&" + GetDetailColour();
        }

        public override void RenderSmearPrimary(
          LiquidVolume Liquid,
          RenderEvent eRender,
          GameObject obj)
        {
            if (eRender.ColorsVisible)
            {
                int num = XRLCore.CurrentFrame % 60;
                if (num > 5 && num < 15)
                    eRender.ColorString = "&" + GetDetailColour();
            }
            base.RenderSmearPrimary(Liquid, eRender, obj);
        }

        public abstract override float GetValuePerDram();

        public abstract string GetFluidID();

        public abstract string GetColourStringPrimary();

        public abstract string GetColourStringSecondary();

        public abstract string GetDetailColour();
        public abstract string GetTileColour();

        protected abstract int GetFluidEvaporativity();
        protected abstract int GetFluidThermalConductivity();
        protected abstract int GetFluidCombuestibility();
        protected abstract int GetFluidVapourTemp();
        protected abstract int GetFluidFlameTemp();
        public override string GetColor() => GetTileColour();
        protected abstract string GetFluidName();
        protected abstract string GetShaderString();
        protected abstract string GetFluidAdjective();
        protected abstract string GetFluidSmearedName();
        protected abstract string GetFluidPreparedCookingIngredient();
        protected abstract string GetFluidVapourObject();
        protected abstract bool IsFluidDangerousToDrink();
        protected abstract bool IsFluidDangerousToContact();
        protected abstract bool DoesFluidInterruptAutoWalk();
    }
}
