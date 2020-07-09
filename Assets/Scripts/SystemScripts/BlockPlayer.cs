using UnityEngine;

namespace SystemScripts
{
    public class BlockPlayer : MonoBehaviour
    {
        private void OnCollisionEnter2D(Collision2D other)
        {
            if (!other.gameObject.CompareTag("Player"))
            {
                GetComponent<BoxCollider2D>().isTrigger = true;
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                GetComponent<BoxCollider2D>().isTrigger = false;
            }
        }
    }
}