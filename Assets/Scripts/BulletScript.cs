/*****************************************************************************
// File Name : BulletScript.cs
// Author : Drew Higgins
// Creation Date : September 6th, 2025
//
// Brief Description : This script controls how the bullet acts in the game.
*****************************************************************************/

using System.Collections;
using UnityEngine;

public class BulletScript : MonoBehaviour
{
    private PlayerController playerController;
    [SerializeField] private Animator animator;

    /// <summary>
    /// Start connects to the PlayerController script and makes the bullet shoot to the right when spawned
    /// </summary>
    void Start()
    {
        playerController = GameObject.FindFirstObjectByType<PlayerController>();

        GetComponent<Rigidbody2D>().linearVelocity = new Vector2(20, 0);
    }

    /// <summary>
    /// A variety of things will happen when the player collides with an enemy
    /// </summary>
    /// <param name="collision"></param>
    private void OnCollisionEnter2D(Collision2D collision)
    {
        //Stops the bullet and plays the explosion animation
        GetComponent<Rigidbody2D>().linearVelocity = new Vector2(0, 0);
        animator.SetBool("isFiring", true);
        
        //Calls to update the score in the PlayerController script
        if(playerController.bullet != null)
        {
            playerController.ScoreUpdate();
        }
        
        //Starts the coroutine to put a small amount of time between the explosion spawn and the bullet
        //object getting destroyed.
        StartCoroutine(KillExplosion());
        
    }

    /// <summary>
    /// Creates a small amount of time (0.25 seconds) so the player can see the explosion animation
    /// </summary>
    /// <returns></returns>
    private IEnumerator KillExplosion()
    {
        yield return new WaitForSeconds(0.25f);
        animator.SetBool("isFiring", false);
        Destroy(gameObject);
    }

    /// <summary>
    /// Will despawn any active bullet in the scene when the player loses the game
    /// </summary>
    private void Update()
    {
        if (playerController.playerHealth <= 0)
        {
            Destroy(gameObject);
        }
    }
}
