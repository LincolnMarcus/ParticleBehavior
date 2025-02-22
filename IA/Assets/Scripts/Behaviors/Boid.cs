using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.Windows.Speech;
using static UnityEditor.PlayerSettings;
using static UnityEngine.RuleTile.TilingRuleOutput;
using Transform = UnityEngine.Transform;

public class Boid : MonoBehaviour
{
    Particle particle;

    [SerializeField] float speed = 3.0f;
    [SerializeField] float awarenessRadii = 4.0f;
    [SerializeField] float dangerRadii = 4.0f;
    [SerializeField] float coherenceFactor = 1.0f;
    [SerializeField] float alignFactor = 1.0f;
    [SerializeField] float separationFactor = 1.0f;
    [SerializeField] float populationPercent = 0.5f;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(Random.value > populationPercent) {  Destroy(gameObject); }
        particle = GetComponent<Particle>();
        gameObject.tag = "Boid";
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 steeringTarget = Vector2.zero;
        Vector2 position = transform.position;

        Dictionary<Transform, float> neighborDistancePairs = new Dictionary<Transform, float>();
        var d = particle.GetNeighborDistances(awarenessRadii, "Boid", 10);
        foreach (float f in d.Keys) {
            neighborDistancePairs.Add(d[f].transform, f);
        }

        steeringTarget += Cohere(neighborDistancePairs , position) * coherenceFactor;
        steeringTarget += Align(d, position) * alignFactor;
        steeringTarget += Separate(neighborDistancePairs , position) * separationFactor;

        particle.addVelocity(steeringTarget, speed);
    }

    private Vector2 Cohere( Dictionary<Transform, float> neighborDistancePairs , Vector2 position) {
        Vector2 res = Vector2.zero;
        foreach (Transform n in neighborDistancePairs.Keys) {
            res += (Vector2) n.position;
        }
        res /= neighborDistancePairs.Count;
        res = ( res - position ).normalized;
        return res;
    }
    private Vector2 Align(Dictionary<float, GameObject> distanceObjectPairs, Vector2 position) {
        Vector2 res = Vector2.zero;
        foreach ( GameObject n in distanceObjectPairs.Values ) {
            res += n.GetComponent<Rigidbody2D>().linearVelocity;
        }
        if (res.y == 0 && res.x == 0) { return Vector2.zero; }
        res /= distanceObjectPairs.Count;
        res *= 0.5f;
        return res;
    }
    private Vector2 Separate(Dictionary<Transform, float> neighborDistancePairs, Vector2 position) {
        Vector2 res = Vector2.zero;
        foreach (Transform n in neighborDistancePairs.Keys) {
            if(neighborDistancePairs[n] <= dangerRadii ) {
                res -= (Vector2)n.position - position;
            }
        }
        return res;
    }

    private void OnDrawGizmos() {
        Gizmos.DrawWireSphere(transform.position, awarenessRadii);
    }
}
