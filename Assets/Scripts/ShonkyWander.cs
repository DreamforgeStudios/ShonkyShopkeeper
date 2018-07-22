using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

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
    
    
	// Use this for initialization
    private void Awake()
    {
	    Debug.Log("Calling start function in golem");
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
        Debug.Log("pouch is active" + pouch.activeSelf);
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

    public void FloatToPen()
    {
        StartCoroutine(Float());
    }
    
    IEnumerator Float()
    {
        transform.DOMove(CalculateRandomPenLoc(), 1f, false);
        yield return new WaitForSeconds(1.5f);
        Debug.Log("Reenabling agent");
        pickedUp = false;
        agent.enabled = true;
    }

    private Vector3 CalculateRandomPenLoc()
    {
        float XPos = Random.Range(-5f, 4.5f);
        float YPos = -1.78f;
        float ZPos = Random.Range(-5.45f, -1.95f);
        Vector3 returnPos = new Vector3(XPos,YPos,ZPos);
        return returnPos;
    }

}
