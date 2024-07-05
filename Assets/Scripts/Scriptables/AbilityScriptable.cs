using UnityEngine;
using UnityEngine.UI;

public enum AbilityType
{
    HealthStats = 0,
    DamageStats,
    Turret,
    TurretReloading,
    TurretAttack,
    Dash,
    DashReloading,
    DashAttack,
    Heal,
}

[CreateAssetMenu(fileName = "Ability", menuName = "Scriptable/Ability", order = 0)]
public class AbilityScriptable : ScriptableObject
{
    public Sprite Sprite = null;
    public AbilityType AbilityType;
    public string AbilityTitle;
    public float Value;
}
