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
    public int BonusPointBin = 10;
    public int MalusPointBin = 5;

    [Header("Reference UI (World Space Canva for VR)")]
    public TextMeshProUGUI bonusText;
    public TextMeshProUGUI malusText;

    [Header("Display time")]
    public float displayDuration = 2f;
    public float distanceAnimation = 0.2f;
    public float fadeSpeed = 2f;

    [Header("Global Score")]
    public ScoreManager scoreManager;

    [Header("Particles")]
    public ParticleSystem bonusParticlesPrefab;
    public ParticleSystem malusParticlesPrefab;
    public Vector3 particleSpawnOffset = Vector3.zero;
    // fallback delay if particle lifetime calculation is not reliable
    public float particleAutoDestroyDelay = 5f;

    Coroutine bonusCoroutine;
    Coroutine malusCoroutine;
    void Awake()
    {
        // Initialization desactivated texts
        if (bonusText != null) bonusText.gameObject.SetActive(false);
        if (malusText != null) malusText.gameObject.SetActive(false);

        // Find all DestructionZones in the scene and subscribe to their events
        var zones = FindObjectsByType<DestructionZone>(FindObjectsSortMode.None);
        foreach (var zone in zones)
        {
            zone.OnBonusObjectDestroyed.AddListener(OnBonusObjectDestroyed);
            zone.OnMalusObjectDestroyed.AddListener(OnMalusObjectDestroyed);
        }

        // Find all DestructionZoneGood in the scene and subscribe to their events
        var zonesGood = FindObjectsByType<DestructionZoneGood>(FindObjectsSortMode.None);
        foreach (var zoneGood in zonesGood)
        {
            zoneGood.OnBonusObjectBinDestroyed.AddListener(OnBonusObjectBinDestroyed);
            zoneGood.OnMalusObjectBinDestroyed.AddListener(OnMalusObjectBinDestroyed);
        }

        // Find ScoreManager if not assigned
        if (scoreManager == null)
            scoreManager = FindFirstObjectByType<ScoreManager>();
    }

    public void OnBonusObjectDestroyed(GameObject go)
    {
        ShowPopup(bonusText, $"+{BonusPoint}", Color.green, ref bonusCoroutine);
        SpawnParticles(go, bonusParticlesPrefab, Color.green);
        scoreManager?.AddScore(BonusPoint);
    }

    public void OnMalusObjectDestroyed(GameObject go)
    {
        ShowPopup(malusText, $"-{MalusPoint}", Color.red, ref malusCoroutine);
        SpawnParticles(go, malusParticlesPrefab, Color.red);
        scoreManager?.AddScore(-MalusPoint);
    }

    public void OnBonusObjectBinDestroyed(GameObject go)
    {
        ShowPopup(bonusText, $"+{BonusPointBin}", Color.green, ref bonusCoroutine);
        SpawnParticles(go, bonusParticlesPrefab, Color.green);
        scoreManager?.AddScore(BonusPointBin);
    }

    public void OnMalusObjectBinDestroyed(GameObject go)
    {
        ShowPopup(malusText, $"-{MalusPointBin}", Color.red, ref malusCoroutine);
        SpawnParticles(go, malusParticlesPrefab, Color.red);
        scoreManager?.AddScore(-MalusPointBin);
    }

    void ShowPopup(TextMeshProUGUI target, string message, Color color, ref Coroutine running)
    {
        // Safety check
        if (target == null) return;
        // Set text and color
        target.text = message;
        target.color = color;
        target.gameObject.SetActive(true);

        // Restart animation
        if (running != null) StopCoroutine(running);
        running = StartCoroutine(AnimatePopup(target));
    }


    IEnumerator AnimatePopup(TextMeshProUGUI target)
    {
        // Start position and alpha
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

    void SpawnParticles(GameObject origin, ParticleSystem prefab, Color tint)
    {
        if (prefab == null || origin == null) return;

        // Instantiate at origin + offset
        ParticleSystem instance = Instantiate(prefab, origin.transform.position + particleSpawnOffset, Quaternion.identity);
        try
        {
            var main = instance.main;
            main.startColor = tint;
            instance.Play();

            // Compute a safe destroy time from duration + lifetime; fallback to particleAutoDestroyDelay
            float destroyAfter = particleAutoDestroyDelay;
            try
            {
                float dur = main.duration;
                float lifetimeMax = 0f;
                var lifetimeCurve = main.startLifetime;
                // attempt to read the constantMax (works for MinMaxCurve)
                lifetimeMax = lifetimeCurve.constantMax;
                destroyAfter = dur + lifetimeMax + 0.1f;
            }
            catch { /* ignore and use fallback */ }

            Destroy(instance.gameObject, destroyAfter);
        }
        catch
        {
            Destroy(instance.gameObject, particleAutoDestroyDelay);
        }
    }
}