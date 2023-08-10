using SumoArena.Network.Movement;
using Unity.Netcode;
using UnityEngine;

namespace SumoArena.GamePlayer
{
  public class PlayerController : NetworkBehaviour
  {
    [SerializeField] private NetworkPlayerMoveComponent _moveComponent;
    
    private MovementActions _movementActions;
    
    public override void OnNetworkSpawn()
    {
      if (!IsOwner)
      {
        return;
      }

      _movementActions = new MovementActions();
      _movementActions.Enable();
    }
    
    private void Update()
    {
      if ((IsClient || IsHost) && IsLocalPlayer)
      {
        Vector2 moveInput = _movementActions.MoveMap.PlayerMovement.ReadValue<Vector2>();
        // print($"Player {OwnerClientId} :Move {moveInput}");
        _moveComponent.ProcessLocalPlayerMovement(moveInput);
      }
      else
      {
        _moveComponent.ProcessSimulatedPlayerMovement();
      }
    }
    
    public override void OnNetworkDespawn()
    {
      if (!IsOwner)
      {
        return;
      }
      
      _movementActions.Disable();
    }
  }
}