using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace SumoArena.GameLobby
{
  public class GameLobbyManager : MonoBehaviour
  {
    [SerializeField] private LobbyManager _lobbyManager;
    
    public async Task<bool> CreateLobby()
    {
      if (_lobbyManager != null)
      {
        await _lobbyManager.DeleteLobby();
      }

      Dictionary<string, string> playerData = new Dictionary<string, string>()
      {
        {"GamerTag", "HostPlayer"}
      };
      
      bool succeeded = await _lobbyManager.CreateLobby(4, true, playerData);

      return succeeded;
    }

    private void OnApplicationQuit()
    {
      _lobbyManager?.DeleteLobby();
    }
  }
}