using UnityEngine;

public class AIHealthSystem : MonoBehaviour
{
    public int health = 100;
    public Transform player;
    public GameObject parent;
    public Animator anim;
    public bool killed;
    public bool onFire;
    public GameObject fire;
    public GameObject[] loot;
    public GameObject[] damagePanels;
    public GameObject damagePopup;
    private bool onetime;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;

    }
    void Update()
    {
        if (onFire)
        {
            if (!onetime)
            {
                onetime = true;
            }
            fire.SetActive(true);
            health--;
            anim.SetBool("onfire", true);
            if (health <= 0) Die();
        }
        else if (health <= 0)
        {
            Die();
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Fire"))
        {

            TakeDamage(5);
            
        }
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        Instantiate(damagePopup, damagePanels[Random.Range(0, damagePanels.Length)].transform);
        if (health <= 0) Die();
    }

    private void Die()
    {
        anim.SetBool("onfire", true);
        fire.SetActive(true);
        anim.SetBool("falltodeath", true);
        DisableComponents();
        if (!killed)
        {
            player.GetComponent<PlayerHealth>().totalkills++;
            killed = true;
        }
        Invoke("DeactivateParent", 10);
        Destroy(transform.parent.gameObject,5);
    }

    private void DisableComponents()
    {
        var lineOfSight = parent.GetComponent<LineOfSight>();
        if (lineOfSight != null)
        {
            lineOfSight.CancelInvoke();
            lineOfSight.enabled = false;
            lineOfSight.gun.GetComponent<Weapons>().enabled = false;
        }
        parent.GetComponent<CrowdBot>().enabled = true;
    }

    private void DeactivateParent()
    {
        loot[Random.Range(0, loot.Length)].SetActive(true);
        parent.SetActive(false);
    }
}
