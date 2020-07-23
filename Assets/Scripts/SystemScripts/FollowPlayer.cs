using UnityEngine;

namespace SystemScripts
{
    public class FollowPlayer : MonoBehaviour
    {
        public GameObject player;
        private float _furthestPlayerPosition;
        private float _currentPlayerPosition;

        private void Start()
        {
            if (player != null)
            {
                _currentPlayerPosition = player.transform.position.x;
            }
        }

        // Camera follows player
        void LateUpdate()
        {
            if (player != null)
            {
                _currentPlayerPosition = player.transform.position.x;
                if (_currentPlayerPosition >= _furthestPlayerPosition)
                {
                    _furthestPlayerPosition = _currentPlayerPosition;
                }

                if (_currentPlayerPosition > 3.5f &&
                    _currentPlayerPosition >= _furthestPlayerPosition)
                {
                    transform.position = !GameStatusController.IsBossBattle
                        ? new Vector3(player.transform.position.x, 5, -10)
                        : new Vector3(285, 5, -10);
                }
            }
        }
    }
}