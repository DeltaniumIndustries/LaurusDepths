using System;
using System.Collections.Generic;
using XRL.World;

public static class StatUtils
{
    /// <summary>
    /// Safely retrieves the statistics dictionary from a GameObject.
    /// </summary>
    private static Dictionary<string, Statistic> GetStatistics(GameObject entity)
    {
        if (entity == null)
        {
            L.Info("[StatUtils] Entity is null. No statistics available.");
            return new Dictionary<string, Statistic>();
        }

        if (entity.Statistics == null)
        {
            L.Info($"[StatUtils] {entity.DisplayName} has no statistics.");
            return new Dictionary<string, Statistic>();
        }

        return entity.Statistics;
    }

    /// <summary>
    /// Improves each stat randomly up to a maximum increase.
    /// </summary>
    public static void RandomImproveStats(GameObject entity, int minIncrease, int maxIncrease)
    {
        var statistics = GetStatistics(entity);
        if (statistics.Count == 0) return; // No valid stats to modify

        var statsToImprove = new HashSet<string>(Statistic.Attributes);
        statsToImprove.UnionWith(Statistic.MentalStats);
        statsToImprove.UnionWith(Statistic.StatDisplayNames.Keys);
        statsToImprove.ExceptWith(Statistic.InverseBenefitStats); // Remove unwanted stats

        foreach (string stat in statsToImprove)
        {
            QudStatType statType = QudStatTypeExtensions.GetQudStatType(stat);
            BoostStat(statistics, statType, minIncrease, RandomUtils.NextInt(minIncrease, maxIncrease));
        }
    }

    /// <summary>
    /// Boosts an individual stat if it exists in the statistics dictionary.
    /// </summary>
    public static void BoostStat(GameObject entity, QudStatType stat, int fixedIncrease)
    {
        var statistics = GetStatistics(entity);
        if (statistics.Count == 0) return; // No valid stats to modify

        BoostStat(statistics, stat, fixedIncrease, fixedIncrease);
    }

    /// <summary>
    /// Boosts an individual stat if it exists in the statistics dictionary.
    /// </summary>
    private static void BoostStat(Dictionary<string, Statistic> statistics, QudStatType stat, int minIncrease, int maxIncrease)
    {
        string statName = stat.ToString();
        if (statistics.TryGetValue(statName, out Statistic statObj))
        {
            int amount = RandomUtils.NextInt(minIncrease, maxIncrease);
            statObj.BaseValue += amount;
            L.Info($"[StatUtils] {statName} increased by {amount}. New value: {statObj.BaseValue}");
        }
        else
        {
            L.Info($"[StatUtils] {statName} does not exist in Statistics.");
        }
    }

    /// <summary>
    /// Boosts a list of stats by a random amount up to maxIncrease.
    /// </summary>
    private static void BoostStats(GameObject entity, IEnumerable<QudStatType> stats, int minIncrease, int maxIncrease)
    {
        var statistics = GetStatistics(entity);
        if (statistics.Count == 0) return;

        foreach (var stat in stats)
        {
            BoostStat(statistics, stat, minIncrease, maxIncrease);
        }
    }

    /// <summary>
    /// Boosts all primary attributes (Strength, Agility, Toughness, Intelligence, Willpower, Ego).
    /// </summary>
    public static void BoostAttributes(GameObject entity, int minIncrease, int maxIncrease) =>
        BoostStats(entity, new[] {
            QudStatType.Strength, QudStatType.Agility, QudStatType.Toughness,
            QudStatType.Intelligence, QudStatType.Willpower, QudStatType.Ego
        }, minIncrease, maxIncrease);

    /// <summary>
    /// Boosts all mental stats (Ego, Intelligence, Willpower).
    /// </summary>
    public static void BoostMentalStats(GameObject entity, int minIncrease, int maxIncrease) =>
        BoostStats(entity, new[] {
            QudStatType.Ego, QudStatType.Intelligence, QudStatType.Willpower
        }, minIncrease, maxIncrease);

    /// <summary>
    /// Boosts all resistance stats (Acid, Cold, Electric, Heat).
    /// </summary>
    public static void BoostResistances(GameObject entity, int minIncrease, int maxIncrease) =>
        BoostStats(entity, new[] {
            QudStatType.AcidResistance, QudStatType.ColdResistance,
            QudStatType.ElectricResistance, QudStatType.HeatResistance
        }, minIncrease, maxIncrease);

    /// <summary>
    /// Boosts all derived stats (Hitpoints, Speed, MoveSpeed).
    /// </summary>
    public static void BoostDerivedStats(GameObject entity, int minIncrease, int maxIncrease) =>
        BoostStats(entity, new[] {
            QudStatType.Hitpoints, QudStatType.Speed, QudStatType.MoveSpeed
        }, minIncrease, maxIncrease);

    /// <summary>
    /// Boosts player-only stats (AP, MP, SP, XP), ensuring the entity is a player.
    /// </summary>
    public static void BoostPlayerStats(GameObject entity, int minIncrease, int maxIncrease)
    {
        if (entity == null || !entity.IsPlayer())
        {
            L.Info($"[StatUtils] {entity?.DisplayName ?? "Unknown Entity"} is not a player. Skipping stat increase.");
            return;
        }

        BoostStats(entity, new[] {
            QudStatType.AttributePoints, QudStatType.MutationPoints,
            QudStatType.SkillPoints, QudStatType.ExperiencePoints
        }, minIncrease, maxIncrease);
    }

    /// <summary>
    /// Boosts XPValue uniquely, independent of player status.
    /// </summary>
    public static void BoostXPValue(GameObject entity, int minIncrease, int maxIncrease)
    {
        var statistics = GetStatistics(entity);
        if (statistics.Count == 0) return;

        BoostStat(statistics, QudStatType.XPValue, minIncrease, maxIncrease);
    }

    /// <summary>
    /// Boosts Hitpoints uniquely, independent of player status.
    /// </summary>
    public static void BoostEntityHealth(GameObject entity, int minIncrease, int maxIncrease)
    {
        var statistics = GetStatistics(entity);
        if (statistics.Count == 0) return;

        BoostStat(statistics, QudStatType.Hitpoints, minIncrease, maxIncrease);
    }
    /// <summary>
    /// Boosts Level uniquely, independent of player status.
    /// </summary>
    public static void BoostEntityLevel(GameObject entity, int minIncrease, int maxIncrease)
    {
        var statistics = GetStatistics(entity);
        if (statistics.Count == 0) return;

        BoostStat(statistics, QudStatType.Level, minIncrease, maxIncrease);
    }
}
