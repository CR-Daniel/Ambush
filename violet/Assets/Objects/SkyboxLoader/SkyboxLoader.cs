using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyboxLoader : MonoBehaviour
{
    public Material[] skyboxMaterials;

    private void Awake() { GameManager.OnGameStateChange += HandleGameStateChange; }
    private void OnDestroy() { GameManager.OnGameStateChange -= HandleGameStateChange; }
    private void HandleGameStateChange(GameState state)
    {
        if (state == GameState.LoadSkybox)
        {
            LoadSkybox();
            GameManager.Instance.UpdateGameState(GameState.Start);
        }
    }

    private void LoadSkybox()
    {
        if (skyboxMaterials.Length > 0)
        {
            int highScore = PlayerPrefs.GetInt("HighScore", 0);
            if      (highScore > 30) { RenderSettings.skybox = skyboxMaterials[3]; }
            else if (highScore > 20) { RenderSettings.skybox = skyboxMaterials[2]; }
            else if (highScore > 10) { RenderSettings.skybox = skyboxMaterials[1]; }
            else                     { RenderSettings.skybox = skyboxMaterials[0]; }
            DynamicGI.UpdateEnvironment();
        }
        else
        {
            Debug.LogError("No skybox materials have been assigned.");
        }
    }
}
