using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using SumoArena.Tools;
using Unity.Services.Authentication;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;

namespace SumoArena.GameLobby
{
  public class LobbyManager
  {
    private readonly CoroutineHelper _coroutineHelper;
    private Lobby _lobby;

    private Coroutine _heartbeatCoroutine;
    private Coroutine _refreshCoroutine;
    
    public LobbyManager(CoroutineHelper coroutineHelper)
    {
      _coroutineHelper = coroutineHelper;
    }

    public async Task<bool> CreateLobby(int maxPlayer, bool isPrivate, Dictionary<string, string> data)
    {
      Dictionary<string, PlayerDataObject> playerData = SerializePlayerData(data);
      
      Player player = new Player(AuthenticationService.Instance.PlayerId, null, playerData);
      
      CreateLobbyOptions options = new CreateLobbyOptions()
      {
        IsPrivate = isPrivate,
        Player = player
      };

      try
      {
        _lobby = await LobbyService.Instance.CreateLobbyAsync("Lobby", maxPlayer, options);
      }
      catch (System.Exception)
      {
        return false;
      }

      Debug.Log($"Lobby create with lobby id {_lobby.Id}");
      
      _heartbeatCoroutine = _coroutineHelper.StartCoroutine(HeartbeatLobbyCoroutine(_lobby.Id, 6.66f));
      _refreshCoroutine = _coroutineHelper.StartCoroutine(RefreshLobbyCoroutine(_lobby.Id, 1.0f));
      
      return true;
    }
    
    public async Task DeleteLobby()
    {
      if (_heartbeatCoroutine != null)
      {
        _coroutineHelper.StopCoroutine(_heartbeatCoroutine);
        _heartbeatCoroutine = null;
      }
      
      if (_refreshCoroutine != null)
      {
        _coroutineHelper.StopCoroutine(_refreshCoroutine);
        _refreshCoroutine = null;
      }
      
      if (_lobby != null && _lobby.HostId == AuthenticationService.Instance.PlayerId)
      {
        await LobbyService.Instance.DeleteLobbyAsync(_lobby.Id);
      }
    }

    private Dictionary<string, PlayerDataObject> SerializePlayerData(Dictionary<string, string> data)
    {
      Dictionary<string, PlayerDataObject> playerData = new Dictionary<string, PlayerDataObject>();

      foreach (var (key, value) in data)
      {
        playerData.Add(key, new PlayerDataObject(
          PlayerDataObject.VisibilityOptions.Member,
          value));
      }
      return playerData;
    }

    private IEnumerator HeartbeatLobbyCoroutine(string lobbyId, float waitTimeSecond)
    {
      while (true)
      {
        Debug.Log("Heartbeat");
        LobbyService.Instance.SendHeartbeatPingAsync(lobbyId);
        yield return new WaitForSecondsRealtime(waitTimeSecond);
      }
    }
    
    private IEnumerator RefreshLobbyCoroutine(string lobbyId, float waitTimeSecond)
    {
      while (true)
      {
        Task<Lobby> task = LobbyService.Instance.GetLobbyAsync(lobbyId);
        yield return new WaitUntil(() => task.IsCompleted);

        Lobby newLobby = task.Result;
        if (newLobby.LastUpdated > _lobby.LastUpdated)
        {
          _lobby = newLobby;
        }
        
        yield return new WaitForSecondsRealtime(waitTimeSecond);
      }
    }
  }
}