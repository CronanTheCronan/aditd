using UnityEngine;

namespace ADoorInsideTheDark.Player
{
    public sealed class PlayerContext : MonoBehaviour
    {
        [SerializeField] private Transform _body;
        [SerializeField] private Transform _cameraRoot;

        public Transform Body => _body != null ? _body : transform;
        public Transform CameraRoot => _cameraRoot;

        private void Awake()
        {
            if (_body == null)
            {
                _body = transform;
            }
        }

        private void OnValidate()
        {
            if (_body == null)
            {
                _body = transform;
            }
        }
    }
}
