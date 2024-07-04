using SimpleInputNamespace;
using TriInspector;
using UnityEngine;

namespace Game
{
    public class SceneData : MonoBehaviour
    {
        [field: SerializeField, Required] public Joystick Joystick { get; private set; }
    }
}