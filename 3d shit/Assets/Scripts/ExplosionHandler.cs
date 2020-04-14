using UnityEngine;

public class ExplosionHandler : MonoBehaviour
{
    [SerializeField]
    private Transform[] explosionSpawner;
    [SerializeField]
    private Transform explosionCube;

    private void Update()
    {
        Instantiate(explosionCube);
    }
}
