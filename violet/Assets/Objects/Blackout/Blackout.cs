using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blackout : MonoBehaviour
{
    private bool fadeOnStart = true;
    public Color fadeColor;
    private Renderer rend;

    private void Awake() { GameManager.OnGameStateChange += HandleGameStateChange; }
    private void OnDestroy() { GameManager.OnGameStateChange -= HandleGameStateChange; }

    void Start()
    {
        rend = GetComponent<Renderer>();

        if (fadeOnStart)
        {
            StartCoroutine(FadeRoutine(1, 0, 1.0f));
        }
    }

    private void HandleGameStateChange(GameState state)
    {
        if (state == GameState.FadeOut)
        {
            StartCoroutine(FadeOutRoutine(1.0f));
        }
        else if (state == GameState.FadeIn)
        {
            StartCoroutine(FadeInRoutine(1.0f));
        }
    }

    private IEnumerator FadeOutRoutine(float duration)
    {
        yield return StartCoroutine(FadeRoutine(0, 1, duration));
        GameManager.Instance.UpdateGameState(GameState.Gameplay);
    }

    private IEnumerator FadeInRoutine(float duration)
    {
        yield return StartCoroutine(FadeRoutine(1, 0, duration));
        GameManager.Instance.UpdateGameState(GameState.LoadEnvironment);
    }

    public IEnumerator FadeRoutine(float alphaIn, float alphaOut, float duration)
    {
        float timer = 0;
        while(timer <= duration)
        {
            Color newColor = fadeColor;
            newColor.a = Mathf.Lerp(alphaIn, alphaOut, timer / duration);
            rend.material.SetColor("_BaseColor", newColor);

            timer += Time.unscaledDeltaTime;
            yield return null;
        }

        // Insurance
        Color newColor2 = fadeColor;
        newColor2.a = alphaOut;
        rend.material.SetColor("_BaseColor", newColor2);
    }
}
