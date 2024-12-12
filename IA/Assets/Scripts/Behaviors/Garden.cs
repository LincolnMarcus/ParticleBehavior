using UnityEngine;

public class Garden : MonoBehaviour
{
    Particle particle;

    public bool plant;
    public float lifeRemaining;

    [SerializeField] float lifeSpan = 3f;

    [SerializeField] float actionPeriod = 0.1f;

    public float nextAction = 0.0f;

    [Header("Plant Config")]
    [SerializeField] float seedChance = 0.5f;
    

    private void Awake() {
        particle = GetComponent<Particle>();
        
    }

    void Start()
    {
        if( plant != true ) {
            if( Random.value < 0.9 ) {
                plant = true;
                GetComponent<SpriteRenderer>().color = Color.green;
                actionPeriod = 0.5f;
            } else {
                plant = false;
                actionPeriod = lifeSpan / 3f;
                GetComponent<SpriteRenderer>().color = new Color( 0.5882352941f , 0.2941176471f , 0f );
            }
        }

    }

    void Update()
    {
        if( Time.time > nextAction ) { 
            nextAction += actionPeriod;
            if( plant ) {
                if(Random.value < 0.5f ) {

                }
            }
            
        }
    }

    private void SpawnNewPlant() {
        GameObject copy = Instantiate( gameObject , GameObject.Find( "Manager" ).transform );
        copy.transform.position 

        Garden gardenCopy = copy.GetComponent<Garden>();
        gardenCopy.lifeRemaining = lifeSpan;
        gardenCopy.nextAction = 0f;
    }
}
