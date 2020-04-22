using UnityEngine;

namespace EventCallbacks
{
    public class DebugListener : MonoBehaviour
    {
        void OnUnitDied(DebugEvent debugInfo)
        {
            Debug.Log("A unit has died, it sends this message: " + debugInfo.debugMessage);
        }

        void Start()
        {
            GravityRefresherEventCoordinator.Current.RegisterListener<DebugEvent>(OnUnitDied);
        }
    }
}