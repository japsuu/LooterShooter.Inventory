using UnityEngine;

namespace LooterShooter.DevTools
{
    public class LimitFps : MonoBehaviour
    {
        [SerializeField] private int _targetFrameRate = 60;
 
        private void Awake()
        {
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = _targetFrameRate;
        }
    }
}