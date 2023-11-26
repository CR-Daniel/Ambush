using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
            StartCoroutine(FadeRoutine(1, 0, 1.0f, false));
        }
        else if (state == GameState.End)
        {
            Time.timeScale = 0.25f;
            StartCoroutine(FadeRoutine(0, 1, 3.0f, true));
        }
    }

    public IEnumerator FadeRoutine(float alphaIn, float alphaOut, float duration, bool reloadLevel)
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

        // Reload level if required
        if (reloadLevel)
        {
            Time.timeScale = 1.0f;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}
