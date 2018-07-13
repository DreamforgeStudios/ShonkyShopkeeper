using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ShonkyWander : MonoBehaviour {
    //Golem Components
    //private Rigidbody rb;
    private NavMeshAgent agent;
    private Animator animator;

    //Movement variables
    public float wanderTimer;
    private float cooldownTime;
    public float cooldown = 10.0f;
    public float maxDistance = 3.0f;
    public float forceToAdd = 2.0f;
    private Vector3 position;
    private Vector3 destination;
    public bool enableNavmesh = false;
    private bool firstTime = true;
    public bool pickedUp = false;

    //The resource pouch object being held
    public GameObject pouch;

    //If in mine
    public bool inMine = false;
    
    
	// Use this for initialization
	void Start () {
        pouch.SetActive(false);
        agent = GetComponent<NavMeshAgent>();
        //rb = GetComponent<Rigidbody>();
        cooldownTime = Time.time;
        animator = GetComponent<Animator>();
	}

    // Update is called once per frame
    void Update() {
        if (!pickedUp) {
            if (enableNavmesh) {
                Animate();
                if (!firstTime) {
                    wanderTimer = Time.time;
                    if (wanderTimer > cooldownTime) {
                        position = GetNewPosition(firstTime);
                        cooldownTime = Time.time + UnityEngine.Random.Range(7.0f, 10.0f);
                    }

                    GoToNewPosition(position);
                }
                else {
                    GoToWarpNewPosition(GetNewPosition(firstTime));
                    firstTime = false;
                }
            }
        }
    }

    private void GoToWarpNewPosition(Vector3 newPosition) {
        agent.Warp(newPosition);
        destination = newPosition;
    }

    private void GoToNewPosition(Vector3 newPosition) {
        agent.SetDestination(newPosition);
        destination = newPosition;
    }

    private void Animate() {
        if (transform.position == destination) {
            animator.SetBool("Idle", true);
        } else {
            animator.SetBool("Idle", false);
        }
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

    public void HoldPouch() {
        pouch.SetActive(true);
    }

    public void RemovePouch() {
        pouch.SetActive(false);
    }

    public bool IsHoldingPouch() {
        if (pouch.activeSelf)
            return true;
        else
            return false;
    }
}
