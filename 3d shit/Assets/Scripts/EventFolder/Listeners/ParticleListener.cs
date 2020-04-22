using UnityEngine;

namespace EventCallbacks
{
    public class ParticleListener : MonoBehaviour
    {
        void OnUnitDied(ParticleEvent particleEvent)
        {
            Debug.Log("Particles: " + particleEvent.particles);

            if (particleEvent.particles != null)
            {
                Instantiate(particleEvent.particles, particleEvent.UnitGameObject.transform.position,
                                                     particleEvent.UnitGameObject.transform.rotation);
            }
        }

        void Start()
        {
            GravityRefresherEventCoordinator.Current.RegisterListener<ParticleEvent>(OnUnitDied);
        }
    }
}