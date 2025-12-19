using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;
using UnityEngine.UIElements;

[RequireComponent(typeof(Rigidbody))]
public class Tomato : MonoBehaviour
{
    [Header("Targeting")]
    public Transform target;        

    [Header("Feedback UI")]
    public UnityEngine.UI.Image redScreenImage;    
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI DefeatWord;
    public float flashDuration = 0.2f;
    public AudioSource Splasher;

    [Header("Physics Settings")]
    public float throwForce = 18f;  
    public float liftForce = 5f;    
    
    [Header("Spawn Settings")]
    public float throwInterval = 3f; 
    public float spawnDistance = 14f; 

    private Rigidbody rb;
    private int score = 0;
    private bool hasHitTarget = false;
    private bool isFirstThrow = true; // Added to prevent score jump on start

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Start()
    {
        // FIX: Ensure the red screen is hidden the moment the game starts
        if (redScreenImage != null)
        {
            redScreenImage.enabled = false;
        }
        DefeatWord.enabled = false;

        UpdateScoreUI();
        ResetTomato();
        InvokeRepeating("SpawnAndThrow", 2f, throwInterval);
    }

    void SpawnAndThrow()
    {
        

        if (target == null) return;

        // Check for dodge: if not first throw and we didn't hit the target
        if (!isFirstThrow && !hasHitTarget)
        {
            ChangeScore(1);
        }

        isFirstThrow = false;
        hasHitTarget = false; 
        
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        float randomSide = Random.Range(-6f, 6f);
        Vector3 spawnPos = target.position + (-Vector3.forward * spawnDistance) + (target.right * randomSide);
        spawnPos.y = 2.5f; 
        transform.position = spawnPos;


        

        Vector3 direction = (target.position - transform.position).normalized;
        Vector3 finalForce = (direction * throwForce) + (Vector3.up * liftForce);

        if (randomSide > -2 && randomSide < 2)
        {
            finalForce += new Vector3(0, 0.8f, 0) +direction;
        }

        rb.AddForce(finalForce, ForceMode.VelocityChange);
        rb.AddTorque(Random.insideUnitSphere * 10f, ForceMode.Impulse);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("MainCamera") || other.name == "Main Camera")
        {
            hasHitTarget = true;
            ChangeScore(-1);
            Splasher.Play();
            
            if (redScreenImage != null)
            {
                // Stop any current flash to start a fresh one
                StopCoroutine("FlashRedScreen");
                StartCoroutine(FlashRedScreen());
            }
            
        }
        ResetTomato();
    }

    void ChangeScore(int amount)
    {
        score += amount;
        UpdateScoreUI();
    }

    void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + score;
        }
        if (score < -5)
        {
            DefeatWord.enabled = true;
            scoreText.enabled = false;
            CancelInvoke("SpawnAndThrow");
            
        }
    }

    void ResetTomato()
    {
        transform.position = new Vector3(0, -100, 0); 
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }

    IEnumerator FlashRedScreen()
    {
        redScreenImage.enabled = true;
        yield return new WaitForSeconds(flashDuration);
        redScreenImage.enabled = false;
    }
}