using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTrigger2 : MonoBehaviour
{
    //Prebuild Enemy Vars
    public Enemy[] enemies;
    public TriggerState Enter;
    public TriggerState Exit;

    public AudioSource audioClip;

    //FSM Vars
    int currentState = 0;

    void Start()
    {
        GameObject.FindGameObjectWithTag("Player");
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
            audioClip.Play(); 
           
        }
    }
}