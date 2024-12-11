using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Particle : MonoBehaviour
{
    Rigidbody2D rb;

    private void Start() {
        rb = GetComponent<Rigidbody2D>();

        //Debug.Log( gameObject.name+" Closest : "+GetNeighborDistances( 3f ).Keys.ToList()[0].name );
    }
    void Update()
    {
        
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

    public Dictionary<float, GameObject> GetNeighborDistances( float radius )
    {
        Dictionary<float , GameObject> result = new Dictionary<float , GameObject>();

        List<Collider2D> neighbors = Physics2D.OverlapCircleAll( transform.position , radius ).ToList();
        neighbors.Remove(GetComponent<Collider2D>());

        foreach ( var neighbor in neighbors ) {
            float d = Vector2.Distance( transform.position , neighbor.transform.position );
            try {
                result.Add( d , neighbor.gameObject );
            } catch {
                result.Add( d+Random.value , neighbor.gameObject );
            }
        }


        return result;
    }
}
