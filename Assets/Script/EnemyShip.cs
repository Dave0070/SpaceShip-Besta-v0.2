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

    // Zmienne do kontrolowania p³ynnoœci obrotu
    public float rotationSpeed = 2f; // Szybkoœæ obrotu w kierunku gracza
    public float fleeRotationSpeed = 1f; // Szybkoœæ obrotu podczas ucieczki

    // Zmienne do kontrolowania uników
    public float evadeInterval = 2f; // Czêstotliwoœæ uników
    public float minEvadeDuration = 0.5f; // Minimalny czas trwania uniku
    public float maxEvadeDuration = 1.5f; // Maksymalny czas trwania uniku
    public float minEvadeStrength = 200f; // Minimalna si³a uniku
    public float maxEvadeStrength = 500f; // Maksymalna si³a uniku
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

        // Jeœli przeciwnik mo¿e strzelaæ, podlatuje do gracza
        if (canShoot)
        {
            // P³ynny obrót w kierunku gracza
            Quaternion targetRotation = Quaternion.LookRotation(targetLocation);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            rb.AddRelativeForce(Vector3.forward * Mathf.Clamp((distance - 10) / 50, 0f, 1f) * thrust);
            fleeing = false; // Jeœli przeciwnik podlatuje do gracza, to nie ucieka
        }
        else
        {
            // Jeœli przeciwnik ma przerwê od strzelania, to odlatuje od gracza i wykonuje unik
            FleeFromPlayer();

            // Wykonaj unik, jeœli nie jesteœmy w trakcie uniku
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
        // Wybierz losowy kierunek uniku, z wyj¹tkiem kierunku przodu
        Vector3 evadeDirection = Random.onUnitSphere;
        evadeDirection.y = Mathf.Abs(evadeDirection.y); // Upewnij siê, ¿e unik nie bêdzie w dó³
        evadeDirection.z = Mathf.Abs(evadeDirection.z); // Upewnij siê, ¿e unik nie bêdzie do przodu

        // Wybierz losow¹ si³ê uniku w zakresie od minEvadeStrength do maxEvadeStrength
        float evadeStrength = Random.Range(minEvadeStrength, maxEvadeStrength);

        // Wybierz losowy czas trwania uniku
        evadeDuration = Random.Range(minEvadeDuration, maxEvadeDuration);

        // Dodaj si³ê uniku
        rb.AddRelativeForce(evadeDirection * evadeStrength);

        // Ustaw timer uniku
        evadeTimer = evadeInterval;

        // Oznacz, ¿e jesteœmy w trakcie uniku
        evading = true;
    }

    // Ustawia losowy czas do nastêpnego uniku
    void SetRandomEvadeTimer()
    {
        evadeTimer = evadeInterval;
    }

    // Metoda wywo³ywana, gdy przeciwnik rozpocznie strzelanie
    public void StartShooting()
    {
        canShoot = true;
    }

    // Metoda wywo³ywana, gdy przeciwnik przestanie strzelaæ
    public void StopShooting()
    {
        canShoot = false;
    }
}