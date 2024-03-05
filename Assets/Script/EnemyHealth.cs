using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public delegate void DeathAction();
    public event DeathAction OnDeath;

    public int maxHealth = 100;
    private int currentHealth;

    void Start()
    {
        currentHealth = maxHealth;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlayerBullet")) // Sprawdzamy, czy to pocisk gracza
        {
            TakeDamage();
        }
    }

    void TakeDamage()
    {
        currentHealth -= 10;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        if (OnDeath != null)
        {
            OnDeath(); // Wywo³ujemy zdarzenie œmierci
        }
        Destroy(gameObject);
    }
}
