using System.Collections;
using TMPro;
using UnityEngine;

public class ScorePopup : MonoBehaviour
{
    [SerializeField] private float duration = 0.6f;
    [SerializeField] private float moveDistance = 80f;
    [SerializeField] private float popScale = 1.3f;

    private TextMeshProUGUI popupText;
    private RectTransform rectTransform;

    private void Awake()
    {
        popupText = GetComponent<TextMeshProUGUI>();
        rectTransform = GetComponent<RectTransform>();
    }

    public void Play(int amount)
    {
        popupText.text = amount >= 0
            ? "+" + amount
            : amount.ToString();

        StartCoroutine(PlayAnimation());
    }

    private IEnumerator PlayAnimation()
    {
        Vector2 startPosition = rectTransform.anchoredPosition;
        Vector3 originalScale = rectTransform.localScale;
        Color originalColor = popupText.color;

        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;

            float progress = Mathf.Clamp01(elapsed / duration);

            rectTransform.anchoredPosition =
                startPosition + Vector2.up * moveDistance * progress;

            float punch = Mathf.Sin(progress * Mathf.PI);

            rectTransform.localScale =
                originalScale * Mathf.Lerp(1f, popScale, punch);

            Color color = originalColor;
            color.a = 1f - progress;
            popupText.color = color;

            yield return null;
        }

        Destroy(gameObject);
    }
}