using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadNewScene : MonoBehaviour
{
    public Object newScene;
    [SerializeField] private string area;
    [SerializeField] private int index;
    [SerializeField] private PlayerController player;

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.name == "Player")
        {
            if(player.animator.GetFloat("moveX") == 1) // Set new direction the character will face in the new scene.
            {
                PlayerController.curDir = "East";
            } else if(player.animator.GetFloat("moveX") == -1)
            {
                PlayerController.curDir = "West";
            }
            else if (player.animator.GetFloat("moveY") == 1)
            {
                PlayerController.curDir = "North";
            }
            else if (player.animator.GetFloat("moveY") == -1)
            {
                PlayerController.curDir = "South";
            }
            StartCoroutine(Delay());  
        }
    }

    IEnumerator Delay()
    {
        yield return new WaitForSeconds(2); // Add a delay before switching to the new scene.
        SceneManager.LoadScene(newScene.name);
        PlayerController.spawnArea = area;
        PlayerController.spawnIndex = index;
        
    }
}
