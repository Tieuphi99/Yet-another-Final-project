using UnityEngine;

namespace SystemScripts
{
    public class BlockBigPlayer : MonoBehaviour
    {
        private void OnCollisionEnter2D(Collision2D other)
        {
            if (!other.gameObject.CompareTag("BigPlayer"))
            {
                GetComponent<BoxCollider2D>().isTrigger = true;
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.CompareTag("BigPlayer"))
            {
                GetComponent<BoxCollider2D>().isTrigger = false;
            }
        }
    }
}