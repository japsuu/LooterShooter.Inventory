using UnityEngine;
using UnityEngine.UI;

namespace InventorySystem.Inventories.Spatial.Rendering
{
    [RequireComponent(typeof(RectTransform))]
    [RequireComponent(typeof(Image))]
    public class SpatialInventoryFloaterPositionValidator : MonoBehaviour
    {
        public static SpatialInventoryFloaterPositionValidator Singleton;
        
        [SerializeField] private Color _validPositionColor = new Color(0f, 1f, 0f, 0.4f);
        [SerializeField] private Color _invalidPositionColor = new Color(1f, 0f, 0f, 0.4f);
        
        private RectTransform _rectTransform;
        private Image _validatorImage;


        private void Awake()
        {
            if (Singleton != null)
            {
                Debug.LogError($"Multiple {nameof(SpatialInventoryFloaterPositionValidator)} in scene!");
                return;
            }

            Singleton = this;
            
            _rectTransform = GetComponent<RectTransform>();
            _validatorImage = GetComponent<Image>();
            Hide();
        }


        private void LateUpdate()
        {
            _rectTransform.SetAsLastSibling();
        }


        public void Hide() => _validatorImage.enabled = false;


        public void UpdateSize(float width, float height)
        {
            _validatorImage.enabled = true;
            
            _rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
            _rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
            
            //_rectTransform.SetParent(entity.transform.parent);
            // int targetIndex = entity.RectTransform.GetSiblingIndex() - 1;
            // if(targetIndex < 1)
            //     entity.RectTransform.SetSiblingIndex(targetIndex + 2);
            // else
            //     _rectTransform.SetSiblingIndex(targetIndex);
        }


        public void UpdatePosition(Vector2 anchoredPosition, SpatialFloater floater)
        {
            _validatorImage.enabled = true;
            //_rectTransform.anchoredPosition = anchoredPosition;
            _rectTransform.position = anchoredPosition;

            bool isValidPosition = floater.IsBoundsValid();
            _validatorImage.color = isValidPosition ? _validPositionColor : _invalidPositionColor;
        }
    }
}