using System;

namespace ExpandedHotbars.Enums;

public enum ClassJobRole {
    Tank = 1,
    RegenHealer = 2,
    Melee = 3,
    PhysicalRanged = 4,
    Caster = 5,
    ShieldHealer = 6,
}

public static class ClassJobRoleExtensions {
    extension(ClassJobRole role) {
        public string Label => role switch {
            ClassJobRole.Tank => "Tank",
            ClassJobRole.RegenHealer => "Regen Healer",
            ClassJobRole.Melee => "Melee",
            ClassJobRole.PhysicalRanged => "Physical Ranged",
            ClassJobRole.Caster => "Caster",
            ClassJobRole.ShieldHealer => "Shield Healer",
            _ => throw new ArgumentOutOfRangeException(nameof(role), role, null),
        };
    }
}
