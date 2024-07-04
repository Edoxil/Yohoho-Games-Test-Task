using AB_Utility.FromSceneToEntityConverter;
using TriInspector;
using UnityEngine;

namespace Game
{
    public class SizeConverter : ComponentConverter<SizeComponent>
    {
        [SerializeField] private bool _drawGizmos;
        [SerializeField, ShowIf(nameof(_drawGizmos), true)] private Color _gizmoColor = Color.black;

        private void OnDrawGizmos()
        {
            if (_drawGizmos == false)
                return;

            Gizmos.color = _gizmoColor;
            Gizmos.DrawCube(transform.position, _component.value);
        }
    }
}