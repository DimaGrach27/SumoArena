using Unity.Netcode;
using UnityEngine;

namespace SumoArena.Network.Movement
{
  public class NetworkPlayerMoveComponent : NetworkBehaviour
  {
    private const int BUFFER_SIZE = 1024;
    
    [SerializeField] private CharacterController _characterController;

    [SerializeField] private float _speed;

    [SerializeField] private int _tick = 0;
    
    [SerializeField] private float _tickRate = 1.0f / 60.0f;
    [SerializeField] private float _tickDeltaTime = 0.0f;
    
    private readonly InputState[] _inputStates = new InputState[BUFFER_SIZE];
    private readonly TransformState[] _transformStates = new TransformState[BUFFER_SIZE];

    [SerializeField] private NetworkVariable<TransformState> ServerTransformState = new();

    [SerializeField] private TransformState _previousTransformState;

    private void OnEnable()
    {
      ServerTransformState.OnValueChanged += OnValueChangedHandler;
    }

    private void OnValueChangedHandler(TransformState previousValue, TransformState newValue)
    {
      _previousTransformState = previousValue;
    }

    public void ProcessLocalPlayerMovement(Vector2 movementInput)
    {
      _tickDeltaTime += Time.deltaTime;
      if (_tickDeltaTime <= _tickRate)
      {
        return;
      }

      int bufferIndex = _tick % BUFFER_SIZE;

      if (!IsServer)
      {
        MovePlayerServerRpc(_tick, movementInput);
        MovePlayer(movementInput);
      }
      else
      {
        MovePlayer(movementInput);

        TransformState state = new TransformState()
        {
          Tick = _tick,
          Position = transform.position,
          Rotation = transform.rotation,
          HasStartedMoving = true
        };

        _previousTransformState = ServerTransformState.Value;
        ServerTransformState.Value = state;
      }

      InputState inputState = new InputState()
      {
        Tick = _tick,
        MovementInput = movementInput
      };
      
      TransformState transformState = new TransformState()
      {
        Tick = _tick,
        Position = transform.position,
        Rotation = transform.rotation,
        HasStartedMoving = true
      };
      
      _inputStates[bufferIndex] = inputState;
      _transformStates[bufferIndex] = transformState;

      _tickDeltaTime -= _tickRate;
      _tick++;
    }

    public void ProcessSimulatedPlayerMovement()
    {
      _tickDeltaTime += Time.deltaTime;
      if (_tickDeltaTime <= _tickRate)
      {
        return;
      }

      if (ServerTransformState.Value.HasStartedMoving)
      {
        transform.position = ServerTransformState.Value.Position;
        transform.rotation = ServerTransformState.Value.Rotation;
      }
      
      _tickDeltaTime -= _tickRate;
      _tick++;
    }

    [ServerRpc]
    private void MovePlayerServerRpc(int tick, Vector2 movementInput)
    {
      MovePlayer(movementInput);

      TransformState state = new TransformState()
      {
        Tick = tick,
        Position = transform.position,
        Rotation = transform.rotation,
        HasStartedMoving = true
      };

      _previousTransformState = ServerTransformState.Value;
      ServerTransformState.Value = state;
    }

    private void MovePlayer(Vector2 movementInput)
    {
      Vector3 movement = new Vector3(movementInput.x, 0.0f, movementInput.y);

      if (!_characterController.isGrounded)
      {
        movement.y = -9.61f;
      }

      movement *= _speed * _tickRate;

      _characterController.Move(movement);
    }
  }
}