using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;

public class SentryEnemy : NavigationAgent {

    //Player Reference
    Player player;

    //Movement Variables
    public float moveSpeed = 10.0f;
    public float minDistance = 0.1f;

    //FSM Variables
    public int newState = 0;
    private int currentState = 0;
    private int hideIndex = 25;

    //AudioSource Var's
    public AudioSource audioSource;
    public AudioClip HeavyMachineGun;

    //Color Var's
    Renderer CapRend;

    //Stopwatch Timer (for seconds)
    Stopwatch timer = new Stopwatch();


    // Use this for initialization
    void Start()
    {
        //Find waypoint graph
        graphNodes = GameObject.FindGameObjectWithTag("waypoint graph").GetComponent<WaypointGraph>();
        //Initial node index to move to
        currentPath.Add(currentNodeIndex);
        //Establish reference to player game object
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();

        //Get Material Renderer and Gameobect
        GameObject.FindGameObjectsWithTag("Sentry");
        CapRend = GetComponent<Renderer>();
    }

    //Enemy States in Enum format
     enum EnemyStates
     {
        //Based off numbers 0-3
        Patrol,
        Idle,
        Attack,
        
     };

    void Update ()
    {
        Move();
        currentState = newState;

        switch (currentState)
        {
          //Roam
          case 0:
                //Roam();
                break;
         //Hide
         case 1:
                Idle();
                break;
         //Attack
         case 2:
                Attack();
                break;
        }

        
    }



    //Move Enemy
    private void Move()
    {

        if (currentPath.Count > 0)
        {

            //Move towards next node in path
            transform.position = Vector3.MoveTowards(transform.position, graphNodes.graphNodes[currentPath[currentPathIndex]].transform.position, moveSpeed * Time.deltaTime);

            //Increase path index
            if (Vector3.Distance(transform.position, graphNodes.graphNodes[currentPath[currentPathIndex]].transform.position) <= minDistance) {

                if (currentPathIndex < currentPath.Count - 1)
                    currentPathIndex++;
            }

            currentNodeIndex = graphNodes.graphNodes[currentPath[currentPathIndex]].GetComponent<LinkedNodes>().index;   //Store current node index
        }
    }

    //FSM Behaviour - Roam - Randomly select nodes to travel to using Greedy Search Algorithm
    private void Roam()
    {
        if (Vector3.Distance(transform.position, graphNodes.graphNodes[currentPath[currentPath.Count - 1]].transform.position) <= minDistance)
        {
            //Randomly select new waypoint 
            int randomNode = Random.Range(0, graphNodes.graphNodes.Length);

            //Reset current path and add first node - needs to be doen here because of reursive function of greedy
            currentPath.Clear();
            greedyPaintList.Clear();
            currentPathIndex = 0;
            currentPath.Add(currentNodeIndex);

            //Greedy Search - navigate towards randomNode
            currentPath = GreedySearch(currentPath[currentPathIndex], randomNode, currentPath);

            //Reverse path and remove final (i.e intial) position
            currentPath.Reverse();
            currentPath.RemoveAt(currentPath.Count - 1);
        }
    }

    //FSM Behaviour - Move towards hide location using A* Search Algorithm
    private void Idle()
    {
        //Move towards next node in path
        transform.position = Vector3.MoveTowards(transform.position, graphNodes.graphNodes[currentPath[currentPathIndex]].transform.position, moveSpeed * Time.deltaTime);

        //Increase path index
        if (Vector3.Distance(transform.position, graphNodes.graphNodes[currentPath[currentPathIndex]].transform.position) <= minDistance)
        {

            if (currentPathIndex < currentPath.Count - 1)
                currentPathIndex++;
        }

        currentNodeIndex = graphNodes.graphNodes[currentPath[currentPathIndex]].GetComponent<LinkedNodes>().index;   //Store current node index
    }
    
    //FSM Behaviour - Move towards node closest to player using A* Search Algorithm
    private void Attack()
    {
        StartCoroutine(Timer());
        Timer();
    }

    IEnumerator Timer()
    {
        yield return new WaitForSeconds(5);

        moveSpeed = 5f;
        CapRend.material.SetColor("_Color", Color.yellow);


        if (Vector3.Distance(transform.position, graphNodes.graphNodes[player.currentNodeIndex].transform.position)
        > minDistance && currentPath[currentPath.Count - 1] != player.currentNodeIndex)
        {
            currentPath = AStarSearch(currentPath[currentPathIndex], player.currentNodeIndex);
            currentPathIndex = 0;
        }

        StartCoroutine(Timer());
    }
}
