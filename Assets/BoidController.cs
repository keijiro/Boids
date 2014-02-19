using UnityEngine;
using System.Collections;

public class BoidController : MonoBehaviour
{
    public GameObject boidPrefab;

    public int numberOfAgents = 10;
    public float spawnRadius = 4.0f;

    public float minVelocity = 3.0f;
    public float maxVelocity = 9.0f;

    [Range(0.1f, 20.0f)]
    public float rotationCoeff = 4.0f;

    [Range(0.1f, 10.0f)]
    public float neighborDist = 2.0f;

    public LayerMask searchLayer;

    void Start()
    {
        for (var i = 0; i < numberOfAgents; i++)
        {
            var position = transform.position + Random.insideUnitSphere * spawnRadius;
            var boid = Instantiate(boidPrefab, position, Random.rotation) as GameObject;
            boid.GetComponent<BoidBehaviour>().controller = this;
        }
    }
}
