using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Player : MonoBehaviour
{
    [SerializeField] MovementJoystick Joystick = null;
    [SerializeField] float Speed = 5.0f;
    [SerializeField] int Damages = 4;
    [SerializeField] int Health = 90;
    [SerializeField] Image HealthImage = null;
    [SerializeField] TextMeshProUGUI HealthText = null;
    [SerializeField] float RotateAngle = 15.0f;
    [SerializeField] float RotateSpeed = 5.0f;
    [SerializeField] GameObject TargetDirection = null;

    [SerializeField] Renderer Renderer = null;
    [SerializeField] Turret Turret = null;
    [SerializeField] internal Pooler BulletPooler = null;

    internal int Level { get; private set; } = 0;
    internal float XP { get; private set; } = 0;
    internal float XPRequire { get; private set; } = 0;

    private int currentHealth = 0;
    private int maxHealth = 0;

    private bool transition = false;
    private bool transitionHasBegun = false;
    private float startTime = 0.0f;
    private bool selectAbility = false;
    private Vector3 positionTransition = Vector3.zero;

    private Color tempColor;
    private bool takeDamage = false;
    private float takeDamageTimer = 0.2f;

    // Dash
    private bool dashUnlock = false;
    internal bool IsDashing { get; private set; } = false;
    internal bool IsDashReady { get; private set; } = false;
    internal float dashReloading = 1.0f;
    private int dashDamage = 0;
    private float dashSpeed = 0;
    private Enemy dashTarget = null;
    //

    private bool gameOver = false;

    private new Rigidbody rigidbody = null;

    private GameManager gameManager = null;
    private DataManager dataManager = null;

    #region Unity Functions

    private void Awake()
    {
        gameManager = GameManager.Instance;
        gameManager.InitPlayer(this);

        dataManager = DataManager.Instance;
        dashDamage = dataManager.Data.DashDamage;
        dashSpeed = dataManager.Data.DashSpeed;

        XPRequire = DataManager.Instance.Data.ExperienceRequireForFirstLevel;
    }

    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        tempColor = Renderer.material.color;
        currentHealth = Health;
        maxHealth = Health;
        dashReloading = dataManager.Data.DashReloading;
    }

    private void Update()
    {
        if (gameOver)
            return;

        HealthText.text = currentHealth.ToString();

        #region Dash

        if (dashUnlock)
        {
            dashTarget = GetClosestEnemy();
            gameManager.AddMarkerToTarget(dashTarget);
        }

        if (IsDashing)
        {
            rigidbody.velocity = transform.forward * dashSpeed;
            transform.LookAt(dashTarget.transform);
            return;
        }

        #endregion

        #region Movement

        if (Joystick.joystickVector.y != 0)
        {
            rigidbody.velocity = new Vector3(Joystick.joystickVector.x * Speed, 0.0f, Joystick.joystickVector.y * Speed);

            if (Joystick.joystickVector.x > 0.5f)
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0.0f, RotateAngle, 0.0f), RotateSpeed * Time.deltaTime);
            else if (Joystick.joystickVector.x < -0.5f)
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0.0f, -RotateAngle, 0.0f), RotateSpeed * Time.deltaTime);
            else
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.identity, RotateSpeed * Time.deltaTime);
        }
        else
        {
            rigidbody.velocity = Vector3.zero;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.identity, RotateSpeed * Time.deltaTime);
        }

        TargetDirection.transform.localPosition = new Vector3(Joystick.joystickVector.x, 0.1f, Joystick.joystickVector.y) * 3.0f;

        #endregion

        #region TakeDamage

        if (takeDamage)
        {
            takeDamageTimer -= Time.deltaTime;
            if (takeDamageTimer <= 0.0f)
            {
                takeDamageTimer = 0.2f;
                Renderer.material.color = tempColor;
                takeDamage = false;
            }
        }

        #endregion

        #region Transition

        if (transition && !selectAbility)
        {
            if (!transitionHasBegun)
                InitTransition();

            float t = (Time.time - startTime) / DataManager.Instance.Data.ArenaTransitionDuration;
            transform.position = new Vector3(Mathf.SmoothStep(transform.position.x, positionTransition.x, t), 0, Mathf.SmoothStep(transform.position.z, positionTransition.z, t));

            if (Vector3.Distance(transform.position, positionTransition) <= 10.0f)
                EndTransition();
        }

        #endregion
    }

    #endregion

    #region Status

    internal int GetDamages()
    {
        return Damages;
    }

    internal int GetTurretDamages()
    {
        return Turret.Damages;
    }

    internal int GetDashDamages()
    {
        return dashDamage;
    }

    internal void TakeDamages(int damages)
    {
        if (IsDashing)
            return;

        takeDamage = true;
        Renderer.material.color = Color.red;

        currentHealth -= damages;
        HealthImage.fillAmount = (currentHealth * 100.0f / maxHealth) / 100.0f;
        if (currentHealth <= 0)
            Death();
    }

    internal void GainXP(float value)
    {
        XP += value;
        if (XP >= XPRequire)
            LevelUP();
    }

    private void LevelUP()
    {
        Level += 1;
        XPRequire = DataManager.Instance.Data.ExperienceRequireForFirstLevel * (Level + 1);
        XP = 0.0f;
        gameManager.PlayerLevelUP();
    }

    private void Death()
    {
        gameOver = true;
        Turret.Over();
        gameManager.GameOver();
    }

    internal void Dash()
    {
        IsDashReady = false;
        IsDashing = true;
        Enemy target = GetClosestEnemy();
        transform.LookAt(target.transform);
    }

    internal void DashOver()
    {
        IsDashing = false;
        transform.rotation = Quaternion.identity;
        rigidbody.velocity = Vector3.zero;
    }

    internal void DashReady()
    {
        IsDashReady = true;
    }

    #endregion

    #region Transition

    internal void MakeTransition(Vector3 newPositionTransition)
    {
        transition = true;
        positionTransition = newPositionTransition;
    }

    private void InitTransition()
    {
        transitionHasBegun = true;
        rigidbody.velocity = Vector3.zero;
        startTime = Time.time;
    }

    private void EndTransition()
    {
        transition = false;
        transitionHasBegun = false;
        gameManager.TransitionIsOver();
    }

    #endregion

    #region Unlock Abilities

    internal void UnlockNewAbility(AbilityType type, float value)
    {
        switch (type)
        {
            case AbilityType.HealthStats:
                maxHealth += (int)value;
                currentHealth += (int)value;
                break;
            case AbilityType.DamageStats:
                Damages += (int)value;
                break;
            case AbilityType.Turret:
                Turret.gameObject.SetActive(true);
                Turret.Init(this);
                break;
            case AbilityType.TurretReloading:
                Turret.UpdateReloading(value);
                break;
            case AbilityType.TurretAttack:
                Turret.UpdateAttack((int)value);
                break;
            case AbilityType.Dash:
                dashUnlock = true;
                break;
            case AbilityType.DashReloading:
                dashReloading -= value;
                break;
            case AbilityType.DashAttack:
                dashDamage += (int)value;
                break;
            case AbilityType.Heal:
                currentHealth += (int)value;
                if (currentHealth > maxHealth)
                    currentHealth = maxHealth;
                break;
            default:
                break;
        }
    }

    #endregion

    internal Enemy GetClosestEnemy()
    {
        List<Enemy> enemies = gameManager.CurrentArena.Enemies;
        float currentdistance = Mathf.Infinity;
        Enemy closestEnemy = null;
        for (int i = 0; i < enemies.Count; ++i)
        {
            if ((transform.position - enemies[i].transform.position).magnitude < currentdistance)
            {
                currentdistance = (transform.position - enemies[i].transform.position).magnitude;
                closestEnemy = enemies[i];
            }
        }

        if (gameManager.CurrentArena.Enemies.Contains(closestEnemy))
            return closestEnemy;

        return null;
    }

    internal void SelectAbility(bool value)
    {
        selectAbility = value;
    }
}
