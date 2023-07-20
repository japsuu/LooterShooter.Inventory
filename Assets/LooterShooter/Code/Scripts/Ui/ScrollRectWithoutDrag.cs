using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace LooterShooter.Ui
{
    public class ScrollRectWithoutDrag : ScrollRect
    {
        public override void OnBeginDrag(PointerEventData eventData) { }


        public override void OnDrag(PointerEventData eventData) { }


        public override void OnEndDrag(PointerEventData eventData) { }
    }
}