/*****************************************************************************
// File Name : PlayerController.cs
// Author : Drew Higgins
// Creation Date : September 5th, 2025
//
// Brief Description : This script controls player actions in the game.
*****************************************************************************/

using System.Runtime.CompilerServices;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private int playerSpeed;
    public int playerHealth;
    public int playerScore;
    
    public TMP_Text scoreText;
    public TMP_Text livesText;

    [SerializeField] private InputAction move;
    [SerializeField] private InputAction shoot;
    [SerializeField] private InputAction restart;
    [SerializeField] private InputAction quit;

    private float moveDirection;
    private int shootCountdown;
    private bool isFiring;

    public GameObject bullet;
    [SerializeField] private GameObject bulletSpawn;

    [SerializeField] private AudioClip shootSound;

    [SerializeField] private Animator animator;

    /// <summary>
    /// Start establishes everything the player/game needs when the game boots up
    /// </summary>
    void Start()
    {
        //Sets all the necessary values to what they should be should be at the start of the game

        playerScore = 0;
        playerHealth = 3;
        isFiring = false;
        shootCountdown = 0;

        //Calls the function for the action map
        SetupActions();
    }

    /// <summary>
    /// Allows the player to do the necessary game actions
    /// </summary>
    private void SetupActions()
    {
        //Enables the action map
        playerInput.currentActionMap.Enable();

        //Sets up the actions
        move = playerInput.currentActionMap.FindAction("Move");
        shoot = playerInput.currentActionMap.FindAction("Shoot");
        restart = playerInput.currentActionMap.FindAction("Restart");
        quit = playerInput.currentActionMap.FindAction("Quit");

        move.started += Move_started;
        shoot.started += Shoot_started;
        restart.started += Restart_started;
        quit.started += Quit_started;
        shoot.canceled += Shoot_canceled;
    }

    /// <summary>
    /// Allows the player to quit the game
    /// </summary>
    /// <param name="obj"></param>
    private void Quit_started(InputAction.CallbackContext obj)
    {
        Application.Quit();
        UnityEditor.EditorApplication.isPlaying = false;
    }

    /// <summary>
    /// Allows the player to restart
    /// </summary>
    /// <param name="obj"></param>
    private void Restart_started(InputAction.CallbackContext obj)
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    /// <summary>
    /// Stops the shooting of bullets when the button is released
    /// </summary>
    /// <param name="obj"></param>
    private void Shoot_canceled(InputAction.CallbackContext obj)
    {
        if(animator != null)
        {
            isFiring = false;
            animator.SetBool("isFiring", false);
        }
    }

    /// <summary>
    /// Allows the player to shoot a bullet
    /// </summary>
    /// <param name="obj"></param>
    private void Shoot_started(InputAction.CallbackContext obj)
    {
        if(bullet != null)
        {
            shootCountdown = 0;
            isFiring = true;
            animator.SetBool("isFiring", true);
        }
    }

    /// <summary>
    /// Allows the player to move up and down
    /// </summary>
    /// <param name="obj"></param>
    private void Move_started(InputAction.CallbackContext obj)
    {
        print("Movement started!");
    }

    /// <summary>
    /// For if the player collides with an enemy
    /// </summary>
    public void Collision()
    {
        print("Collision occurred!");

        //Decreases player health
        playerHealth--;

        if (playerHealth <= 0)
        {
            //Calls the function to end the game if player health is zero or lower
            EndGame();
        }
    }

    /// <summary>
    /// Updates the score when a player kills an enemy
    /// </summary>
    public void ScoreUpdate()
    {
        playerScore = playerScore + 100;
        string score = playerScore.ToString();
        scoreText.text = "Score: " + score;
    }

    /// <summary>
    /// Updates the life amount when the player takes damage
    /// </summary>
    public void TextUpdate()
    {
        livesText.text = "Lives: " + playerHealth;
    }

    /// <summary>
    /// The necessary functions required to stop the game from continuing when the player loses
    /// </summary>
    public void EndGame()
    {
        //Disables the bullets, actions, and player movement
        bullet.SetActive(false);
        move.started -= Move_started;
        shoot.started -= Shoot_started;
        shoot.canceled -= Shoot_canceled;
        isFiring = false;
        playerSpeed = 0;
    }

    /// <summary>
    /// What is being checked and called continously
    /// </summary>
    void FixedUpdate()
    {
        //Allows the player to continuously move
        moveDirection = move.ReadValue<float>();
        player.GetComponent<Rigidbody2D>().linearVelocity = new Vector2(moveDirection * playerSpeed,
            moveDirection * playerSpeed);


        //Function for making the player continue shooting while holding down space, checks if the button is down
        if (isFiring == true)
        {
            //Checks that the delay between shots is over
            if (shootCountdown == 0)
            {
                animator.SetBool("isFiring", true);
                AudioSource.PlayClipAtPoint(shootSound, transform.position);
                print("Shooting started!");
                Instantiate(bullet, bulletSpawn.transform.position, Quaternion.identity);
                shootCountdown = 15;
            }
            else
            {
                //If it's not, it comes to this function to decrease the countdown and remove any explosion effects
                animator.SetBool("isFiring", false);
                shootCountdown--;
            }
        }
    }
}
