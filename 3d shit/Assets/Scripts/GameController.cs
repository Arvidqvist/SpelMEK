using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
/* this object needs a tag called GameController for other scripts to be able to find it*/

public class GameController : MonoBehaviour
{
    private static GameController instance;

    //spawn stuff

    //public Vector3 SpawnTransformPosition;
    //public Quaternion SpawnTransformRotation;
    public Controller ControllerSettingsAtSpawn;
    public bool testreload = false;
    //[Tooltip("This sets itself so ignore it")]
    public bool newSceneIsLoaded = false;
    // material for the switches
    [Tooltip("Put in the material you want the switch to have when it is activated")]
    public Material switchActivatedMaterial;
    [Tooltip("put in the material you want the switch to have when it is deactivated")]
    public Material switchDeactivatedMaterial;
    
    // Start is called before the first frame update
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(instance);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (testreload)
        {
            loadScene(SceneManager.GetActiveScene().buildIndex);
            testreload = false;
        }
    }

    public void loadScene(int buildIndex)
    {
        // for this to work the controller needs in start to set the spawntransform and controllersettings to something if they are null
        if (SceneManager.GetActiveScene().buildIndex != buildIndex)
        {
            newSceneIsLoaded = true;
        }
        SceneManager.LoadScene(buildIndex);
    }

    public void SetSpawnTransform(Transform newTransform)
    {
        Debug.Log(newTransform.rotation+ "rotation");
        transform.SetPositionAndRotation(newTransform.position,newTransform.rotation);
    }

    public void SetControllerSettingsAtSpawn(Controller newControllerSettings)
    {
        //if (ControllerSettingsAtSpawn != null)
        //{
        //    ControllerSettingsAtSpawn = null;
        //}
        ControllerSettingsAtSpawn = Instantiate(newControllerSettings, newControllerSettings.transform);
        ControllerSettingsAtSpawn.tag = "PlayerReference";
        ControllerSettingsAtSpawn.gameObject.SetActive(false);
    }
    public Controller getControllerSpawnSettings()
    {
        return ControllerSettingsAtSpawn;
    }
    public Transform getSpawnTransform()
    {
        return transform;
    }
    public void Respawn()
    {
        loadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
