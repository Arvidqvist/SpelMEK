using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityRefresher : MonoBehaviour
{
    [Tooltip("Insert your player here.")]
    [SerializeField]
    private GameObject player = null;

    [Tooltip("How near the player has to be to the gravity refresher to activate it.")]
    [SerializeField]
    private float minDistance = 1;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    private void Update()
    {
        if (Vector3.Distance(player.transform.position, this.transform.position) < minDistance && player.GetComponent<Controller>().GetFlipTokens() != 1)
        {
            player.GetComponent<Controller>().IncreaseFlipTokens();

            Debug.Log("IT HAS TOUCHED, FLIPPIES ARE NOW: " + player.GetComponent<Controller>().GetFlipTokens());
        }
    }
}
