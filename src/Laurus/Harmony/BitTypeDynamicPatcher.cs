using static L;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection;
using XRL.World.Tinkering;

namespace LaurusHarmony
{
    //[HarmonyPatch(typeof(BitType))]
    public static class BitTypePatches
    {
        private static MethodInfo AddTypeMethod;
        private static FieldInfo BitTypesField;
        private static FieldInfo BitMapField;

        // Store any dynamically added types here
        private static List<BitType> _dynamicBitTypes = new List<BitType>();

        // This will patch the AddType method to allow dynamic addition of new bit types
        [HarmonyPatch("AddType")]
        [HarmonyPrefix]
        public static bool AddTypePrefix(ref BitType NewType, bool TierPriority = false, bool visible = true)
        {
            Info($"AddTypePrefix: Attempting to add new bit type: {NewType?.Color}");

            if (AddTypeMethod == null)
            {
                AddTypeMethod = typeof(BitType).GetMethod("AddType", BindingFlags.NonPublic | BindingFlags.Static);
                if (AddTypeMethod == null)
                {
                    Info("Error: Unable to find AddType method.");
                    return true;
                }
            }

            // Add the new type dynamically if it doesn't exist yet
            if (NewType != null && !BitMap().ContainsKey(NewType.Color))
            {
                // Use reflection to add the new BitType
                Info($"AddTypePrefix: Adding new BitType for {NewType.Color}");
                AddTypeMethod.Invoke(null, new object[] { NewType, TierPriority, visible });
                _dynamicBitTypes.Add(NewType); // Store the dynamic type for later use
                Info($"AddTypePrefix: Successfully added BitType for {NewType.Color}");
                return false; // Skip the original AddType method
            }

            Info($"AddTypePrefix: BitType for {NewType.Color} already exists.");
            return true; // Continue with the original method if the type exists
        }

        // This will ensure that the BitTypes collection is initialized and has space for new additions
        [HarmonyPatch("Init")]
        [HarmonyPrefix]
        public static bool InitPrefix()
        {
            Info("InitPrefix: Initializing BitTypes with dynamic additions.");
            int initialCapacity = BitTypes().Count + _dynamicBitTypes.Count; // Include the dynamic additions
            BitTypes().Clear();
            BitMap().Clear();

            // Re-add the default and dynamic types
            foreach (var bitType in _dynamicBitTypes)
            {
                Info($"InitPrefix: Re-adding dynamic BitType {bitType.Color}");
                // Call the AddType method via reflection to add the dynamic BitType
                AddTypeMethod.Invoke(null, new object[] { bitType, true, true });
            }

            Info("InitPrefix: Dynamic BitTypes added.");
            return true; // Skip the original Init method and proceed with custom initialization
        }

        // Method to access the _BitTypes private static field using reflection
        private static List<BitType> BitTypes()
        {
            if (BitTypesField == null)
            {
                BitTypesField = typeof(BitType).GetField("_BitTypes", BindingFlags.NonPublic | BindingFlags.Static);
            }

            return BitTypesField.GetValue(null) as List<BitType>;
        }

        // Method to access the _BitMap private static field using reflection
        private static Dictionary<char, BitType> BitMap()
        {
            if (BitMapField == null)
            {
                BitMapField = typeof(BitType).GetField("_BitMap", BindingFlags.NonPublic | BindingFlags.Static);
            }

            return BitMapField.GetValue(null) as Dictionary<char, BitType>;
        }

        // Method to allow dynamic addition of new BitType instances
        public static void AddNewBitType(int level, char color, string description)
        {
            Info($"AddNewBitType: Adding new dynamic BitType with color: {color}");
            BitType newBitType = new BitType(level, color, description);
            AddTypePrefix(ref newBitType, true, true); // Add the type using the dynamic patch
        }

        // Patch the BitTypes property to return dynamically updated list
        [HarmonyPatch("BitTypes")]
        [HarmonyPostfix]
        public static void BitTypesPostfix(ref List<BitType> __result)
        {
            Info("BitTypesPostfix: Returning dynamically updated list.");
            // Ensure that the BitTypes list is updated with dynamically added types
            __result = new List<BitType>(BitTypes()); // Return the dynamically updated list of bit types
        }

        // Initialize the Harmony patches
        public static void ApplyPatches()
        {
            Info("ApplyPatches: Applying Harmony patches.");
            var harmony = new Harmony("net.laurus.dynamicbittypes");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
            Info("ApplyPatches: Harmony patches applied.");
        }

        // Patch GetBitTier to dynamically handle new bit types that may not exist in the original method
        [HarmonyPatch("GetBitTier")]
        [HarmonyPrefix]
        public static bool GetBitTierPrefix(ref int __result, char Bit)
        {
            Info($"GetBitTierPrefix: Handling BitTier for Bit {Bit}");
            if (BitType.BitMap.ContainsKey(Bit))
            {
                __result = BitType.GetBitTier(Bit);
                Info($"GetBitTierPrefix: Found BitTier for Bit {Bit}: {__result}");
                return false; // Skip the original method since we provided the result
            }

            // Default return for new dynamic bits (you can customize the tier level Log.Infoic)
            __result = 0; // Default tier for unrecognized bits
            Info($"GetBitTierPrefix: Default BitTier for Bit {Bit}: {__result}");
            return false; // Skip original method
        }

        // Patch TranslateBit to dynamically handle new bit types
        [HarmonyPatch("TranslateBit")]
        [HarmonyPrefix]
        public static bool TranslateBitPrefix(ref string __result, char Bit)
        {
            Info($"TranslateBitPrefix: Translating Bit {Bit}");
            if (BitType.BitMap.ContainsKey(Bit))
            {
                __result = BitType.TranslateBit(Bit);
                Info($"TranslateBitPrefix: Found translation for Bit {Bit}: {__result}");
                return false; // Skip original method
            }

            // Default translation for unrecognized bits
            __result = "?"; // You can return something else if needed
            Info($"TranslateBitPrefix: Default translation for Bit {Bit}: {__result}");
            return false; // Skip original method
        }

        // Patch ReverseTranslateBit to dynamically handle new bit types
        [HarmonyPatch("ReverseTranslateBit")]
        [HarmonyPrefix]
        public static bool ReverseTranslateBitPrefix(ref string __result, char Bit)
        {
            Info($"ReverseTranslateBitPrefix: Reverse translating Bit {Bit}");
            if (BitType.BitMap.ContainsKey(Bit))
            {
                __result = BitType.ReverseTranslateBit(Bit);
                Info($"ReverseTranslateBitPrefix: Found reverse translation for Bit {Bit}: {__result}");
                return false; // Skip original method
            }

            // Default reverse translation for unrecognized bits
            __result = "?"; // You can return something else if needed
            Info($"ReverseTranslateBitPrefix: Default reverse translation for Bit {Bit}: {__result}");
            return false; // Skip original method
        }
    }
}
