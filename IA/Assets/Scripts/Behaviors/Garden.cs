using System;
using System.Linq;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;
using Random = UnityEngine.Random;

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

    bool targetingFood = false;
    Vector2 targetPosition;
    GameObject targetPlant;

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
                if(Random.value < seedChance) {
                    SpawnNewCopy();
                }
            }
            if (!plant) {
                if (Random.value < childChance*foodCount && foodCount >= requiredFoodForChild) {
                    SpawnNewCopy();
                    foodCount -= requiredFoodForChild;
                }
                if (targetPlant == null) {
                    targetingFood = false;
                }
            }
        }

        if (!plant) {

            if (!targetingFood) {
                var neighbors = particle.GetNeighborDistances(gardenerRadii, "Plant", 1);
                try {
                    if (neighbors.Count > 0) {
                        targetPlant = neighbors.Values.ToList()[Random.Range(0, neighbors.Count - 1)];
                        targetPosition = targetPlant.transform.position;
                        targetingFood = true;
                    } else {
                        particle.addVelocity(((Vector2)transform.position + targetPosition).normalized, speed);
                    }
                } catch ( Exception e ) { }
            } else {
                Vector2 target = (targetPosition - (Vector2)transform.position);
                Debug.DrawLine(transform.position, (Vector2)transform.position + target);
                particle.addVelocity(target.normalized, speed);
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
        copy.transform.position = (Vector2)transform.position + new Vector2(Random.Range(-1, 1), Random.Range(-1, 1)).normalized * (transform.localScale.x*Random.value*5f);
        copy.tag = gameObject.tag;
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
            targetingFood = false;
        }
    }

    private void OnCollisionStay2D(Collision2D collision) {
        if (tag == "Plant" && collision.collider.tag == "Plant") {
            particle.addVelocity((transform.position - collision.transform.position).normalized, 2f);
        }
    }

    private void OnBecameInvisible() {
        Destroy(gameObject);
    }
}
