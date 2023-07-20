using UnityEngine;

namespace LooterShooter.Tools.Extensions
{
    public static class RectTransformExtensions
    {
        public static Vector2 GetScreenSpacePosition(this RectTransform rectTransform, Vector2 anchoredPosition)
        {
            // Calculate the world space position of the anchoredPosition.
            Vector2 anchoredPositionWorldSpace = rectTransform.TransformPoint(anchoredPosition);

            // Convert the world space position to screen space.
            Vector2 screenSpacePosition = RectTransformUtility.WorldToScreenPoint(null, anchoredPositionWorldSpace);

            return screenSpacePosition;
        }
    }
}