using UnityEngine;
using System.Collections;

public class FlockingBehaviour : MonoBehaviour
{
    public GameObject target;

    public float minVelocity = 5.0f;
    public float maxVelocity = 7.0f;
    public float rotationCoeff = -10.0f;
    public float neighborDist = 2.0f;
    public LayerMask searchLayer;

    float noiseOffset;

    void Start()
    {
        noiseOffset = Random.value * 3.0f;
    }
    
    void Update()
    {
        var velocity = Mathf.PerlinNoise(Time.time, noiseOffset) * (maxVelocity - minVelocity) + minVelocity;

        var diff = transform.position - target.transform.position;
        var separation = diff.normalized * Mathf.Max(neighborDist - diff.magnitude, 0.0f);
        var alignment = target.transform.forward;
        var cohesion = target.transform.position;

        var flocks = Physics.OverlapSphere(transform.position, neighborDist, searchLayer);

        foreach (var flock in flocks)
        {
            if (flock.gameObject == gameObject) continue;
            diff = transform.position - flock.transform.position;
            separation += diff.normalized * Mathf.Max(neighborDist - diff.magnitude, 0.0f);
            alignment += flock.transform.forward;
            cohesion += flock.transform.position;
        }

        var avg = 1.0f / flocks.Length;
        alignment *= avg;

        cohesion *= avg;
        cohesion = (cohesion - transform.position).normalized;

        var wantedDirection = separation + alignment + cohesion;

        var rotation = Quaternion.FromToRotation(Vector3.forward, wantedDirection);

        transform.rotation = ExpEase.Out(transform.rotation, rotation, rotationCoeff);
        transform.position += transform.forward * (velocity * Time.deltaTime);
    }
}
