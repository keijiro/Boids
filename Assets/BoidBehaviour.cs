using UnityEngine;
using System.Collections;

public class BoidBehaviour : MonoBehaviour
{
    public BoidController controller;

    float noiseOffset;

    void Start()
    {
        noiseOffset = Random.value * 10.0f;
    }

    Vector3 GetSeparationVector(Transform target)
    {
        var diff = transform.position - target.transform.position;
        var diffLen = diff.magnitude;
        var scaler = Mathf.Clamp01(1.0f - diffLen / controller.neighborDist);
        return diff * (scaler / diffLen);
    }

    void RotateSmooth(Quaternion target)
    {
        var current = transform.rotation;
        if (current != target)
            transform.rotation = Quaternion.Lerp(target, current, Mathf.Exp(-controller.rotationCoeff * Time.deltaTime));
    }
    
    void Update()
    {
        // The current velocity randomized by noise.
        var velocity = Mathf.Lerp(controller.minVelocity, controller.maxVelocity, Mathf.PerlinNoise(Time.time, noiseOffset));

        // Initial vectors.
        var separation = Vector3.zero;
        var alignment = controller.transform.forward;
        var cohesion = controller.transform.position;

        // Looks up the overlapped boids.
        var overlappedBoids = Physics.OverlapSphere(transform.position, controller.neighborDist, controller.searchLayer);

        // Accumulates the vectors.
        foreach (var boid in overlappedBoids)
        {
            if (boid.gameObject == gameObject) continue;
            var t = boid.transform;
            separation += GetSeparationVector(t);
            alignment += t.forward;
            cohesion += t.position;
        }

        var averager = 1.0f / overlappedBoids.Length;
        alignment *= averager;
        cohesion *= averager;
        cohesion = (cohesion - transform.position).normalized;

        // Gets a rotation from the vectors.
        var direction = separation + alignment + cohesion;
        var rotation = Quaternion.FromToRotation(Vector3.forward, direction);

        // Applys to the transform.
        transform.position += transform.forward * (velocity * Time.deltaTime);
        RotateSmooth(rotation);
    }
}
