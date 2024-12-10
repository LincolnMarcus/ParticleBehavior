using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{

    public GameObject part;

    public int populationMultiplier = 2;

    List<List<Vector2>> grid = new List<List<Vector2>>();

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

        foreach ( List<Vector2> row in grid )
        {
            foreach ( Vector2 point in row)
            {
                if (Random.value < 0.2f)
                {
                    GameObject g = Instantiate(part, transform);
                    g.transform.localScale = scale;
                    if (populationMultiplier <= 1)
                    {
                        g.transform.localScale = new Vector2(0.5f, 0.5f);
                    }
                    g.transform.position = point;
                }
            }
        }
    }
    void Update()
    {
        
    }
}
