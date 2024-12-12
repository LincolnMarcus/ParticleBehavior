using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{

    public GameObject part;

    public int populationMultiplier = 2;

    List<List<Vector2>> grid = new List<List<Vector2>>();

    List<Particle> particles = new List<Particle>();

    void Start()
    {

        for (int y = 0; y < (20 * populationMultiplier) + 1; y++) {
            List<Vector2> row = new List<Vector2>();
            for (int x = 0; x < (20 * populationMultiplier) + 1; x++)
            {
                Vector2 point = new Vector2((float)x / populationMultiplier - 10,
                    (float)y / populationMultiplier - 10);

                
                row.Add(point);
            }
            grid.Add(row);
        }

        Vector2 scale = new Vector2( 1f / populationMultiplier, 1f / populationMultiplier);

        int idx = 0;

        foreach ( List<Vector2> row in grid )
        {
            foreach ( Vector2 point in row)
            {
                if (Random.value < 0.2f)
                {
                    GameObject g = Instantiate(part, transform);
                    g.name = idx.ToString();
                    g.transform.localScale = scale;
                    g.GetComponent<Particle>().awarenessRadii = 2f/populationMultiplier;
                    if (populationMultiplier <= 1)
                    {
                        g.transform.localScale = new Vector2(0.5f, 0.5f);
                    }
                    g.transform.position = point;
                    particles.Add(g.GetComponent<Particle>());
                    idx++;
                }
            }
        }
        foreach( var g in particles ) {
            g.GetComponent<Particle>().worldParticles = particles;
        }
    }
}
