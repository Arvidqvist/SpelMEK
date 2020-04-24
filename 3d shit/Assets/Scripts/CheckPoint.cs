using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CheckPoint : MonoBehaviour
{
    private GameController gameController;
    private GameObject playerObject;
    private int currentSceneIndex;
    public bool testButton = false;
    public bool activated = false;
    // Start is called before the first frame update
    void Start()
    {
        gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
        playerObject = GameObject.FindGameObjectWithTag("Player");
        if (gameController == null)
        {
            Debug.Log("The GameControllerScript is not found on an object with the tag 'GameController' in the scene");
        }
        currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
    }

    // Update is called once per frame
    void Update()
    {
        if (testButton)
        {
            SetCheckPoint();
            testButton = false;
        }
    }
    public void SetCheckPoint()
    {
        if (gameController != null && activated == false)
        {
            GameObject testObject = new GameObject();
            testObject.transform.SetPositionAndRotation(transform.position, GameObject.FindGameObjectWithTag("Player").transform.rotation);
            gameController.SetSpawnTransform(testObject.transform);
            gameController.SetControllerSettingsAtSpawn(GameObject.FindGameObjectWithTag("Player").GetComponent<Controller>());
            this.GetComponent<BoxCollider>().enabled = false;
            activated = true;
        }
       
    }
}
