using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerScript : MonoBehaviour
{


    public int health;


    public Image healthBar;

    public bool isDead;
    public TextMeshProUGUI statusText;
    public Vector3 spawnPoint;
    public GameObject arms;

    public float turnSpeed=300;

    Rigidbody2D rb;
    float angle;

    public string horizontalAxis;
    public string verticalAxis;

    Quaternion targetRotation;



    private void Awake()
    {

        health = 100;
        float barVal = ((float)health / 100f);
        healthBar.fillAmount = barVal;
        isDead = false;
        statusText.text = "";
        spawnPoint = transform.position;
        rb = GetComponent<Rigidbody2D>();

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!isDead)
        {
            //Vector2 shootDir = Vector2.right * Input.GetAxis("Horizontal") + Vector2.up * Input.GetAxis("Vertical");
            CalculateDirection();
            Rotate();
            //Vector3 forwardVector = Quaternion.Euler(shootDir) * Vector3.forward;
            
        }
    }

    void CalculateDirection()
    {
        angle = Mathf.Atan2(Input.GetAxis(horizontalAxis), Input.GetAxis(verticalAxis));
        angle = Mathf.Rad2Deg * angle;
        angle += Camera.main.transform.eulerAngles.y;
    }

    void Rotate()
    {
        targetRotation = Quaternion.Euler(0, 0, -angle);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);
    }


    private void OnDrawGizmos()
    {
        Vector2 shootDir = Vector2.right * Input.GetAxis("Horizontal") + Vector2.up * Input.GetAxis("Vertical");
        Ray ray = new Ray();
        ray.origin = transform.position;
        ray.direction = shootDir;
        Gizmos.DrawRay(ray);
    }


    public void TakeDamage(int damage)
    {
        health -= damage;
        float barVal = ((float)health / 100f);

        healthBar.fillAmount = barVal;

        if(health<0)
        {
            Die();
        }
    }

  
    public PlayerScript Die()
    {
        isDead = true;

        StartCoroutine(WaitForRespawn());

        return this;
    }

    IEnumerator WaitForRespawn()
    {
        statusText.text = "Respawning in 3...";
        yield return new WaitForSeconds(1f);
        statusText.text = "Respawning in 2...";
        yield return new WaitForSeconds(1f);
        statusText.text = "Respawning in 1...";
        yield return new WaitForSeconds(1f);
        statusText.text = "";
        transform.position = spawnPoint;
        health = 100;
        float barVal = ((float)health / 100f);

        healthBar.fillAmount = barVal;

        rb.velocity = Vector2.zero;
        rb.rotation = 0;

    }

    //TODO: track mouse location in relation to center point of char
    //TODO: add negative force on click
    //TODO: raycast direction on click, check for hittables
    //TODO: deal dmg knockback, etc,
}
