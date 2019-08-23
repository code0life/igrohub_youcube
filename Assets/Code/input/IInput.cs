using UnityEngine;

namespace Code.input
{
  public interface IInput
  {
    Vector3 axis { get; }
    float step_size { get; set; }
  }
}