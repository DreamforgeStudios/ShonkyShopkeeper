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
    public float maxDistance = 3.0f;
    public float forceToAdd = 5.0f;
    private Vector3 position;
    public bool enableNavmesh = false;
    private bool firstTime = true;
    
    
	// Use this for initialization
	void Start () {
        agent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();
        cooldownTime = Time.time;
	}
	
	// Update is called once per frame
	void Update () {
        if (enableNavmesh) { 
            if (!firstTime) {
                wanderTimer = Time.time;
                if (wanderTimer > cooldownTime) {
                    position = GetNewPosition(firstTime);
                    cooldownTime = Time.time + UnityEngine.Random.Range(3.0f, 7.0f);
                }

                GoToNewPosition(position);
            } else {
                GoToWarpNewPosition(GetNewPosition(firstTime));
                firstTime = false;
            }
        }
    }

    private void GoToWarpNewPosition(Vector3 newPosition) {
        agent.Warp(newPosition);
    }

    private void GoToNewPosition(Vector3 newPosition) {
        agent.SetDestination(newPosition);
    }

    private Vector3 GetNewPosition(bool firstTime) {
        if (!firstTime) {
            Vector3 newPosition = UnityEngine.Random.insideUnitSphere * maxDistance;
            newPosition += transform.position;
            newPosition.y = -1.57f;
            NavMeshHit hit;
            NavMesh.SamplePosition(newPosition, out hit, maxDistance, 1);
            Vector3 position = hit.position;
            Debug.DrawLine(transform.position, position);
            return position;
        } else {
            return transform.position;
        }

    }
}
