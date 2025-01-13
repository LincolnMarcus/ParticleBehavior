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


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
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

        steeringTarget += Cohere(neighborDistancePairs , position);
        steeringTarget += Align(d, position);
        steeringTarget += Separate(neighborDistancePairs , position);

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
        res /= distanceObjectPairs.Count;
        res = (res - particle.GetVelocity()) / 8f;
        return res;
    }
    private Vector2 Separate(Dictionary<Transform, float> neighborDistancePairs, Vector2 position) {
        Vector2 res = Vector2.zero;
        foreach (Transform n in neighborDistancePairs.Keys) {
            if(neighborDistancePairs[n] <= dangerRadii ) {
                res -= position - (Vector2) n.position;
            }
        }
        return res;
    }
}
