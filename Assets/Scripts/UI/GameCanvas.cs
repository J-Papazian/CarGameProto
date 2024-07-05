using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameCanvas : MonoBehaviour
{
    [SerializeField] Image LevelImage = null;
    [SerializeField] TextMeshProUGUI LevelText = null;
    [SerializeField] TextMeshProUGUI WaveText = null;
    [SerializeField] GameObject DashButton = null;
    [SerializeField] Image DashReloadImage = null;

    private bool dashIsUnlock = false;
    private bool reloadDash = true;

    private GameManager gameManager = null;
    private AbilityManager abilityManager = null;

    void Start()
    {
        gameManager = GameManager.Instance;
        gameManager.OnSwitchArena += OnSwitchArena;
        gameManager.OnLevelUP += OnLevelUp;

        abilityManager = AbilityManager.Instance;
        abilityManager.OnDashHasUnlocked += UnlockDash;

        UpdateDatas();
    }

    private void OnDestroy()
    {
        gameManager.OnSwitchArena -= OnSwitchArena;
        abilityManager.OnDashHasUnlocked -= UnlockDash;
        gameManager.OnLevelUP -= OnLevelUp;
    }

    private void Update()
    {
        if (dashIsUnlock && reloadDash)
            ReloadDash();
    }

    private void OnSwitchArena()
    {
        UpdateDatas();
    }

    private void OnLevelUp()
    {
        LevelImage.fillAmount = 0.0f;
        LevelText.text = "Level " + (gameManager.Player.Level + 1);
    }

    private void UpdateDatas()
    {
        LevelText.text = "Level " + (gameManager.Player.Level + 1);
        WaveText.text = "WAVE " + (gameManager.CurrentWave + 1);
        LevelImage.fillAmount = gameManager.Player.XP / gameManager.Player.XPRequire;
    }

    internal void UnlockDash()
    {
        dashIsUnlock = true;
        DashButton.SetActive(true);
    }

    internal void ReloadDash()
    {
        DashReloadImage.fillAmount -= Time.deltaTime / gameManager.Player.dashReloading;

        if (DashReloadImage.fillAmount <= 0.0f)
        {
            reloadDash = false;
            gameManager.Player.DashReady();
        }
    }

    public void Dash()
    {
        if (!gameManager.Player.IsDashReady)
            return;

        DashReloadImage.fillAmount = 1.0f;
        reloadDash = true;
        gameManager.Player.Dash();
    }
}
