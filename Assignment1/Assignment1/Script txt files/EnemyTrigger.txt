using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTrigger : MonoBehaviour
{
    //Prebuild Enemy Vars
    public Enemy[] enemies;
    public TriggerState Enter;
    public TriggerState Exit;

    //FSM Vars
    int currentState = 0;

    //string NewState;
    //int DFA[][]

    void Start()
    {
        GameObject.FindGameObjectWithTag("Player");
        currentState = 0;

    }

    void Update()
    {
      
    }

    private void ChangeEnemyStates(int state)
    {
        foreach (Enemy enemy in enemies)
        {
            enemy.newState = state;
        }
    }

    //FSM Enumerator for Enemy
    public enum TriggerState
    {
        //Based off numbers 0-2
        Patrol,
        Hide,
        Attack
    }


    void OnTriggerEnter(Collider Other)
    {
        if (Other.tag == "Player")
        {
            switch (Enter)
            {
                case TriggerState.Patrol:
                    ChangeEnemyStates(0);
                    break;
                case TriggerState.Hide:
                    ChangeEnemyStates(1);
                    break;
                case TriggerState.Attack:
                    ChangeEnemyStates(2);
                    break;
            }
        }
    }

    void OnTriggerExit(Collider Other)
    {
        if (Other.tag == "Player")
        {
            switch (Exit)
            {
                case TriggerState.Patrol:
                    ChangeEnemyStates(0);
                    break;
                case TriggerState.Hide:
                    ChangeEnemyStates(1);
                    break;
                case TriggerState.Attack:
                    ChangeEnemyStates(2);
                    break;
            }
        }
    }

}