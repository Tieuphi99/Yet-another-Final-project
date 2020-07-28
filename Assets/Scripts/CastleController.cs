using UnityEngine;

public class CastleController : MonoBehaviour
{
    private Animator _castleAnim;
    private static readonly int FinishB = Animator.StringToHash("Finish_b");

    private void Awake()
    {
        _castleAnim = GetComponent<Animator>();
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player") || other.gameObject.CompareTag("BigPlayer"))
        {
            _castleAnim.SetBool(FinishB, true);
        }
    }
}
