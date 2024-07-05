using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityManager : Singleton<AbilityManager>
{
    [SerializeField] List<AbilityScriptable> AbilitiesData = new List<AbilityScriptable>();
    [SerializeField] int AbilityNumber = 3;

    private List<AbilityScriptable> abilitiesAvailable = new List<AbilityScriptable>();

    internal System.Action OnAbilityHasSelected;
    internal System.Action OnDashHasUnlocked;

    private GameManager gameManager = null;

    void Start()
    {
        gameManager = GameManager.Instance;
        gameManager.OnGameOver += OnGameOver;

        Init();
    }

    internal void Init()
    {
        for (int i = 0; i < AbilitiesData.Count; ++i)
        {
            switch (AbilitiesData[i].AbilityType)
            {
                case AbilityType.HealthStats:
                    abilitiesAvailable.Add(AbilitiesData[i]);
                    break;
                case AbilityType.DamageStats:
                    abilitiesAvailable.Add(AbilitiesData[i]);
                    break;
                case AbilityType.Turret:
                    abilitiesAvailable.Add(AbilitiesData[i]);
                    break;
                case AbilityType.TurretReloading:
                    break;
                case AbilityType.TurretAttack:
                    break;
                case AbilityType.Dash:
                    abilitiesAvailable.Add(AbilitiesData[i]);
                    break;
                case AbilityType.DashReloading:
                    break;
                case AbilityType.DashAttack:
                    break;
                case AbilityType.Heal:
                    abilitiesAvailable.Add(AbilitiesData[i]);
                    break;
                default:
                    break;
            }
        }
    }

    private void OnDestroy()
    {
        gameManager.OnGameOver += OnGameOver;
    }

    private void OnGameOver()
    {
        abilitiesAvailable.Clear();
    }

    internal List<AbilityScriptable> SelectAbilitiesForLevelUP()
    {
        List<AbilityScriptable> levelUPAbilities = new List<AbilityScriptable>();

        for (int i = 0; i < AbilityNumber; ++i)
        {
            AbilityScriptable ability = abilitiesAvailable[Random.Range(0, abilitiesAvailable.Count)];
            if (levelUPAbilities.Contains(ability))
                --i;
            else
                levelUPAbilities.Add(ability);
        }

        return levelUPAbilities;
    }

    internal void AbilityHasSelected(AbilityType abilityType, float value)
    {
        if (abilityType == AbilityType.Turret)
        {
            for (int i = 0; i < AbilitiesData.Count; ++i)
            {
                if (AbilitiesData[i].AbilityType == AbilityType.TurretAttack ||
                    AbilitiesData[i].AbilityType == AbilityType.TurretReloading)
                    abilitiesAvailable.Add(AbilitiesData[i]);

                else if (AbilitiesData[i].AbilityType == AbilityType.Turret)
                    abilitiesAvailable.Remove(AbilitiesData[i]);
            }
        }

        else if (abilityType == AbilityType.Dash)
        {
            for (int i = 0; i < AbilitiesData.Count; ++i)
            {
                if (AbilitiesData[i].AbilityType == AbilityType.DashAttack ||
                    AbilitiesData[i].AbilityType == AbilityType.DashReloading)
                    abilitiesAvailable.Add(AbilitiesData[i]);

                else if (AbilitiesData[i].AbilityType == AbilityType.Dash)
                    abilitiesAvailable.Remove(AbilitiesData[i]);
            }

            OnDashHasUnlocked?.Invoke();
        }

        gameManager.Player.UnlockNewAbility(abilityType, value);

        OnAbilityHasSelected?.Invoke();
    }
}
