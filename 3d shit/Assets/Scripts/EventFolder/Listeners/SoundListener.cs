using UnityEngine;

namespace EventCallbacks
{
    public class SoundListener : MonoBehaviour
    {
        AudioSource tempAudioSource = null;

        void OnUnitDied(SoundEvent soundEvent)
        {
            Debug.Log("Sound playing: " + soundEvent.sound);

            if (soundEvent.sound != null)
            {
                if (soundEvent.audioSource != null)
                {
                    tempAudioSource = Instantiate(soundEvent.audioSource,
                                                  soundEvent.UnitGameObject.transform.position,
                                                  soundEvent.UnitGameObject.transform.rotation);

                    tempAudioSource.PlayOneShot(soundEvent.sound);

                    tempAudioSource.GetComponent<ObjectDestroyer>().SetLifeTime(soundEvent.sound.length);
                }
            }
        }

        void Start()
        {
            GravityRefresherEventCoordinator.Current.RegisterListener<SoundEvent>(OnUnitDied);
        }
    }
}