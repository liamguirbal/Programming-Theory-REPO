using UnityEngine;
using TMPro;
using System.Collections;

public class GameTimer : MonoBehaviour
{
    public static GameTimer Instance;

    [Header("UI")]
    public TextMeshProUGUI timerText;
    public RectTransform timerRect; // RectTransform du texte

    private float timeLeft;
    private bool timerRunning = true;
    private int lastSecond = 61;

    void Awake() { Instance = this; }

    void Start()
    {
        timeLeft = 60f;
        UpdateTimerUI();
    }

    void Update()
    {
        if (timerRunning)
        {
            timeLeft -= Time.deltaTime;
            int currentSecond = Mathf.FloorToInt(timeLeft);

            // ANIMATION CHAQUE SECONDE !
            if (currentSecond < lastSecond)
            {
                AnimateSecondTick();
                lastSecond = currentSecond;
            }

            UpdateTimerUI();

            if (timeLeft <= 0)
            {
                timerRunning = false;
                GameOverManager.Instance.ShowGameOver();
            }
        }
    }

    void AnimateSecondTick()
    {
        StartCoroutine(ShakeAndPulse());
    }

    IEnumerator ShakeAndPulse()
    {
        // PULSE (gros → normal)
        timerRect.localScale = Vector3.one * 1.4f;
        yield return new WaitForSeconds(0.1f);
        timerRect.localScale = Vector3.one;

        // SHAKE
        Vector3 originalPos = timerRect.anchoredPosition;
        for (int i = 0; i < 8; i++)
        {
            Vector2 shake = new Vector2( Random.Range(-8f, 8f),Random.Range(-8f, 8f));
            yield return new WaitForSeconds(0.025f);
        }
        timerRect.anchoredPosition = originalPos;
    }

    void UpdateTimerUI()
    {
        int seconds = Mathf.FloorToInt(timeLeft);
        timerText.text = seconds > 10 ? $"Timer : {seconds}" : $"<color=red>Timer : {seconds}</color>";
        timerText.color = seconds > 10 ? Color.white : Color.red;
    }
}
