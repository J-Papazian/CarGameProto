using UnityEngine;

public class GameOverCanvas : MonoBehaviour
{
    [SerializeField] RectTransform Background = null;

    private bool gameOver = false;
    float startTime = 0.0f;

    void Start()
    {
        GameManager.Instance.OnGameOver += OnGameOver;
    }

    void Update()
    {
        if (gameOver)
        {
            float t = (Time.unscaledTime - startTime) / DataManager.Instance.Data.GameOverSpeedPanel;
            Background.localPosition = new Vector3(0, Mathf.SmoothStep(Background.localPosition.y, 0, t), 0);
        }    
    }

    private void OnDestroy()
    {
        GameManager.Instance.OnGameOver -= OnGameOver;
    }

    private void OnGameOver()
    {
        Background.gameObject.SetActive(true);
        startTime = Time.unscaledTime;
        gameOver = true;
    }

    public void ReturnToMainMenu()
    {
        Time.timeScale = 1.0f;
        LevelManager.Instance.LoadMenu();
    }
}
