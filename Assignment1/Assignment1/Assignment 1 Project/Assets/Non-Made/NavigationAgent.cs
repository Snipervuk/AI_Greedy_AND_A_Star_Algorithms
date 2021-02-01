using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class NavigationAgent : MonoBehaviour
{

    //Navigation Variables
    public WaypointGraph graphNodes;
    public List<int> openList = new List<int>();
    public List<int> closedList = new List<int>();
    public List<int> currentPath = new List<int>();
    public List<int> greedyPaintList = new List<int>();
    public int currentPathIndex = 0;
    public int currentNodeIndex = 0;

    //DIY
    //public int DummyVar = 10;


    public Dictionary<int, int> cameFrom = new Dictionary<int, int>();

    // Use this for initialization
    void Start()
    {
        //Find waypoint graph
        graphNodes = GameObject.FindGameObjectWithTag("waypoint graph").GetComponent<WaypointGraph>();

        //Initial node index to move to
        currentPath.Add(currentNodeIndex);
    }

    public List<int> ReconstructPath(Dictionary<int, int> CF, int current)
    {
        List<int> finalPath = new List<int>();

        finalPath.Add(current);

        while (CF.ContainsKey(current))
        {
            current = CF[current];
            finalPath.Add(current);
        }

        finalPath.Reverse();

        return finalPath;
    }

    //Declaring OpenList Functions/Vars
    public int bestOpenListFScore(int start, int goal)
    {

        int bestIndex = 0;

        for (int i = 0; i < openList.Count; i++)
        {

            if ((Heuristic(openList[i], start) + Heuristic(openList[i], goal)) <
                (Heuristic(openList[bestIndex], start) + Heuristic(openList[bestIndex], goal)))
            {
                bestIndex = i;
            }
        }

        int bestNode = openList[bestIndex];
        return bestNode;
    }


    //A-Star Search
    public List<int> AStarSearch(int start, int goal)
    {

        //Clear everything at the start
        openList.Clear();
        closedList.Clear();
        cameFrom.Clear();

        //Begin
        openList.Add(start);

        float gScore = 0;
        float fScore = gScore + Heuristic(start, goal);

        while (openList.Count > 0)
        {
            //Find the Node in the open list with the lowest Fscore Value
            int currentNode = bestOpenListFScore(start, goal);

            //Found the end, recontruct entire path and return
            if (currentNode == goal)
            {
                return ReconstructPath(cameFrom, currentNode);
            }

            openList.Remove(currentNode);
            closedList.Add(currentNode);

            //For each of the nodes connected to the current node
            for (int i = 0; i < graphNodes.graphNodes[currentNode].GetComponent<LinkedNodes>().linkedNodesIndex.Length; i++)
            {
                int thisNeighbourNode = graphNodes.graphNodes[currentNode].GetComponent<LinkedNodes>().linkedNodesIndex[i];

                //Ignore if neighbour node is disabled
                if (!closedList.Contains(thisNeighbourNode))
                {

                    //Distance from current to the nextNode
                    float tentativeGScore = Heuristic(start, currentNode) + Heuristic(currentNode, thisNeighbourNode);

                    //Check to see if in openlist or new Gscore is more sensible
                    if (!openList.Contains(thisNeighbourNode) || tentativeGScore < gScore)
                        openList.Add(thisNeighbourNode);

                    //Add to Dictionary this neighbour came this parent
                    if (!cameFrom.ContainsKey(thisNeighbourNode)) cameFrom.Add(thisNeighbourNode, currentNode);

                    gScore = tentativeGScore;
                    fScore = Heuristic(start, thisNeighbourNode) + Heuristic(thisNeighbourNode, goal);
                }
            }
        }
        return null;
    }

    public float Heuristic(int a, int b)
    {
        return Vector3.Distance(graphNodes.graphNodes[a].transform.position, graphNodes.graphNodes[b].transform.position);
    }


    public class GreedyChildren : IComparable<GreedyChildren>
    {
        public int childID { get; set; }
        public float childHScore { get; set; }

        public GreedyChildren(int childrenID, float childrenHScore)
        {
            this.childID = childrenID;
            this.childHScore = childrenHScore;
        }

        public int CompareTo(GreedyChildren other)
        {
            return this.childHScore.CompareTo(other.childHScore);
        }
    }


    //Greedy Search
    public List<int> GreedySearch(int currentNode, int goal, List<int> path)
    {
        //if (!greedyPaintList.Contains(currentNode))
            //greedyPaintList.Add(currentNode);

        //Make a Custom list that stores the current node's childern nodes and H scores, sorted them by ascending order of Heuristic
        List<GreedyChildren> thisNodesChildern = new List<GreedyChildren>();

        for (int i = 0; i < graphNodes.graphNodes[currentNode].GetComponent<LinkedNodes>().linkedNodesIndex.Length; i++)
        {
           thisNodesChildern.Add(new GreedyChildren(graphNodes.graphNodes[currentNode].GetComponent<LinkedNodes>().linkedNodesIndex[i],
               Heuristic(graphNodes.graphNodes[currentNode].GetComponent<LinkedNodes>().linkedNodesIndex[i], goal)));
        }
      
        
        thisNodesChildern.Sort();

        //Loop through the childern
        for (int i = 0; (i <= thisNodesChildern.Count - 1); i++)
        {
            //If the child has not been painted
            if (!greedyPaintList.Contains(currentNode))
                greedyPaintList.Add(currentNode);
            {

                //Base case we reach the goal node
                if (thisNodesChildern[i].childID == goal)
                {
                    path.Add(goal);
                    return path;
                }

                //Rescurse the search on the child node
                path = GreedySearch(thisNodesChildern[i].childID, goal, path);
                
                //Have path now unwind stack
                if (path.Count >= 2)
                {
                    path.Add(currentNode);
                    return path;
                }
                
            }
        }
        return path;
    }
}
