using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Ability : MonoBehaviour
{
    [SerializeField] Image AbilityImage = null;
    [SerializeField] TextMeshProUGUI AbilityName = null;

    private AbilityType currentAbilityType;
    private float currentValue;

    internal void Init(Sprite sprite, string name, AbilityType abilityType, float value)
    {
        AbilityImage.sprite = sprite;
        AbilityName.text = name;
        currentAbilityType = abilityType;
        currentValue = value;
    }

    public void SelectAbility()
    {
        AbilityManager.Instance.AbilityHasSelected(currentAbilityType, currentValue);
    }
}
