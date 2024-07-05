using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] GameObject Marker = null;

    private GameObject marker = null;
    private MovementJoystick joystickCanvas = null;

    internal Player Player { get; private set; }
    internal int CurrentWave { get; private set; } = 0;
    internal Arena CurrentArena { get; private set; } = null;
    internal bool Transition { get; private set; } = false;

    internal System.Action OnSwitchArena;
    internal System.Action OnGameOver;
    internal System.Action OnLevelUP;

    private void Start()
    {
        LevelManager.Instance.LoadNextLevel();
    }

    internal void InitPlayer(Player newPlayer)
    {
        Player = newPlayer;
        AbilityManager.Instance.Init();
    }

    internal void InitJoystick(MovementJoystick joystick)
    {
        joystickCanvas = joystick;
    }

    internal void EnableOrDisableJoystick(bool enable)
    {
        joystickCanvas.gameObject.SetActive(enable);
    }

    internal void Vibrate()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        Handheld.Vibrate();
#endif
    }

    internal void SwitchArena()
    {
        ++CurrentWave;
        Transition = true;
        joystickCanvas.gameObject.SetActive(false);
        StartCoroutine(WaitBeforeTransition());
    }

    private IEnumerator WaitBeforeTransition()
    {
        yield return new WaitForSeconds(DataManager.Instance.Data.TimeBeforeTransition);

        Player.MakeTransition(Vector3.forward * DataManager.Instance.Data.ArenaWidth * CurrentWave);
    }

    internal void TransitionIsOver()
    {
        Transition = false;
        joystickCanvas.gameObject.SetActive(true);
        OnSwitchArena?.Invoke();
    }

    internal void GameOver()
    {
        Time.timeScale = 0.0f;
        CurrentWave = 0;
        CurrentArena = null;
        OnGameOver?.Invoke();
    }

    internal void PlayerLevelUP()
    {
        OnLevelUP?.Invoke();
    }

    internal void AddMarkerToTarget(Enemy target)
    {
        if (marker == null)
            marker = Instantiate(Marker, Vector3.zero, Quaternion.identity);

        if (target == null)
            return;

        marker.transform.SetParent(target.transform);
        marker.transform.localPosition = Vector3.zero;
        marker.transform.localPosition += Vector3.up * 0.25f;
        marker.transform.localScale = Vector3.one * 0.5f;
        marker.transform.localRotation = Quaternion.Euler(90.0f, 0.0f, 0.0f);
    }

    internal void DealDamagesToPlayer(int damages)
    {
        Vibrate();
        Player.TakeDamages(damages);
    }

    internal void SetCurrentArena(Arena arena)
    {
        CurrentArena = arena;
    }

    internal void DestroyObject(GameObject objectToDestroy)
    {
        Destroy(objectToDestroy);
    }
}
