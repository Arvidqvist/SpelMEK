using System.Collections.Generic;
using UnityEngine;

namespace EventCallbacks
{
    public class GravityRefresherEventCoordinator : MonoBehaviour
    {
        List<GameObject> gravRefresherList = new List<GameObject>();

        public GameObject player = null;
        public GameObject currentGravityRefresher = null;

        public float minDistance = 0;
        public float currentLowestDistance = 10;

        private void Awake()
        {
            foreach (GameObject gravityRefresher in GameObject.FindGameObjectsWithTag("GravityRefresher"))
            {
                gravRefresherList.Add(gravityRefresher);
            }

            minDistance = gravRefresherList[0].GetComponent<GravityRefresher>().minDistance;

            player = GameObject.FindGameObjectWithTag("Player");
        }

        private void Update()
        {
            foreach (GameObject g in gravRefresherList)
            {
                if (Vector3.Distance(player.transform.position, g.transform.position) < currentLowestDistance)
                {
                    currentLowestDistance = Vector3.Distance(player.transform.position, g.transform.position);
                }

                if (currentLowestDistance < minDistance)
                {
                    currentGravityRefresher = g;

                    Debug.Log(currentLowestDistance + ", " + minDistance +
                              ", I am a big stinky poopoo head! " +
                              player.GetComponent<Controller>().GetFlipTokens() + ", " +
                              g.transform.position);

                    gravRefresherList.Remove(g);
                    break;
                }
            }

            if (currentGravityRefresher != null)
            {
                Debug.Log("current is: " + currentGravityRefresher.transform.position);
            }

            if (currentLowestDistance < minDistance && player.GetComponent<Controller>().GetFlipTokens() < 1)
            {
                currentGravityRefresher.GetComponent<GravityRefresher>().GivePlayerFlipToken();

                currentLowestDistance = 10;
            }
        }

        static private GravityRefresherEventCoordinator __Current;
        static public GravityRefresherEventCoordinator Current
        {
            get
            {
                if (__Current == null)
                {
                    __Current = GameObject.FindObjectOfType<GravityRefresherEventCoordinator>();
                }
                return __Current;
            }
        }

        delegate void EventListener(Event ei);
        Dictionary<System.Type, List<EventListener>> eventListeners;

        void OnEnable()
        {
            __Current = this;
        }

        public void RegisterListener<T>(System.Action<T> listener) where T : Event
        {
            System.Type eventType = typeof(T);

            if (eventListeners == null)
            {
                eventListeners = new Dictionary<System.Type, List<EventListener>>();
            }

            if (eventListeners.ContainsKey(eventType) == false || eventListeners[eventType] == null)
            {
                eventListeners[eventType] = new List<EventListener>();
            }

            void wrapper(Event evnt)
            {
                listener((T)evnt);
            }

            eventListeners[eventType].Add(wrapper);
        }

        public void UnregisterListener<T>(System.Action<T> listener) where T : Event
        {
            if (eventListeners == null)
            {
                return;
            }

            System.Type eventType = typeof(T);

            if (eventListeners.ContainsKey(eventType) == true || eventListeners[eventType] != null)
            {
                void wrapper(Event evnt)
                {
                    listener((T)evnt);
                }

                eventListeners[eventType].Remove(wrapper);
            }
        }

        public void FireEvent(Event evnt)
        {
            System.Type trueEventClass = evnt.GetType();

            if (eventListeners == null || eventListeners[trueEventClass] == null)
            {
                return;
            }

            foreach (EventListener el in eventListeners[trueEventClass])
            {
                el(evnt);
            }
        }
    }
}