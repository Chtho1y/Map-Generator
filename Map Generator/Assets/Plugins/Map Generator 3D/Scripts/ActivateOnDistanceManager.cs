using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CNB
{
    /// <summary>
    /// Evaluates every "EvaluationFrequency" the distance from every GameObject with  "ActivateOnDistance.cs component" to Game Object tagged as "Player"
    /// </summary>
    public class ActivateOnDistanceManager : MonoBehaviour
    {
        [HideInInspector]
        public bool AutomaticallySetPlayerAsTarget = true;
        [HideInInspector]
        public Transform ProximityTarget;
        [HideInInspector]
        public bool AutomaticallyGrabControlledObjects = true;
        [HideInInspector]
        public float EvaluationFrequency = 0.5f;

        GameObject _spawnHolder;

        protected float _lastEvaluationAt = 0f;
        List<GameObject> taggedGameObjects = new List<GameObject>();

        //.................................
        // the distance from the proximity center (the player) under which the object should be enabled
        [Tooltip("the distance from the proximity center (the player) under which the object should be enabled")]
        public float EnableDistance = 100f;
        /// the distance from the proximity center (the player) after which the object should be disabled
        [Tooltip("the distance from the proximity center (the player) after which the object should be disabled")]
        public float DisableDistance = 100f;

        protected virtual void Start()
        {
            taggedGameObjects.Clear();
            _spawnHolder = GameObject.FindGameObjectWithTag("SpawnManager");
            GrabControlledObjects();
        }

        protected virtual void GrabControlledObjects()
        {
            if (AutomaticallyGrabControlledObjects && _spawnHolder != null )
            {
                int childCount = _spawnHolder.transform.childCount;
                for (int i = 0; i < childCount; i++)
                {
                    Transform child = _spawnHolder.transform.GetChild(i);
                    int childChildCount = child.childCount;
                    for (int j = 0; j < childChildCount; j++)
                    {
                        Transform childChild = child.GetChild(j);
                        if (childChild.tag == "DistanceManaged")
                        {
                            taggedGameObjects.Add(childChild.gameObject);
                        }
                    }
                }
            }
        }
        
        protected virtual void SetPlayerAsTarget()
        {
            if (AutomaticallySetPlayerAsTarget)
            {
                ProximityTarget = GameObject.FindGameObjectWithTag("Player")?.transform;
            }
        }

        protected virtual void Update()
        {
            if (!ProximityTarget)
            {
                SetPlayerAsTarget();
            }

            if (taggedGameObjects.Count == 0)
            {
                GrabControlledObjects();
            }

            if (ProximityTarget)
            {
                EvaluateDistance();
            }
        }

        protected virtual void EvaluateDistance()
        {
            if (Time.time - _lastEvaluationAt > EvaluationFrequency)
            {
                _lastEvaluationAt = Time.time;
            }
            else
            {
                return;
            }
            foreach (GameObject proxy in taggedGameObjects)
            {
                if (proxy)
                {
                    float distance = Vector3.Distance(proxy.transform.position, ProximityTarget.position);
                    if (proxy.gameObject.activeInHierarchy && distance > DisableDistance)
                    {
                        proxy.gameObject.SetActive(false);
                    }
                    if (!proxy.gameObject.activeInHierarchy && distance < EnableDistance)
                    {
                        proxy.gameObject.SetActive(true);
                    }
                }
            }
        }
    }
}