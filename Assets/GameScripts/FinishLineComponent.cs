using UnityEngine;
using UnityEngine.SceneManagement;

public class FinishLineComponent : MonoBehaviour
{
    public static bool playerWins = false;
    private int playerHitCount = 0;
    private int otherHitCount = 0;
    private bool playerCollision = false;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name.Contains("player") && !playerCollision)
        {
            playerCollision = true;
            playerHitCount++;
            Debug.Log("enter player object anme: " + other.gameObject.name + ", " + other.name + " count = " + playerHitCount);

        }
        else if(other.gameObject.name.Contains("Agent"))
        {   
            otherHitCount++;
            Debug.Log("game object anme: " + other.gameObject.name);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.gameObject.name.Contains("player"))
        {
            Debug.Log("exit player object anme: " + other.gameObject.name + ", " + other.name + " count = " + playerHitCount);
        }
        
        playerCollision = false;
        if (playerHitCount >= 4 && playerHitCount > otherHitCount/3)
        {
            playerWins = true;
            EndGame();
        }

        if (otherHitCount >= 12 && otherHitCount/3 > playerHitCount)
        {
            // end game
            playerWins = false;
            EndGame();
        }
    }


    void EndGame()
    {
        // unlocks the cursor before leaving the scene, to be able to click buttons
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (playerWins)
        {
            SceneManager.LoadScene("WinScene");
        }
        else
        {
            SceneManager.LoadScene("LoseScene");
        }

    }
}
