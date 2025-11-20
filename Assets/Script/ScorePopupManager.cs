using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ScorePopupManager : MonoBehaviour
{

    [Header("Scoring")]
    public int BonusPoint=1;
    public int MalusPoint=2;

    [Header("Reference UI (World Space Canva for VR)")]
    public TextMeshProUGUI bonusText;
    public TextMeshProUGUI malusText;

    [Header("Display time")]
    public float displayDuration = 2f;
    public float distanceAnimation = 0.2f;
    public float fadeSpeed = 2f;

    [Header("Global Score")]
    public ScoreManager scoreManager;

    Coroutine bonusCoroutine;
    Coroutine malusCoroutine;
    void Awake()
    {
        // Désactivation initiale
        if (bonusText != null) bonusText.gameObject.SetActive(false);
        if (malusText != null) malusText.gameObject.SetActive(false);

        // Trouve toutes les zones de destruction dans la scène
        var zones = FindObjectsByType<DestructionZone>(FindObjectsSortMode.None);
        foreach (var zone in zones)
        {
            zone.OnBonusObjectDestroyed.AddListener(OnBonusObjectDestroyed);
            zone.OnMalusObjectDestroyed.AddListener(OnMalusObjectDestroyed);
        }
        if (scoreManager == null)
            scoreManager = FindFirstObjectByType<ScoreManager>();
    }

    public void OnBonusObjectDestroyed(GameObject go)
    {
        ShowPopup(bonusText, $"+{BonusPoint}", Color.green, ref bonusCoroutine);
        scoreManager?.AddScore(BonusPoint);
    }

    public void OnMalusObjectDestroyed(GameObject go)
    {
        ShowPopup(malusText, $"-{MalusPoint}", Color.red, ref malusCoroutine);
        scoreManager?.AddScore(-MalusPoint);
    }

    void ShowPopup(TextMeshProUGUI target, string message, Color color, ref Coroutine running)
    {
        if (target == null) return;
        target.text = message;
        target.color = color;
        target.gameObject.SetActive(true);

        if (running != null) StopCoroutine(running);
        running = StartCoroutine(AnimatePopup(target));
    }


    IEnumerator AnimatePopup(TextMeshProUGUI target)
    {
        // Position et alpha initiaux
        Vector3 startPos = target.rectTransform.localPosition;
        Vector3 endPos = startPos + Vector3.up * distanceAnimation;
        Color startColor = target.color;
        float elapsed = 0f;

        while (elapsed < displayDuration)
        {
            elapsed += Time.deltaTime;

            // interpolation position + alpha
            float t = elapsed / displayDuration;
            target.rectTransform.localPosition = Vector3.Lerp(startPos, endPos, t);
            float alpha = Mathf.Lerp(1f, 0f, t * fadeSpeed);
            target.color = new Color(startColor.r, startColor.g, startColor.b, alpha);

            yield return null;
        }

        // Reset
        target.rectTransform.localPosition = startPos;
        target.gameObject.SetActive(false);
        target.color = startColor;
    }
}