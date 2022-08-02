using System;
using System.Collections.Generic;
using UnityEngine;

namespace RubiksCube
{
    public class Block : MonoBehaviour
    {
        [SerializeField] private List<BlockSide> _sides;

        private Collider[] _colliders;

        private void GetHorizontalPieces()
        {
            var extents = new Vector3(3, 0.25f, 3);
            // TODO use non-alloc
            _colliders = Physics.OverlapBox(transform.position, extents);
        }

        private void OnGUI()
        {
            GUILayout.Space(100);
            if (GUILayout.Button("Get Horizontal Pieces"))
            {
                GetHorizontalPieces();
                foreach (var c in _colliders)
                {
                    Debug.Log(c.transform.position);
                }
            }
        }
    }
}
