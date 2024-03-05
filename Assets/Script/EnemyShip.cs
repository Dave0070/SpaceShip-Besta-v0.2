using UnityEngine;

public class EnemyShip : MonoBehaviour
{
    public float torque = 500f;
    public float thrust = 1000f;
    private Rigidbody rb;
    public Transform player;
    public float fleeDistance = 20f;
    public float maxChaseDistance = 30f;
    public Transform chaseBoundary;

    private bool fleeing = false;
    private bool canShoot = true;

    // Zmienne do kontrolowania p�ynno�ci obrotu
    public float rotationSpeed = 2f; // Szybko�� obrotu w kierunku gracza
    public float fleeRotationSpeed = 1f; // Szybko�� obrotu podczas ucieczki

    // Zmienne do kontrolowania unik�w
    public float evadeInterval = 2f; // Cz�stotliwo�� unik�w
    public float minEvadeDuration = 0.5f; // Minimalny czas trwania uniku
    public float maxEvadeDuration = 1.5f; // Maksymalny czas trwania uniku
    public float minEvadeStrength = 200f; // Minimalna si�a uniku
    public float maxEvadeStrength = 500f; // Maksymalna si�a uniku
    private float evadeTimer = 0f;
    private float evadeDuration = 0f;
    private bool evading = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        SetRandomEvadeTimer();
    }

    void FixedUpdate()
    {
        Vector3 targetLocation = player.position - transform.position;
        float distance = targetLocation.magnitude;

        // Je�li przeciwnik mo�e strzela�, podlatuje do gracza
        if (canShoot)
        {
            // P�ynny obr�t w kierunku gracza
            Quaternion targetRotation = Quaternion.LookRotation(targetLocation);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            rb.AddRelativeForce(Vector3.forward * Mathf.Clamp((distance - 10) / 50, 0f, 1f) * thrust);
            fleeing = false; // Je�li przeciwnik podlatuje do gracza, to nie ucieka
        }
        else
        {
            // Je�li przeciwnik ma przerw� od strzelania, to odlatuje od gracza i wykonuje unik
            FleeFromPlayer();

            // Wykonaj unik, je�li nie jeste�my w trakcie uniku
            if (!evading)
            {
                Evade();
            }
        }

        // Aktualizacja timera uniku
        if (evading)
        {
            evadeTimer -= Time.deltaTime;
            if (evadeTimer <= 0f)
            {
                evading = false;
                SetRandomEvadeTimer();
            }
        }
    }

    // Metoda do odlatywania od gracza
    void FleeFromPlayer()
    {
        Vector3 fleeDirection = transform.position - player.position;
        Quaternion fleeRotation = Quaternion.LookRotation(fleeDirection);
        transform.rotation = Quaternion.Lerp(transform.rotation, fleeRotation, fleeRotationSpeed * Time.deltaTime);
        rb.AddRelativeForce(Vector3.forward * thrust);
        fleeing = true;
    }

    // Metoda do wykonania uniku
    void Evade()
    {
        // Wybierz losowy kierunek uniku, z wyj�tkiem kierunku przodu
        Vector3 evadeDirection = Random.onUnitSphere;
        evadeDirection.y = Mathf.Abs(evadeDirection.y); // Upewnij si�, �e unik nie b�dzie w d�
        evadeDirection.z = Mathf.Abs(evadeDirection.z); // Upewnij si�, �e unik nie b�dzie do przodu

        // Wybierz losow� si�� uniku w zakresie od minEvadeStrength do maxEvadeStrength
        float evadeStrength = Random.Range(minEvadeStrength, maxEvadeStrength);

        // Wybierz losowy czas trwania uniku
        evadeDuration = Random.Range(minEvadeDuration, maxEvadeDuration);

        // Dodaj si�� uniku
        rb.AddRelativeForce(evadeDirection * evadeStrength);

        // Ustaw timer uniku
        evadeTimer = evadeInterval;

        // Oznacz, �e jeste�my w trakcie uniku
        evading = true;
    }

    // Ustawia losowy czas do nast�pnego uniku
    void SetRandomEvadeTimer()
    {
        evadeTimer = evadeInterval;
    }

    // Metoda wywo�ywana, gdy przeciwnik rozpocznie strzelanie
    public void StartShooting()
    {
        canShoot = true;
    }

    // Metoda wywo�ywana, gdy przeciwnik przestanie strzela�
    public void StopShooting()
    {
        canShoot = false;
    }
}