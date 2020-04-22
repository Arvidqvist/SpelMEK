using UnityEngine;

namespace EventCallbacks
{
    public class DeathListener : MonoBehaviour
    {
        void OnUnitDied(UnitDeathEvent unitDeathInfo)
        {
            Debug.Log("Unit: " + unitDeathInfo.UnitGameObject.name + " has died");
            Destroy(unitDeathInfo.UnitGameObject);
        }

        void Start()
        {
            GravityRefresherEventCoordinator.Current.RegisterListener<UnitDeathEvent>(OnUnitDied);
        }
    }
}