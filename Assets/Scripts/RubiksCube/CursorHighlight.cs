using UnityEngine;
using UnityEngine.EventSystems;

namespace RubiksCube
{
    public class CursorHighlight : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private Renderer _renderer;
        [SerializeField] private Color _highLightColor;
        
        private Color _originalColor;

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (_originalColor == _highLightColor)
            {
                return;
            }
            var material = _renderer.material;
            _originalColor = material.color;
            material.color = _highLightColor;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _renderer.material.color = _originalColor;
        }
    }
}