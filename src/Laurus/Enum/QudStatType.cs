using System;
using System.Reflection;

public enum QudStatType
{
    [StatAbbreviation("STR")] Strength,
    [StatAbbreviation("AGI")] Agility,
    [StatAbbreviation("TOU")] Toughness,
    [StatAbbreviation("INT")] Intelligence,
    [StatAbbreviation("WIL")] Willpower,
    [StatAbbreviation("EGO")] Ego,
    [StatAbbreviation("HP")] Hitpoints,
    [StatAbbreviation("QN")] Speed,
    [StatAbbreviation("MS")] MoveSpeed,
    [StatAbbreviation("AR")] AcidResistance,
    [StatAbbreviation("CR")] ColdResistance,
    [StatAbbreviation("ER")] ElectricResistance,
    [StatAbbreviation("HR")] HeatResistance,
    [StatAbbreviation("AP")] AttributePoints,
    [StatAbbreviation("MP")] MutationPoints,
    [StatAbbreviation("SP")] SkillPoints,
    [StatAbbreviation("XP")] ExperiencePoints,
    [StatAbbreviation("XPV")] XPValue,
    [StatAbbreviation("LVL")] Level
}

/// <summary>
/// Custom attribute to store stat abbreviations.
/// </summary>
[AttributeUsage(AttributeTargets.Field)]
public class StatAbbreviationAttribute : Attribute
{
    public string Abbreviation { get; }
    public StatAbbreviationAttribute(string abbreviation) => Abbreviation = abbreviation;
}


public static class QudStatTypeExtensions
{
    /// <summary>
    /// Extension method to retrieve the abbreviation.
    /// </summary>
    public static string GetAbbreviation(this QudStatType stat)
    {
        FieldInfo field = stat.GetType().GetField(stat.ToString());
        StatAbbreviationAttribute attribute = field?.GetCustomAttribute<StatAbbreviationAttribute>();
        return attribute?.Abbreviation ?? stat.ToString();
    }

    /// <summary>
    /// Gets a QudStatType from a string, checking both names and abbreviations.
    /// Throws an exception if the input does not match a valid stat.
    /// </summary>
    public static QudStatType GetQudStatType(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            throw new ArgumentException("Input cannot be null or empty.", nameof(input));
        }

        // Try parsing directly as an enum name (case-insensitive)
        if (Enum.TryParse(input, true, out QudStatType statType))
        {
            return statType;
        }

        // Check abbreviations
        foreach (QudStatType stat in Enum.GetValues(typeof(QudStatType)))
        {
            if (string.Equals(stat.GetAbbreviation(), input, StringComparison.OrdinalIgnoreCase))
            {
                return stat;
            }
        }

        throw new ArgumentException($"No matching QudStatType found for '{input}'.");
    }
}

