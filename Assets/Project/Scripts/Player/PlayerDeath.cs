using UnityEngine;

public class PlayerDeath : MonoBehaviour
{
    public static PlayerDeath Instance;
    private bool _isDead;

    private void OnTriggerEnter(Collider other)
    {
        if(_isDead) return;
        
        if (other.CompareTag("Chocolate"))
        {
            Die();
        }
    }

    private void Die()
    {
        _isDead = true;
        
        Debug.Log("Player is dead");
        
        GameManager.Instance.PlayerDied();
        GameManager.Instance.ResetScore();
    }

    // private void FreezeInput()
    // {
    //     var movement = GetComponent<PlayerMovement>();
    //     if (movement != null)
    //         movement.enabled = false;
    //     
    //     var rb = GetComponent<Rigidbody>();
    //     if (rb != null)
    //     {
    //         rb.linearVelocity = Vector3.zero;
    //         rb.isKinematic = true;
    //     }
    // }
}
