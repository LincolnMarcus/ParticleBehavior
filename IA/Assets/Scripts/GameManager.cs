using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{

    public GameObject part;

    public int populationMultiplier = 2;

    List<List<Vector2>> grid = new List<List<Vector2>>();

    List<Particle> particles = new List<Particle>();

    public bool savePopulationsToFile = true;

    public int secondsActive;
    public enum SaveMode {
        Garden
    }

    public SaveMode saveType;

    string savePath;

    void Start()
    {
        secondsActive = 0;
        savePath = "Assets/SaveData/" + Enum.GetName(typeof(SaveMode), saveType) + ".json";
        WriteData(new Dictionary<int, List<int>> { });
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

        InvokeRepeating("SaveGardenTimeline", 0, 1);
    }

    public void SaveGardenTimeline() {
        var existingData = ReadData();
        int gardenerCount = 0;
        int plantCount = 0;
        foreach (Transform g in transform) {
            if(g.GetComponent<Garden>().plant) {
                plantCount++;
            } else {
                gardenerCount++;
            }
        }
        List<int> newData = new List<int> { gardenerCount, plantCount };
        existingData.Add(secondsActive, newData);
        WriteData(existingData);

        secondsActive++;
    }

    private void WriteData( Dictionary<int, List<int>> timeline ) {
        if(File.Exists(savePath)) {
            string json = JsonConvert.SerializeObject( timeline );
            using (StreamWriter sw = new StreamWriter( savePath )) {
                sw.Write(json);
            }
        } else {
            Debug.LogError(savePath + " Does not exist!");
        }
    }

    private Dictionary<int, List<int>> ReadData( ) {
        if (File.Exists(savePath)) {
            using (StreamReader sr = new StreamReader(savePath)) {
                string json = sr.ReadToEnd();
                return JsonConvert.DeserializeObject<Dictionary<int, List<int>>>(json);
            }
        } else {
            Debug.LogError(savePath + " Does not exist!");
            return null;
        }
    }
}


