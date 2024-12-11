using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class Avoid : MonoBehaviour
{
    Particle particle;

    [SerializeField] float awarenessRadii = 3f;
    [SerializeField] float boundsAvoidanceFactor = 5f;
    void Start()
    {
        particle = GetComponent<Particle>();
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float steer = 0f;
        float x = transform.position.x; float y = transform.position.y;
        var neighbors = particle.GetNeighborDistances( awarenessRadii );
        
        if( neighbors != null ) {
            List<float> byDistance = neighbors.Keys.ToList();
            byDistance.Sort();
            List<GameObject> neighborsByDistance = new List<GameObject>();
            foreach( float distance in byDistance ) { neighborsByDistance.Add( neighbors[ distance ] ); }
            foreach( var n in neighborsByDistance ) {
                Vector2 t = n.GetComponent<Collider2D>().ClosestPoint( transform.position );
                float dx = t.x - x;
                float dy = t.y - y;

                float angle = -Mathf.Atan2( dx, dy ) * Mathf.Rad2Deg;
                steer += angle;
            }

        }

        particle.Rotate( steer , 4f );

        particle.SetVelocity( 2f );
    }

    private void OnDrawGizmos() {
        Gizmos.DrawWireSphere( transform.position , awarenessRadii );
    }
}
