using System;
using System.Collections.Generic;
using UnityEngine;

namespace RubiksCube
{
    public class Block : MonoBehaviour
    {
        private const int MaxColliderCount = 9;
        private static readonly Vector3 HorizontalOverlapBoxExtents = new(4, 0.25f, 4);
        private static readonly Vector3 VerticalOverlapBoxExtents = new(0.25f, 4, 4);

        [SerializeField] private LayerMask _layer;
        [SerializeField] private List<BlockSide> _sides;

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
            Gizmos.DrawWireCube(transform.position, HorizontalOverlapBoxExtents);
            Gizmos.DrawWireCube(transform.position, VerticalOverlapBoxExtents);
        }

        private void OnGUI()
        {
            GUILayout.Space(100);
            if (GUILayout.Button("Get Horizontal Pieces"))
            {
                GetHorizontalPieces();
                foreach (var c in _colliders)
                {
                    if (c != null)
                    {
                        Debug.Log(c.transform.parent.name);
                    }
                }
            }
        }
    }
}
