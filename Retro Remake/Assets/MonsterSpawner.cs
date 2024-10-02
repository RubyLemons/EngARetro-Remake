using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterSpawner : MonoBehaviour
{
    [SerializeField] Freelook freelook;

    [Header("Spawner")]

    [SerializeField] GameObject monster;
    [SerializeField] Transform locationContainer;
    List<Transform> locations = new List<Transform>();
    [Range(0, 1)] [SerializeField] float threshold = 0.375f;

    int currentNumber;
    bool activeWave;


    float elaspedTime;
    public float delayTime = 2.5f;

    void Start()
    {
        //get locations
        foreach (Transform v in locationContainer)
            locations.Add(v);
    }

    void Update()
    {
        elaspedTime += Time.deltaTime;
        if (elaspedTime < delayTime) return;

        currentNumber = transform.childCount;

        bool lowMonsters = (currentNumber <= Mathf.FloorToInt(Token.leastMonsters));

        if (lowMonsters)
        {
            if (!activeWave)
            {
                activeWave = true;

                Token.numberMonsters = 
                    (Token.numberMonsters <= 0) ? Random.Range(2, 3) : Token.numberMonsters + Random.Range(1, 2); //add one random 1 or 2 : if none set random default
                Token.leastMonsters = (Token.leastMonsters <= 0) ? Random.Range(1, 2 + 1) : Token.leastMonsters + 0.25f; //add one : if none set random default

                Token.monsterSpeed += 0.175f;
                Token.monsterSpeed = Mathf.Clamp(Token.monsterSpeed, 0, 1.75f);

                //print(Token.numberMonsters + " " + Token.monsterSpeed +" "+ Token.leastMonsters); //debug
                if (Token.informMonsters)
                    Inform.instance.Alert($"<color=#AAFFAA>{Token.numberMonsters}</color>, {Token.monsterSpeed}, {Token.leastMonsters}", 15); //debug gui

                SpawnMonsters();
            }
            else
            {
                activeWave = false;
            }
        }
    }

    void SpawnMonsters()
    {
        if (currentNumber > 99) return;

        Token.round++;
        if (Token.round > 16 && !Token.informMonsters)
            Inform.instance.Alert("Mark of the <color=#FF0000>Beast!</color>");

        for (int i = 0; i < Token.numberMonsters; i++)
        {
            Transform point = locations[Random.Range(0, locations.Count)];

            Transform enemy = Instantiate(monster, transform).transform;
            enemy.gameObject.SetActive(false);

            enemy.GetComponent<UnityEngine.AI.NavMeshAgent>().speed += Token.monsterSpeed; //set speed

            StartCoroutine(SpawnWait(point, enemy));
        }

        //after adding new group of ememies, choose one enemy from list, and give them ticket drops
        Enemy randomEnemy = transform.GetChild(Random.Range(0, transform.childCount)).GetComponent<Enemy>();
        randomEnemy.tickets++;
    }

    IEnumerator SpawnWait(Transform point, Transform enemy)
    {
        while (true)
        {
            yield return new WaitForSeconds(0.1f);

            if (currentNumber > 99) yield break;

            Vector3 dir = (point.position - freelook.cam.transform.position);
            float dotProduct = Vector3.Dot(dir.normalized, freelook.cam.transform.forward);

            bool lookingAt = (dotProduct > threshold);

            if (!lookingAt)
            {
                enemy.gameObject.SetActive(true);
                enemy.transform.position = point.position;
                yield break;
            }
        }
    }
}
