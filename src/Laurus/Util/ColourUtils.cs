using System;
using System.Text.RegularExpressions;

public enum QudColour
{
    DarkRed = 'r',
    Red = 'R',
    DarkOrange = 'o',
    Orange = 'O',
    Brown = 'w',
    Gold = 'W',
    DarkGreen = 'g',
    Green = 'G',
    DarkBlue = 'b',
    Blue = 'B',
    DarkCyan = 'c',
    Cyan = 'C',
    DarkMagenta = 'm',
    Magenta = 'M',
    DarkGrey = 'K',
    Grey = 'y',
    White = 'Y'
}
public enum QudShaderType
{
    Hologram,
    Ydfreehold,
    Purple,
    Paisley,
    Biomech,
    Azure,
    Keybind,
    Rainbow,
    Important,
    Metamorphic,
    Transkinetic,
    DarkRed,
    Ubernostrum,
    DesertCamouflage,
    Rocket,
    Rubbergum,
    Visage,
    DarkKeybind,
    SnailEncrusted,
    Dreamsmoke,
    Polarized,
    Ironshank,
    Gold,
    DarkFiery,
    Bethsaida,
    Filthy,
    Sun,
    Pearly,
    Cider,
    Engraved,
    CoatedInPlasma,
    Gaslight,
    Entropic,
    Neutronic,
    Lanterned,
    DarkMagenta,
    Scarlet,
    Overloaded,
    DarkBlue,
    Red,
    Patchwork,
    Nanotech,
    Brainbrine,
    Teal,
    Bee,
    Cryogenic,
    Eater,
    C,
    B,
    G,
    DarkOrange,
    Feathered,
    K,
    M,
    O,
    Internals,
    R,
    Prismatic,
    W,
    Lovesickness,
    Y,
    Fiery,
    UrbanCamouflage,
    Crimson,
    Leopard,
    Ehalcodon,
    Glittering,
    Ghostly,
    PalladiumMesh,
    Tarnished,
    Cyan,
    Shimmering,
    Qon,
    Lava,
    Earth,
    Shugruith,
    Peridot,
    Agolgot,
    Nectar,
    Zetachrome,
    Watery,
    Turbow,
    Plasma,
    Love,
    Thermo,
    Telemetric,
    Black,
    Tartan,
    Psychalflesh,
    Yellow,
    Mirrorshades,
    White,
    Sunset,
    Metachrome,
    Sunslag,
    Structural,
    Psymeridian,
    Starry,
    Extradimensional,
    Sphynx,
    Blaze,
    Putrid,
    Amorous,
    Spiked,
    Spaser,
    Ninefold,
    Slimy,
    Skulk,
    Silvery,
    Magenta,
    Mercurial,
    Graffitied,
    Shemesh,
    Soul,
    Shade,
    DarkGray,
    Snakeskin,
    Rusty,
    Playernotes,
    Green,
    Fungicide,
    Scaled,
    Hotkey,
    Gray,
    Jungle,
    Rules,
    Cloning,
    Lacquered,
    Chiral,
    Refractive,
    Bio,
    Icy,
    ArcticCamouflage,
    SpringTurret,
    Glotrot,
    Psionic,
    Plastifer,
    Crystalline,
    Rermadon,
    Resonance,
    Crochide,
    Plaid,
    Syphon,
    Brown,
    Quantum,
    Olive,
    Beylah,
    Freezing,
    Astral,
    Sparkling,
    Nervous,
    Nanoneuro,
    Horned,
    Defoliant,
    Moon,
    Sickly,
    Painted,
    Kesil,
    Kaleidoslug,
    DarkCyan,
    DarkGreen,
    Radiant,
    Leafy,
    Qas,
    Lah,
    Issachari,
    Normalish,
    Illuminated,
    Levant,
    Emote,
    Hypertractor,
    GreatMachine,
    Normal,
    Forest,
    Blue,
    Electrical,
    Hypervelocity,
    Dreamy,
    Crysteel,
    Cloudy,
    PhaseHarmonic,
    Camouflage,
    Implanted,
    Auroral,
    Opalescent,
    Orange,
    Otherpearl
}


// Add methods in a static partial class
public static partial class QudColourMethods
{
    public static string Get(this QudColour colour)
    {
        return ((char)colour).ToString();
    }
}

public static class ColourUtils
{
    public static QudColour GetRandomColour()
    {
        QudColour[] colours = (QudColour[])Enum.GetValues(typeof(QudColour));
        return colours[RandomUtils.NextInt(0, colours.Length - 1)];
    }

    public static QudShaderType GetRandomShaderType(){
        QudShaderType[] colours = (QudShaderType[])Enum.GetValues(typeof(QudShaderType));
        return colours[RandomUtils.NextInt(0, colours.Length - 1)];
    }
    public static string ApplyColor(string text, QudColour foreground, QudColour? background = null)
    {
        return background.HasValue
            ? $"&{foreground.Get()}^{background.Value.Get()}{text}&y"
            : $"&{foreground.Get()}{text}&y";
    }

    public static string ParseMarkup(string markupText)
    {
        return Regex.Replace(markupText, @"{{(.*?)\|(.*?)}}", match =>
        {
            string colorCode = match.Groups[1].Value;
            string text = match.Groups[2].Value;

            if (colorCode.Length == 1 && Enum.IsDefined(typeof(QudColour), (int)colorCode[0]))
            {
                return ApplyColor(text, (QudColour)colorCode[0]);
            }

            return text; // Return unchanged if the color is invalid
        });
    }
}
