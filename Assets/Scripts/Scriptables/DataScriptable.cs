using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "Scriptable/Data", order = 0)]
public class DataScriptable : ScriptableObject
{
    public float BumpThemAllPanelTime = 5.0f;
    public float TimeEnemyProtectedToDamage = 0.1f;
    [Header("Abilities")]
    public float DashSpeed = 20.0f;
    public int DashDamage = 40;
    public float DashReloading = 2.0f;

    public float TurretReloading = 5.0f;
    public int TurretDamage = 3;
    public float BulletSpeed = 15.0f;
    public float BulletLifetime = 10.0f;

    [Header("Arena")]
    public int FirstExplosiveEnemyArena = 3;
    public int FirstShootingEnemyArena = 1;
    public int BossWave = 2;

    [Header("Obstacle")]
    public float ObstacleSpeed = 20.0f;
    public int ObstacleDamage = 500;

    [Header("Transition")]
    public float TimeBeforeTransition = 1.0f;
    public float ArenaTransitionDuration = 5.0f;
    public float ExperienceByArena = 5.0f;
    public float ExperienceRequireForFirstLevel = 10.0f;

    [Header("Environment")]
    public float ArenaWidth = 65.0f;
    public float WallSpeed = 40.0f;

    [Header("GameOver")]
    public float GameOverSpeedPanel = 10.0f;

    [Header("Enemy")]
    public float MinTimeBetweeTwoShoot = 5.0f;
    public float MaxTimeBetweeTwoShoot = 7.0f;
    public float MinTimeToShoot = 1.0f;
    public float MaxTimeToShoot = 5.0f;
}
