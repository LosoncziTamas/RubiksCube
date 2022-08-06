using UnityEngine;
using UnityEngine.EventSystems;

namespace RubiksCube
{
    public class Rotator : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        private const int MaxColliderCount = 9;
        private static readonly Vector3 HorizontalOverlapBoxExtents = new(4, 0.25f, 4);
        private static readonly Vector3 VerticalOverlapBoxExtents = new(0.25f, 4, 4);

        [SerializeField] private bool _drawGizmos = false;
        [SerializeField] private LayerMask _layer;
        
        private readonly Collider[] _colliders = new Collider[MaxColliderCount];

        private void GetHorizontalPieces()
        {
            Physics.OverlapBoxNonAlloc(transform.position, HorizontalOverlapBoxExtents, _colliders, Quaternion.identity, _layer.value);
        }

        private void GetVerticalPieces()
        {
            Physics.OverlapBoxNonAlloc(transform.position, VerticalOverlapBoxExtents, _colliders, Quaternion.identity, _layer.value);
        }

        private void OnDrawGizmos()
        {
            if (!_drawGizmos)
            {
                return;
            }
            Gizmos.DrawWireCube(transform.position, HorizontalOverlapBoxExtents);
            Gizmos.DrawWireCube(transform.position, VerticalOverlapBoxExtents);
        }
        
        public void OnBeginDrag(PointerEventData eventData)
        {
            DebugGui.Instance.Print("OnBeginDrag0", eventData.delta);
        }

        public void OnDrag(PointerEventData eventData)
        {
            DebugGui.Instance.Print("OnDrag", eventData.delta);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            DebugGui.Instance.Print("OnEndDrag", eventData.delta);
        }
    }
}