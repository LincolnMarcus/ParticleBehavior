using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class AttractImposter : MonoBehaviour
{
    Particle particle;

    [SerializeField] float awarenessRadii = 3f;
    [SerializeField] float boundsAvoidanceFactor = 5f;
    [SerializeField] float avoidForce = 5f;

    GameObject[] enemies;
    void Start() {
        particle = GetComponent<Particle>();
        if( Random.value < 0.01f ) {
            gameObject.tag = "Enemy";
            gameObject.GetComponent<SpriteRenderer>().material.color = Color.red;
            gameObject.GetComponent<TrailRenderer>().startColor = Color.red;
            gameObject.GetComponent<TrailRenderer>().endColor = Color.red;
            awarenessRadii = 10f;
        }

    }

    // Update is called once per frame
    void Update()
    {
        enemies = GameObject.FindGameObjectsWithTag( "Enemy" );

        var neighbors = particle.GetNeighborDistances( awarenessRadii );

        Vector2 totalRepellingForce = Vector2.zero;

        if( gameObject.tag != "Enemy" ) {
            foreach( GameObject enemy in enemies ) {
                Vector2 directionToEnemy = transform.position - enemy.transform.position;
                float distanceToEnemy = Vector2.Distance( transform.position , enemy.transform.position );
                if( distanceToEnemy < awarenessRadii ) {
                    // Calculate repelling force inversely proportional to distance
                    Vector2 repellingForce = directionToEnemy.normalized * ( avoidForce / distanceToEnemy );
                    totalRepellingForce += repellingForce;

                }
            }
            particle.addVelocity( totalRepellingForce , 10f );
        }

        Vector2 centerOfMass = Vector2.zero;
        if( neighbors != null ) {
            foreach( var n in neighbors.Values ) {
                centerOfMass += (Vector2) n.transform.position; 
            }
            centerOfMass /= neighbors.Count;
            Vector2 toCenter = ( centerOfMass - (Vector2) transform.position ).normalized;

            particle.addVelocity( toCenter, 1f );
        }

        

    }

    private void OnDrawGizmos() {
        Gizmos.DrawWireSphere( transform.position , awarenessRadii );
    }
}
