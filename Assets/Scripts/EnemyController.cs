/*****************************************************************************
// File Name : EnemyController.cs
// Author : Drew Higgins
// Creation Date : September 6th, 2025
//
// Brief Description : This script controls how the enemies behave in the game.
*****************************************************************************/

using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    private PlayerController playerController;

    [SerializeField] private int enemySpeed;
    [SerializeField] private int enemyHealth;

    [SerializeField] private AudioClip enemyHitSound;
    [SerializeField] private AudioClip enemyDeathSound;
    [SerializeField] private AudioClip playerHitSound;

    [SerializeField] private Animator animator;
    
    /// <summary>
    /// Start connects to the PlayerController script and makes the enemies move upon spawning in
    /// </summary>
    void Start()
    {
        playerController = GameObject.FindFirstObjectByType<PlayerController>();

        //enemySpeed variable is set in the inspector to give the enemies differing movement values
        GetComponent<Rigidbody2D>().linearVelocity = new Vector2(enemySpeed, 0);   
    }

    /// <summary>
    /// If something collides with the enemy, different things can happen. This function controls that
    /// </summary>
    /// <param name="collision"></param>
    private void OnCollisionEnter2D(Collision2D collision)
    {
        //If the enemy collides with the player, the enemy is destroyed and the player loses a life
        if (collision.gameObject.tag == "Player")
        {
            AudioSource.PlayClipAtPoint(enemyDeathSound, transform.position);
            playerController.Collision();
            Destroy(gameObject);
            Debug.Log("Health lost!");
            playerController.TextUpdate();
            AudioSource.PlayClipAtPoint(playerHitSound, transform.position);
        }
        //If the enemy collides with a bullet, the enemy takes a tick of damage
        else if (collision.gameObject.tag == "Bullet")
        {
            GetComponent<Rigidbody2D>().linearVelocity = new Vector2(0, 0);
            StartCoroutine(HitSlow());
            enemyHealth--;
            AudioSource.PlayClipAtPoint(enemyHitSound, transform.position);

            //If the damage taken brings the enemy to zero health (or lower), then it plays the death animation
            //and disappears from the scene
            if (enemyHealth <= 0)
            {
                AudioSource.PlayClipAtPoint(enemyDeathSound, transform.position);
                animator.SetBool("isDead", true);
                GetComponent<Rigidbody2D>().linearVelocity = new Vector2(0, 0);
                StartCoroutine(DeathSlow());
                Debug.Log("Enemy killed!");
            }
        }
    }

    /// <summary>
    /// Slows down the time between bullet contact and the enemy being destroyed to let the animation play
    /// </summary>
    /// <returns></returns>
    private IEnumerator DeathSlow()
    {
        yield return new WaitForSeconds(0.25f);
        Destroy(gameObject);
    }

    /// <summary>
    /// Briefly stops the enemy when hit
    /// </summary>
    /// <returns></returns>
    private IEnumerator HitSlow()
    {
        yield return new WaitForSeconds(0.5f);
        GetComponent<Rigidbody2D>().linearVelocity = new Vector2(enemySpeed, 0);
        Debug.Log("Sped back up!");
    }

    /// <summary>
    /// Constantly checking for when the player dies to start the EndGame function in PlayerController
    /// </summary>
    private void Update()
    {
        if (playerController.playerHealth <= 0)
        {
            if(gameObject.tag == "Enemy")
            {
                DestroyImmediate(gameObject, true);
            }

            playerController.EndGame();
        }
    }
}
