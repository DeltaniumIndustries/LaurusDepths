
using System.Collections.Generic;
using XRL.World;

namespace XRL.Names
{
    public static class NameUtils
    {
        public static string MakeQudishName(GameObject entity) => NameMaker.MakeName(For: entity, Culture: "Qudish");
        public static string MakeNameWombThousand(GameObject entity) => NameMaker.MakeName(For: entity, Faction: "LaurusFactionWombThousand");
        public static string MakeNameCorpusCommune(GameObject entity) => NameMaker.MakeName(For: entity, Faction: "LaurusFactionCorpusCommune");
        public static string MakeNameMinionsDarkness(GameObject entity) => NameMaker.MakeName(For: entity, Faction: "LaurusFactionMinionsOfDarkness");
    }
}