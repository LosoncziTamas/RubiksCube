using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

namespace RubiksCube
{
    public class Rotator : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        private const int MaxColliderCount = 9;
        private static readonly Vector3 HorizontalOverlapBoxExtents = new(4, 0.25f, 4);
        private static readonly Vector3 VerticalOverlapBoxExtents = new(0.25f, 4, 4);

        [Header("Debug Utilities")]
        [SerializeField] private bool _drawGizmos = false;
        [SerializeField] private float _pivotSphereRadius = 0.1f;
        
        [Header("Parameters")]
        [SerializeField] private LayerMask _layer;
        
        private readonly Collider[] _colliders = new Collider[MaxColliderCount];

        private Vector3? _horizontalPivot;
        private Vector3? _verticalPivot;
        private Rotation _rotation;

        public class Rotation
        {
            public List<Transform> ObjectsToRotate;
            public Vector3 Pivot;
            public Vector3 Axis;
            public Orientation Orientation;
        }

        private Vector3 HorizontalPivot
        {
            get
            {
                _horizontalPivot ??= new Vector3(0, transform.position.y, 0);
                return _horizontalPivot.Value;
            }
        }
        
        private Vector3 VerticalPivot
        {
            get
            {
                _verticalPivot ??= new Vector3(transform.position.x, 0, 0);
                return _verticalPivot.Value;
            }
        }

        private List<Collider> DetermineHorizontalPieces()
        {
            Physics.OverlapBoxNonAlloc(transform.position, HorizontalOverlapBoxExtents, _colliders, Quaternion.identity, _layer.value);
            var result = _colliders.ToList();
            return result;
        }

        private List<Collider> DetermineVerticalPieces()
        {
            Physics.OverlapBoxNonAlloc(transform.position, VerticalOverlapBoxExtents, _colliders, Quaternion.identity, _layer.value);
            var result = _colliders.ToList();
            return result;
        }

        private void OnDrawGizmos()
        {
            if (!_drawGizmos)
            {
                return;
            }
            Gizmos.DrawWireCube(transform.position, HorizontalOverlapBoxExtents);
            Gizmos.DrawWireCube(transform.position, VerticalOverlapBoxExtents);
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(HorizontalPivot, _pivotSphereRadius);
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(VerticalPivot, _pivotSphereRadius);
        }
        
        public void OnBeginDrag(PointerEventData eventData)
        {
            DebugGui.Instance.Print("OnBeginDrag0", eventData.delta);
            
            List<Collider> pieces = null;
            var pivot = default(Vector3?);
            var axis = default(Vector3);
            var orientation = default(Orientation?);
            if (HorizontalDrag(eventData.delta))
            {
                pieces = DetermineHorizontalPieces();
                pivot = HorizontalPivot;
                axis = Vector3.up;
                orientation = Orientation.Horizontal;
            }
            else if (VerticalDrag(eventData.delta))
            {
                pieces = DetermineVerticalPieces();
                pivot = VerticalPivot;
                axis = Vector3.right;
                orientation = Orientation.Vertical;
            }
            
            if (pieces != null)
            {
                _rotation = new Rotation
                {
                    Pivot = pivot.Value,
                    ObjectsToRotate = pieces.Select(c => c.transform).ToList(),
                    Axis = axis,
                    Orientation = orientation.Value
                };
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            DebugGui.Instance.Print("OnDrag", eventData.delta);
            if (_rotation == null)
            {
                return;
            }

            var delta = eventData.delta;
            if (Mathf.Approximately(delta.magnitude, 0))
            {
                return;
            }

            var angle = 1.0f;
            if (_rotation.Orientation == Orientation.Horizontal)
            {
                angle = delta.x < 0 ? angle : -angle;
            }
            else if (_rotation.Orientation == Orientation.Vertical)
            {
                angle = delta.y > 0 ? angle : -angle;
            }
            
            foreach (var trans in _rotation.ObjectsToRotate)
            {
                trans.transform.RotateAround(_rotation.Pivot, _rotation.Axis, angle);
            }
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            DebugGui.Instance.Print("OnEndDrag", eventData.delta);
            _rotation = null;
        }

        private static bool VerticalDrag(Vector2 delta)
        {
            return delta.magnitude > 0 && Mathf.Abs(delta.x) < Mathf.Abs(delta.y);
        }

        private static bool HorizontalDrag(Vector2 delta)
        {
            return delta.magnitude > 0 && Mathf.Abs(delta.x) > Mathf.Abs(delta.y);
        }
    }
}