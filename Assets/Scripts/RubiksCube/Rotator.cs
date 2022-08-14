using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using ProtoPack.Tween;
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
        [SerializeField] private float _rotationSpeed = 1.0f;
        
        [Header("Parameters")]
        [SerializeField] private LayerMask _layer;
        [SerializeField] private RotateTweenProperties _rotateTweenProperties;
        
        private readonly Collider[] _colliders = new Collider[MaxColliderCount];

        private Vector3? _horizontalPivot;
        private Vector3? _verticalPivot;
        private Rotation _rotation;
        private Transform _originalParent;

        public class Rotation
        {
            public List<Transform> ObjectsToRotate;
            public Vector3 Pivot;
            public Vector3 Axis;
            public Orientation Orientation;
            public Transform ParentTransform;
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

        private void Awake()
        {
            _originalParent = transform.parent;
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
            
            if (HorizontalDrag(eventData.delta))
            {
                var objects = DetermineHorizontalPieces().Select(c => c.transform).ToList();
                var parentTrans = new GameObject("Temp parent").transform;
                parentTrans.position = Vector3.zero;
                foreach (var obj in objects)
                {
                    obj.SetParent(parentTrans);
                }
                _rotation = new Rotation
                {
                    Pivot = HorizontalPivot,
                    ObjectsToRotate = objects,
                    Axis = Vector3.up,
                    Orientation = Orientation.Horizontal,
                    ParentTransform = parentTrans
                };
            }
            else if (VerticalDrag(eventData.delta))
            {
                var objects = DetermineVerticalPieces().Select(c => c.transform).ToList();
                var parentTrans = new GameObject("Temp parent").transform;
                parentTrans.position = Vector3.zero;
                foreach (var obj in objects)
                {
                    obj.SetParent(parentTrans);
                }
                _rotation = new Rotation
                {
                    Pivot = VerticalPivot,
                    ObjectsToRotate = objects,
                    Axis = Vector3.right,
                    Orientation = Orientation.Vertical,
                    ParentTransform = parentTrans
                };
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (_rotation == null)
            {
                return;
            }

            var delta = eventData.delta;
            var angle = _rotationSpeed;
            if (_rotation.Orientation == Orientation.Horizontal)
            {
                angle = delta.x < 0 ? angle : -angle;
            }
            else if (_rotation.Orientation == Orientation.Vertical)
            {
                angle = delta.y > 0 ? angle : -angle;
            }

            _rotation.ParentTransform.Rotate(_rotation.Axis, angle);
            DebugGui.Instance.Print("Rotation", transform.rotation.eulerAngles, "angle", angle);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (_rotation == null)
            {
                return;
            }
            
            var currentRotation = transform.rotation.eulerAngles;
            var y = Mathf.RoundToInt(currentRotation.y);
            
            var diff0 = Mathf.Abs(y);
            var diff1 = Mathf.Abs(y - 360);
            var diff2 = Mathf.Abs(y - 90);
            var diff3 = Mathf.Abs(y - 180);
            var diff4 = Mathf.Abs(y - 270);
            var minDiff = Mathf.Min(diff0, diff1, diff2, diff3, diff4);

            var targetAngle = 0;
            
            if (minDiff == diff1 || minDiff == diff0)
            {
                targetAngle = 0;
            }
            else if (minDiff == diff2)
            {
                targetAngle = 90;
            }
            else if (minDiff == diff3)
            {
                targetAngle = 180;
            }
            else if (minDiff == diff4)
            {
                targetAngle = 270;
            }
            
            // TODO: refactor
            _rotation.ParentTransform.DORotate((_rotation.Axis * (targetAngle - y)) + transform.rotation.eulerAngles, 0.6f).SetEase(Ease.Linear).OnComplete(
                () =>
                {
                    foreach (var t in _rotation.ObjectsToRotate)
                    {
                        t.SetParent(_originalParent);
                    }
                    Destroy(_rotation.ParentTransform.gameObject);
                    _rotation = null;
                });

            DebugGui.Instance.Print("OnEndDrag", eventData.delta, "final rotation", currentRotation, "targetAngle", targetAngle);

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