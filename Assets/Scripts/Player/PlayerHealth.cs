using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using WebXR;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth;
    public float invincibilityTime = 1.0f;
    private bool isInvincible = false;
    public int totalkills;
    public TextMeshProUGUI health, kills;

    private bool isGameOver = false;

    public GameObject gameOverScreen; // Assign in Unity
    public TextMeshProUGUI gameOverText; // Assign in Unity
    public WebXRController controller; // Assign Right Controller in Unity

    void Start()
    {
        currentHealth = maxHealth;
        gameOverScreen.SetActive(false); // Hide at start
    }

    public void TakeDamage(int damage)
    {
        if (isGameOver || isInvincible) return; // Don't take damage if game over or invincible

        currentHealth -= damage;
        Debug.Log("Player took damage: " + damage);

        if (currentHealth <= 0)
        {
            GameOver(false); // Player loses
        }
        else
        {
            StartCoroutine(InvincibilityCooldown());
        }
    }

    IEnumerator InvincibilityCooldown()
    {
        isInvincible = true;
        yield return new WaitForSeconds(invincibilityTime);
        isInvincible = false;
    }

    public void Heal(int amount)
    {
        if (isGameOver) return; // No healing after game over

        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
        Debug.Log("Player healed: " + amount);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isGameOver) return; // Ignore triggers if the game is over

        if (other.CompareTag("bullet"))
        {
            TakeDamage(1);
        }
        else if (other.CompareTag("EndGoal"))
        {
            GameOver(true); // Player wins
        }

    }

    private void OnCollisionStay(Collision collision)
    {
        if (isGameOver) return; // Ignore collisions if the game is over

        if (collision.gameObject.CompareTag("Obstacle"))
        {
            TakeDamage(1);
        }
        else if (collision.gameObject.CompareTag("lava"))
        {
            currentHealth = 0;
            GameOver(false); // Player loses
        }
    }

    void GameOver(bool won)
    {
        isGameOver = true;
        gameOverScreen.SetActive(true);

        if (won)
        {
            gameOverText.text = "YOU WIN!";
            Debug.Log("YOU WIN! Press A to Restart.");
        }
        else
        {
            gameOverText.text = "YOU LOSE!";
            Debug.Log("YOU LOSE! Press A to Restart.");
        }
    }

    private void Update()
    {
        health.text = currentHealth.ToString();
        kills.text = totalkills.ToString();

        if (isGameOver && (Input.GetKeyDown(KeyCode.A) || controller.GetButtonDown(WebXRController.ButtonTypes.ButtonA)))
        {
            RestartGame();
        }
    }

    void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
