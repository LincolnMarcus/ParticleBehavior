using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class AttractImposter : MonoBehaviour
{
    Particle particle;

    public float awarenessRadii = 3f;
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
            gameObject.GetComponent<SpriteRenderer>().material.color = Color.black;
            gameObject.GetComponent<TrailRenderer>().enabled = false;
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
        bool amEnemy = ( gameObject.tag == "Enemy" );
        if( neighbors.Count > 0 ) {
            Vector2 centerOfMass = Vector2.zero;
            if( !amEnemy ) { awarenessRadii = particle.awarenessRadii; }
            foreach ( var n in neighbors.Values ) {
                Vector2 nPos = n.transform.position;
                idx++;
                if( !amEnemy && enemies.Contains(n) ) {
                    Vector2 directionToEnemy = pos - nPos;
                    totalRepellingForce += directionToEnemy.normalized * (avoidForce / Vector2.Distance(pos, nPos));
                }
                centerOfMass += nPos;
                if (idx > 10) { break; }
            }
            centerOfMass /= neighbors.Count;
            toCenter = (centerOfMass - (Vector2)pos).normalized;
        } else {
            if( !amEnemy ) { awarenessRadii = particle.awarenessRadii * 5f; }
        }
        try { particle.addVelocity(toCenter + (totalRepellingForce * 10f), 1f); }
        catch { Debug.Log(particle == null); }
        
    }

    private void OnDrawGizmos() {
        Gizmos.DrawWireSphere(transform.position , awarenessRadii );
    }
}
