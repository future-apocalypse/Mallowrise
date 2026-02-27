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
        
        GameManager.Instance.PlayerDied();
        GameManager.Instance.ResetScore();
        
    }
}
