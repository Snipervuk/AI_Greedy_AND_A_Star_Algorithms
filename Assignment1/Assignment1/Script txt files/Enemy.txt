using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : NavigationAgent {

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



    // Use this for initialization
    void Start()
    {
        //Find waypoint graph
        graphNodes = GameObject.FindGameObjectWithTag("waypoint graph").GetComponent<WaypointGraph>();
        //Initial node index to move to
        currentPath.Add(currentNodeIndex);
        //Establish reference to player game object
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();

        //Getting Renderer and gameobject
        GameObject.FindGameObjectsWithTag("Enemy");
        CapRend = GetComponent<Renderer>();

    }

    //Enemy States in Enum format
     //enum EnemyStates
     //{
        //Based off numbers 0-2
     //   Patrol,
     //   Hide,
     //   Attack,
     //};


    //int[,] DFA1Regular = 
    //{
                          /* 0     1      2*/
    //  /* SO Roam */ {true, Roam, Attack, Roam}, //true = 0, 1, 0
    //  /* S1 Attack*/{true, Attack, Attack, Attack}, //true = 1, 1, 1
    //  /* S2 Hide*/  {false,null, null, null}, //Hide is not supported by this agent so would be null
    //};

    //int[,] DFA2Cowardy =
    //{
                         /* 0      1    2*/
    //  /* SO Roam */ {true, Roam, Hide, Roam}, //true = 0, 2, 0
    //  /* S1 Attack*/{false,null, null, null}, //Attack is not supported by this agent so would be null
    //  /* S2 Hide*/  {true, Hide, Hide, Hide}, //true = 2, 2, 2
    //};

    //int[,] DFA3Sentry =
    //{
                         /* 0      1      2*/
    //  /* SO Roam */ {true, null, Attack, null}, //true = 3, 1, 3 (would have to be an idle state so that would have to be S4) 
    //  /* S1 Attack*/{true, Attack, null, Attack}, //true = 1, 3, 1
    //  /* S2 Hide*/  {false, null, null, null}, //Hide is not supported by this agent so would be null
    //};

    //int[,] DFARoamer =
    //{
    //                      /* 1     2      3*/
    //  /* SO Roam */ {true,  Roam, Roam, Roam}, //true = [0, 0, 0] 
    //  /* S1 Attack*/{false, null, null, null}, //Attack is not supported by thsi agent so would be null
    //  /* S2 Hide*/  {false, null, null, null}, //Hide is not supported by thsi agent so would be null
    //};



    void Update ()
    {
        Move();
        currentState = newState;

        switch (currentState)
        {
          //Roam
          case 0:
                CapRend.material.SetColor("_Color", Color.green);
                moveSpeed = 10f;
                Roam();
                break;
         //Hide
         case 1:
                Hide();
                break;
         //Attack
         case 2:
                Attack();
                CapRend.material.SetColor("_Color", Color.red);
                moveSpeed = 15f;
                break;
        }    
    }

    //Move Enemy
    private void Move() {

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
    private void Hide()
    {
        currentPath = AStarSearch(currentPath[currentPathIndex], currentPathIndex + hideIndex); //51 was here instead of hideIndex
        currentPathIndex = 0;

    }
    //FSM Behaviour - Move towards node closest to player using A* Search Algorithm
    private void Attack()
    {
        if (Vector3.Distance(transform.position,graphNodes.graphNodes[player.currentNodeIndex].transform.position) 
            > minDistance && currentPath[currentPath.Count- 1] != player.currentNodeIndex)
        {
            currentPath = AStarSearch(currentPath[currentPathIndex], player.currentNodeIndex);
            currentPathIndex = 0;
        }
    }
}
