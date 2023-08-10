using SumoArena.GameLobby;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace SumoArena.Menu
{
  public class MainMenuController : MonoBehaviour
  {
    [SerializeField] private GameLobbyManager _gameLobbyManager;
    
    [SerializeField] private Button _hostButton;
    [SerializeField] private Button _joinButton;
    [SerializeField] private Button _findButton;

    private void Awake()
    {
      _hostButton.onClick.AddListener(OnHostClicked);
      _joinButton.onClick.AddListener(OnJoinClicked);
      _findButton.onClick.AddListener(OnFindClicked);
    }

    private async void OnHostClicked()
    {
      bool succeeded = await _gameLobbyManager.CreateLobby();
      if (succeeded)
      {
        SceneManager.LoadSceneAsync("Lobby");
      }
    }
    
    private void OnJoinClicked()
    {
      
    }
    
    private void OnFindClicked()
    {
      
    }
  }
}