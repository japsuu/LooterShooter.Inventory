using UnityEngine;
using UnityEngine.UI;

namespace InventorySystem.Inventories.Rendering
{
    [RequireComponent(typeof(Image))]
    public class InventorySlotsRenderer : MonoBehaviour
    {
        private Image _slotsImage;

        public InventoryRenderer InventoryRenderer { get; private set; }


        private void Awake()
        {
            _slotsImage = GetComponent<Image>();
        }


        public void Initialize(InventoryRenderer inventoryRenderer, float width, float height)
        {
            InventoryRenderer = inventoryRenderer;
            
            _slotsImage.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
            _slotsImage.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
        }
    }
}