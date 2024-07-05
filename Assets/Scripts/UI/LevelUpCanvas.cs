using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelUpCanvas : MonoBehaviour
{
    [SerializeField] List<Ability> Abilities = new List<Ability>();
    [SerializeField] GameObject Panel = null;

    private bool levelUP = false;
    private bool panelActive = false;

    private float timer = 0.0f;

    private AbilityManager abilityManager = null;
    private GameManager gameManager = null;

    void Start()
    {
        abilityManager = AbilityManager.Instance;
        abilityManager.OnAbilityHasSelected += OnAbilityHasSelected;

        gameManager = GameManager.Instance;
        gameManager.OnLevelUP += OnLevelUP;
    }

    private void Update()
    {
        if (levelUP)
        {
            if (!panelActive)
                LevelUP();

            timer += Time.unscaledDeltaTime;
        }
    }

    private void OnDestroy()
    {
        abilityManager.OnAbilityHasSelected -= OnAbilityHasSelected;
        gameManager.OnLevelUP -= OnLevelUP;
    }

    private void OnLevelUP()
    {
        levelUP = true;
    }

    private void LevelUP()
    {
        panelActive = true;

        gameManager.EnableOrDisableJoystick(false);
        gameManager.Player.SelectAbility(true);

        List<AbilityScriptable> levelUPAbilities = abilityManager.SelectAbilitiesForLevelUP();

        for (int i = 0; i < Abilities.Count; ++i)
            Abilities[i].Init(levelUPAbilities[i].Sprite, levelUPAbilities[i].AbilityTitle, levelUPAbilities[i].AbilityType, levelUPAbilities[i].Value);

        Panel.SetActive(true);

        Time.timeScale = 0.0f;
    }

    private void OnAbilityHasSelected()
    {
        panelActive = false;
        levelUP = false;
        Time.timeScale = 1.0f;
        gameManager.EnableOrDisableJoystick(true);
        Panel.SetActive(false);

        gameManager.Player.SelectAbility(false);
        timer = 0.0f;
    }
}
