using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EventCallbacks
{
    public class GravityRefresher : MonoBehaviour
    {
        [Tooltip("Insert your player here.")]
        [SerializeField]
        public GameObject player = null;

        [Tooltip("How near the player has to be to the gravity refresher to activate it.")]
        [SerializeField]
        public float minDistance = 1;

        [Header("Event Settings")]

        [SerializeField]
        [Tooltip("The sound that the object will play on death.")]
        private AudioClip deathNoise = null;
        [SerializeField]
        [Tooltip("The particles that will be spawned on the object's death.")]
        private ParticleSystem deathParticles = null;

        [SerializeField]
        private AudioSource audioSource;

        List<Event> events = new List<Event>();

        private bool canGiveToken = false;

        private void Awake()
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }

        public void GivePlayerFlipToken()
        {
            UnitDeathEvent deathEvent = new UnitDeathEvent
            {
                UnitGameObject = gameObject,
                EventDescription = "Unit " + gameObject.name + " has died."
            };

            SoundEvent soundEvent = new SoundEvent
            {
                EventDescription = "Noise " + deathNoise + " is playing.",
                UnitGameObject = this.gameObject,
                audioSource = audioSource,
                sound = deathNoise
            };

            ParticleEvent particleEvent = new ParticleEvent
            {
                EventDescription = "Particles " + deathParticles + " is playing.",
                UnitGameObject = this.gameObject,
                particles = deathParticles
            };

            events.Add(soundEvent);
            events.Add(particleEvent);
            events.Add(deathEvent);

            canGiveToken = true;
        }

        private void Update()
        {
            if (canGiveToken)
            {
                player.GetComponent<Controller>().IncreaseFlipTokens();

                Debug.Log("IT HAS TOUCHED, FLIPPIES ARE NOW: " + player.GetComponent<Controller>().GetFlipTokens());

                foreach (Event e in events)
                {
                    GravityRefresherEventCoordinator.Current.FireEvent(e);
                }
            }
        }
    }
}
