using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blackout : MonoBehaviour
{
    public Color fadeColor;
    private Renderer rend;

    private void Awake() { rend = GetComponent<Renderer>(); GameManager.OnGameStateChange += HandleGameStateChange; }
    private void OnDestroy() { GameManager.OnGameStateChange -= HandleGameStateChange; }

    private void HandleGameStateChange(GameState state)
    {
        if (state == GameState.Start)
        {
            StartCoroutine(FadeRoutine(1, 0, 1.0f));
        }
        else if (state == GameState.End)
        {
            StartCoroutine(FadeRoutine(0, 1, 1.0f));
        }
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
