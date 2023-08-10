using System;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SumoArena.Menu
{
  public class Authentication : MonoBehaviour
  {
    private void Start()
    {
      Init();
    }

    private async void Init()
    {
      await UnityServices.InitializeAsync();
      if (UnityServices.State != ServicesInitializationState.Initialized)
      {
        return;
      }
      
      AuthenticationService.Instance.SignedIn += OnSignedInHandler;
      
      await AuthenticationService.Instance.SignInAnonymouslyAsync();
      
      if (!AuthenticationService.Instance.IsSignedIn)
      {
        return;
      }

      string username = PlayerPrefs.GetString("Username");
      if (username == "")
      {
        username = "PLayer";
        PlayerPrefs.SetString("Username", username);
      }

      SceneManager.LoadSceneAsync("MainMenu");
    }

    private void OnSignedInHandler()
    {
      Debug.Log($"Player Name: {AuthenticationService.Instance.PlayerName}");
      Debug.Log($"Player Id: {AuthenticationService.Instance.PlayerId}");
      Debug.Log($"Token: {AuthenticationService.Instance.AccessToken}");
    }
  }
}