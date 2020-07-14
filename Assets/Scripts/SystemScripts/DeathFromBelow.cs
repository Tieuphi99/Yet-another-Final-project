using UnityEngine;

namespace SystemScripts
{
    public class DeathFromBelow : MonoBehaviour
    {
        private void OnCollisionEnter2D(Collision2D other)
        {
            if (other.gameObject.CompareTag("Player") || other.gameObject.CompareTag("BigPlayer"))
            {
                GameStatusController.IsBigPlayer = false;
                GameStatusController.PlayerTag = "Player";
                GameStatusController.IsDead = true;
            }
            else
            {
                Destroy(other.gameObject);
            }
        }
    }
}