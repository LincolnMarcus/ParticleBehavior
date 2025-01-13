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
    [SerializeField] int maxEnemyNCount = 60;
    [SerializeField] int maxNCount = 5;



    GameObject[] enemies;
    Dictionary<float, GameObject> neighbors;

    bool enemy = false;

    private int frameCount = 0;
    private void Awake()
    {
        particle = GetComponent<Particle>();
        if (Random.value < 0.01f)
        {
            gameObject.tag = "Enemy";
            enemy = true;
        }
    }
    void Start() {
        if( gameObject.tag == "Enemy" ) {
            gameObject.GetComponent<SpriteRenderer>().material.color = Color.red;
            //gameObject.GetComponent<TrailRenderer>().enabled = false;
            awarenessRadii = 10f;
        }
        enemies = GameObject.FindGameObjectsWithTag("Enemy");
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (enemy)
            neighbors = particle.GetNeighborDistances(awarenessRadii, null, maxEnemyNCount);
        else
            neighbors = particle.GetNeighborDistances(awarenessRadii, null, maxNCount);

        Vector2 totalRepellingForce = Vector2.zero;
        Vector2 pos = transform.position;

        Vector2 toCenter = Vector2.zero;
        bool amEnemy = ( gameObject.tag == "Enemy" );
        if( neighbors.Count > 0 ) {
            Vector2 centerOfMass = Vector2.zero;
            if( !amEnemy ) { awarenessRadii = particle.awarenessRadii; }
            foreach ( var n in neighbors.Values ) {
                Vector2 nPos = n.transform.position;
                if( !amEnemy && enemies.Contains(n) ) {
                    Vector2 directionToEnemy = pos - nPos;
                    totalRepellingForce += directionToEnemy.normalized * (avoidForce / Vector2.Distance(pos, nPos));
                }
                centerOfMass += nPos;
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
