using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public interface IFader
{
    IEnumerator FadeIn();
    IEnumerator FadeOut();
}

public class Fader : MonoBehaviour, IFader
{
    [SerializeField]
    private bool m_startsVisible = false;
    [SerializeField]
    private bool m_fadeOnAwake = false;
    [SerializeField]
    private float m_fadeSpeed = 1.0f;
    [SerializeField]
    private float m_minAlpha = 0;
    [SerializeField]
    private float m_maxAlpha = 1.0f;

    private Image fadeOverlay = null;

    private void Start()
    {
        fadeOverlay = GetComponentInChildren<Image>();
        if (fadeOverlay == null)
        {
            Debug.LogError("Fader: No Image found!");
            return;
        }

        if (m_startsVisible)
        {
            Color spriteColor = fadeOverlay.color;
            spriteColor.a = m_maxAlpha;
            fadeOverlay.color = spriteColor;
            if (m_fadeOnAwake)
                StartCoroutine(FadeOut());
        }
        else
        {
            Color spriteColor = fadeOverlay.color;
            spriteColor.a = m_minAlpha;
            fadeOverlay.color = spriteColor;
            if (m_fadeOnAwake)
                StartCoroutine(FadeIn());
        }
    }

    public IEnumerator FadeIn()
    {
        Color spriteColor = fadeOverlay.color;

        while (spriteColor.a < m_maxAlpha)
        {
            yield return null;
            spriteColor.a += m_fadeSpeed * Time.deltaTime;
            fadeOverlay.color = spriteColor;
        }

        spriteColor.a = m_maxAlpha;
        fadeOverlay.color = spriteColor;
    }

    public IEnumerator FadeOut()
    {
        Color spriteColor = fadeOverlay.color;

        while (spriteColor.a > m_minAlpha)
        {
            yield return null;
            spriteColor.a -= m_fadeSpeed * Time.deltaTime;
            fadeOverlay.color = spriteColor;
        }
        spriteColor.a = m_minAlpha;
        fadeOverlay.color = spriteColor;
    }
}