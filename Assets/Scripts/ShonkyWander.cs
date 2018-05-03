using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ShonkyWander : MonoBehaviour {

    private Rigidbody rb;
    private NavMeshAgent agent;
    public float wanderTimer;
    private float cooldownTime;
    public float cooldown = 10.0f;
    public float maxDistance = 5.0f;
    public float forceToAdd = 5.0f;
    private Vector3 position;
    
	// Use this for initialization
	void Start () {
        agent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();
        cooldownTime = Time.time;
	}
	
	// Update is called once per frame
	void Update () {
        wanderTimer = Time.time;
        if (wanderTimer > cooldownTime) {
            position = GetNewPosition();
            cooldownTime = Time.time + cooldown;
        }

        GoToNewPosition(position);

    }

    private void GoToNewPosition(Vector3 newPosition) {
        agent.destination = newPosition;
    }

    private Vector3 GetNewPosition() {
        //Debug.Log("Getting new position");
        Vector3 newPosition = UnityEngine.Random.insideUnitSphere * maxDistance;
        newPosition += transform.position;
        newPosition.y = 1;
        NavMeshHit hit;
        NavMesh.SamplePosition(newPosition, out hit, maxDistance, 1);
        Vector3 position = hit.position;
        Debug.DrawLine(transform.position, position);
        return position;

    }
}
