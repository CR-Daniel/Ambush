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
            LoadRandomSkybox();
            GameManager.Instance.UpdateGameState(GameState.Start);
        }
    }

    private void LoadRandomSkybox()
    {
        if (skyboxMaterials.Length > 0)
        {
            int randomIndex = Random.Range(0, skyboxMaterials.Length);
            RenderSettings.skybox = skyboxMaterials[randomIndex];
            DynamicGI.UpdateEnvironment();
        }
        else
        {
            Debug.LogError("No skybox materials have been assigned.");
        }
    }
}
