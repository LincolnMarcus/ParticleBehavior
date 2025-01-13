using System.Linq;
using UnityEngine;

public class Garden : MonoBehaviour
{
    Particle particle;

    public bool plant;
    public float lifeRemaining;

    [SerializeField] float actionPeriod = 0.1f;

    public float nextAction = 0.0f;

    [Header("Plant Config")]
    [SerializeField] float seedChance = 0.5f;
    [SerializeField] float plantLifeSpan = 3f;
    [SerializeField] float plantInitializationChance = 0.95f;

    [Header("Gardener Config")]
    [SerializeField] float childChance = 0.2f;
    [SerializeField] int requiredFoodForChild = 2;
    [SerializeField] float gardenerRadii = 3f;
    [SerializeField] float speed = 2f;
    [SerializeField] float gardenerLifeSpan = 3f;
    [SerializeField] float foodLifeReplenish = 2f;

    public int foodCount = 0;

    private void Awake() {
        particle = GetComponent<Particle>();
        nextAction = Time.time;
    }
    void Start()
    {
        if ( particle.GetAllParticles().Count > 0 ) {
            if ( Random.value < plantInitializationChance ) {
                plant = true;
                gameObject.tag = "Plant";
                gameObject.layer = 6;
                GetComponent<SpriteRenderer>().color = Color.green;
                actionPeriod = 0.5f;
                lifeRemaining = plantLifeSpan + plantLifeSpan * (Random.value - 0.5f);
            } else {
                plant = false;
                gameObject.layer = 7;
                actionPeriod = 0.5f;
                GetComponent<SpriteRenderer>().color = new Color(0.5882352941f, 0.2941176471f, 0f);
                lifeRemaining = gardenerLifeSpan + gardenerLifeSpan * (Random.value - 0.5f);
            }
        }
    }

    void FixedUpdate()
    {
        if( Time.time > nextAction ) { 
            nextAction = Time.time + actionPeriod;
            if( plant ) {
                if(Random.value < seedChance*5) {
                    SpawnNewCopy();
                }
            }
            if (!plant) {
                if (Random.value < childChance*foodCount && foodCount >= requiredFoodForChild) {
                    SpawnNewCopy();
                    foodCount -= requiredFoodForChild;
                }
            }
        }

        if (!plant) {
            var neighbors = particle.GetNeighborDistances(gardenerRadii, "Plant", 1);
            if( neighbors.Count > 0 ) {
                Vector2 plantPosition = neighbors.Values.ToList()[0].transform.position;
                Vector2 target = (plantPosition - (Vector2)transform.position).normalized;
                particle.addVelocity(target, speed);
            }
            gameObject.GetComponent<SpriteRenderer>().color =
                new Color( 0.5882352941f, 0.2941176471f, 0f, lifeRemaining/gardenerLifeSpan );
        } if( plant ) {
            particle.SetVelocity(0f);
            gameObject.GetComponent<SpriteRenderer>().color = 
                new Color(0 , Mathf.Clamp( lifeRemaining/plantLifeSpan, 0f , 1f ), 0);
        }

        if (lifeRemaining <= 0f) {
            Destroy(gameObject);
        }

        lifeRemaining -= Time.deltaTime;
    }

    private void SpawnNewCopy() {
        particle.worldParticles.Clear();
        GameObject copy = Instantiate( gameObject , GameObject.Find( "Manager" ).transform );
        copy.transform.position = (Vector2)transform.position+new Vector2(Random.Range(-1, 1), Random.Range(-1, 1)).normalized * transform.localScale.x;

        Garden gardenCopy = copy.GetComponent<Garden>();
        gardenCopy.lifeRemaining = plant?plantLifeSpan:gardenerLifeSpan;
        gardenCopy.foodCount = 0;
        gardenCopy.nextAction = 0f;
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        if( tag != "Plant" && collision.collider.tag == "Plant" ) {
            Destroy(collision.gameObject);
            lifeRemaining += foodLifeReplenish;
            foodCount++;
        }
    }
}
