using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{

    public float health = 1;

    [Header("Drops")]

    [SerializeField] GameObject dropCollectable;
    [HideInInspector] public int tickets = 0;

    [SerializeField] float heightStart = 7.5f;

    [Header("AI")]

    [SerializeField] NavMeshAgent agent;
    Transform target;

    bool moving;
    float blend;

    [SerializeField] Animator animator;

    float dist;

    [Header("Attack")]

    [Range(0, 1)] [SerializeField] float damage = 0.375f;

    [SerializeField] float inRangeTime = 0.25f;
    float elaspedTime;


    void Start()
    {
        target = GameObject.Find("PLAYER").transform;
    }


    void Update()
    {
        health = (health > -1) ? Mathf.Clamp01(health) : health;

        //AI

        if (target != null && agent != null)
            agent.SetDestination(target.position);

        dist = (target.position - transform.position).magnitude;
        bool inRange = (dist < 2.5f);

        elaspedTime = (inRange) ? elaspedTime + Time.deltaTime : 0;

        if (inRange && elaspedTime > inRangeTime && health > 0)
        {
            target.GetComponent<Health>().Damage(damage);
        }

        //animation blending
        if (agent != null)
            moving = agent.velocity.magnitude > 0.25f;
        blend = Mathf.Lerp(blend, moving ? 1 : 0, 0.15f);

        animator.SetFloat("Blend", blend);

        //health
        if (health == 0)
        {
            health = -1; //deb
            transform.SetParent(null);

            Destroy(agent);
            animator.Play("Died");
            StartCoroutine(RemoveRemains());

            //Drops

            int luckyTk = (Random.Range(0, 7) == 0) ? 1 : 0;
            for (int i = 0; i < Random.Range(0, 2) + luckyTk + luckyTk; i++) // 50/50 + 2 luckyTks
                StartCoroutine(Drop(DropCollectable.DropType.Tks));

            StartCoroutine(Drop(DropCollectable.DropType.Supplize, Random.Range(0, 4) == 0)); //ammunation

            for (int i = 0; i < tickets; i++)
            {
                StartCoroutine(Drop(DropCollectable.DropType.Tickets));
                StartCoroutine(Drop(DropCollectable.DropType.Tks, Random.Range(0, 2) == 0)); //more chance for tk
            }
        }

    }

    IEnumerator RemoveRemains()
    {
        yield return new WaitForSeconds(1);

        Destroy(gameObject);
    }


    public IEnumerator Drop(DropCollectable.DropType type, bool chance = true)
    {
        if (!chance) yield break;

        GameObject drop = Instantiate(dropCollectable);
        Transform ico = drop.transform.GetChild(0);

        ico.localScale = Vector3.zero;
        LeanTween.scale(ico.gameObject, Vector3.one, 1f).setEaseOutBack();

        drop.GetComponent<DropCollectable>().type = type;

        drop.transform.position = transform.position;
        //drop.transform.GetComponent<Rigidbody>().AddForce(Vector3.left * Random.Range(-spread, spread + 1), ForceMode.Impulse);

        yield return new WaitForEndOfFrame();
        drop.SetActive(true);

        drop.GetComponent<Rigidbody>().AddForce(transform.up * heightStart + transform.right * Random.Range(-heightStart, heightStart) / 4, ForceMode.VelocityChange);
    }
}
