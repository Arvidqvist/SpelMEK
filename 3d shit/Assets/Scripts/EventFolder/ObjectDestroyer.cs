using UnityEngine;

namespace EventCallbacks
{
    public class ObjectDestroyer : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("The amount of time this object will exist in the world")]
        private float lifeTime = 5f;

        void Start()
        {
            lifeTime += Time.time;
        }

        void Update()
        {
            if (Time.time > lifeTime)
            {
                Destroy(gameObject);
            }
        }

        public void SetLifeTime(float newLifeTime)
        {
            lifeTime = newLifeTime;
        }
    }
}