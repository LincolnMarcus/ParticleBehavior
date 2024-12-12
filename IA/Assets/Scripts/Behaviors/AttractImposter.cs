using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class AttractImposter : MonoBehaviour
{
    Particle particle;

    [SerializeField] float awarenessRadii = 3f;
    [SerializeField] float avoidForce = 5f;

    GameObject[] enemies;
    Dictionary<float, GameObject> neighbors;

    private int frameCount = 0;
    private void Awake()
    {
        particle = GetComponent<Particle>();
        if (Random.value < 0.01f)
        {
            gameObject.tag = "Enemy";
        }
    }
    void Start() {
        if( gameObject.tag == "Enemy" ) {
            gameObject.GetComponent<SpriteRenderer>().material.color = Color.red;
            gameObject.GetComponent<TrailRenderer>().startColor = Color.red;
            gameObject.GetComponent<TrailRenderer>().endColor = Color.red;
            awarenessRadii = 10f;
        }
        enemies = GameObject.FindGameObjectsWithTag("Enemy");

        neighbors = particle.GetNeighborDistances(awarenessRadii);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        neighbors = particle.GetNeighborDistances(awarenessRadii);

        Vector2 totalRepellingForce = Vector2.zero;
        Vector2 pos = transform.position;

        Vector2 toCenter = Vector2.zero;
        int idx = 0;
        if( neighbors != null ) {
            Vector2 centerOfMass = Vector2.zero;
            bool amEnemy = (gameObject.tag == "Enemy");
            foreach ( var n in neighbors.Values ) {
                Vector2 nPos = n.transform.position;
                idx++;
                if( !amEnemy && n.tag == "Enemy" ) {
                    Vector2 directionToEnemy = pos - nPos;
                    totalRepellingForce += directionToEnemy.normalized * (avoidForce / Vector2.Distance(pos, n.transform.position));
                }
                centerOfMass += nPos;
                if (idx > 10) { break; }
            }
            centerOfMass /= neighbors.Count;
            toCenter = (centerOfMass - (Vector2)pos).normalized;
        }
        try { particle.addVelocity(toCenter + (totalRepellingForce * 10f), 1f); }
        catch { Debug.Log(particle == null); }
        
    }

    private void OnDrawGizmos() {
        //Gizmos.DrawWireSphere(pos , awarenessRadii );
    }
}
