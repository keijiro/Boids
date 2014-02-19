using UnityEngine;
using System.Collections;

public class AgentSpawner : MonoBehaviour
{
    public GameObject agentPrefab;
    public int numberOfAgent = 10;
    public float spawnRadius = 5.0f;

    void Start ()
    {
        var target = GameObject.Find("Target");

        for (var i = 0; i < numberOfAgent; i++)
        {
            var position = transform.position + Random.insideUnitSphere * spawnRadius;
            var agent = Instantiate(agentPrefab, position, Random.rotation) as GameObject;
            agent.GetComponent<FlockingBehaviour>().target = target;
        }
    }
}
