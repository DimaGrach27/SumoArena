using System;
using UnityEngine;

namespace SumoArena.Network.Movement
{
  [Serializable]
  public class InputState
  {
    public int Tick;
    public Vector2 MovementInput;
  }
}