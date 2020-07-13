using UnityEngine;

namespace SystemScripts
{
    public class DeathFromBelow : MonoBehaviour
    {
        private void OnCollisionEnter2D(Collision2D other)
        {
            if (other.gameObject.CompareTag("Player") || other.gameObject.CompareTag("BigPlayer"))
            {
                GameStatusController.Live -= 1;
                GameStatusController.IsBigPlayer = false;
                GameStatusController.PlayerTag = "Player";
                other.gameObject.GetComponent<PlayerController>().isDead = true;
            }
            else
            {
                Destroy(other.gameObject);
            }
        }
    }
}