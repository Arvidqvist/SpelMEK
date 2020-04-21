using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class SceneManagementScript : MonoBehaviour
{
    public GameController gameController;
    public Transform RespawnPosition;
    public Controller controller;

    private void Awake()
    {
        
    }
    void Start()
    {
        gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void loadScene(int buildIndex)
    {
        if (SceneManager.GetActiveScene().buildIndex != buildIndex)
        {
            RespawnPosition = null;
        }
        else
        {
            
        }

    }
}
