using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Particle : MonoBehaviour
{
    Rigidbody2D rb;
    public float awarenessRadii;

    public List<Particle> worldParticles = new List<Particle>();
    private void Awake() {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Rotate( float degrees, float speed=1f ) {
        // Get the current Z rotation angle
        float currentAngle = transform.eulerAngles.z;

        // Interpolate the angle using MoveTowardsAngle
        float newAngle = Mathf.MoveTowardsAngle( currentAngle , degrees , speed * Time.deltaTime );

        // Apply the interpolated angle
        transform.rotation = Quaternion.Euler( 0 , 0 , newAngle );
    }

    public void Scale( float scale ) {
        transform.localScale = new Vector2( scale , scale );
    }

    public float GetScale() {
        return transform.localScale.x;
    }

    public void addVelocity( Vector2 direction, float amt ) {
        rb.AddForce( direction*amt );
    }

    public void SetVelocity( float amt ) {
        if( rb == null )
            return;
        rb.linearVelocity = transform.right * amt;
    }

    public Vector2 GetVelocity() {
        return rb.linearVelocity;
    }

    public List<Particle> GetAllParticles() {
        return worldParticles;
    }

    public Dictionary<float, GameObject> GetNeighborDistances( float radius , string tag=null , int max=5 )
    {
        Vector2 pos = transform.position;
        Dictionary<float , GameObject> result = new Dictionary<float , GameObject>();

        List<Collider2D> neighbors = Physics2D.OverlapCircleAll( pos, radius ).ToList();
        neighbors.Remove(GetComponent<Collider2D>());
        int idx = 0;
        foreach ( var neighbor in neighbors ) {
            if ( tag != null && neighbor.tag != tag ) { continue; }
            float d = Vector2.Distance(pos, neighbor.transform.position );
            try {
                result.Add( d , neighbor.gameObject );
            } catch {
                result.Add( d+Random.value , neighbor.gameObject );
            }
            if (idx > max) { break; }

            idx++;
        }

        return result;
    }
}
