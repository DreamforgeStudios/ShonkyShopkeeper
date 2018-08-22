using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class ShonkyWander : MonoBehaviour {
    //Golem Components
    private Rigidbody rb;
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
    private bool ballInteraction = false;
    private float interactionLimit = 6.0f;
    private float timeLimit;

    //The resource pouch object being held
    public GameObject pouch;
    
    //Ball to interact with
    private GameObject ball;
	// Use this for initialization
    private void Awake()
    {
	    Debug.Log("Calling start function in golem");
        pouch.SetActive(false);
        agent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();
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
        if (ballInteraction)
            KickBall();
    }

    private void CheckForPickup()
    {
        if (pickedUp)
            ballInteraction = false;
    }
    private void GoToWarpNewPosition(Vector3 newPosition) {
        agent.Warp(newPosition);
        destination = newPosition;
    }

    private void GoToNewPosition(Vector3 newPosition) {
        if (!ballInteraction)
        {
            agent.SetDestination(newPosition);
            destination = newPosition;
        }
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
        animator.SetBool("WalkPouch", true);
        Debug.Log("pouch is active" + pouch.activeSelf);
    }

    public void RemovePouch() {
        pouch.SetActive(false);
        animator.SetBool("WalkPouch", false);
    }

    public bool IsHoldingPouch() {
        if (pouch.activeSelf)
            return true;
        else
            return false;
    }

    public void FloatToPen()
    {
        PickUpAnimation(false);
        StartCoroutine(Float());
    }
    
    IEnumerator Float()
    {
        if (CalculateIfNeedGuidance())
        {
            transform.DOMove(CalculateRandomPenLoc(), 0.6f, false);
        }
        else
        {
            
        }

        yield return new WaitForSeconds(1f);
        Debug.Log("Reenabling agent");
        pickedUp = false;
        agent.enabled = true;
        animator.SetBool("Drop",false);
    }

    private bool CalculateIfNeedGuidance()
    {
        if (transform.position.x <= -5f || transform.position.x >= 4f)
        {
            return true;
        }

        return false;
    }
    
    private Vector3 CalculateRandomPenLoc()
    {
        float xPos = GetXPos();
        float yPos = -1.78f;
        float zPos = GetZPos();
        Vector3 returnPos = new Vector3(xPos,yPos,zPos);
        Debug.Log(String.Format("Current pos is {0} and return position is {1}",transform.position,returnPos));
        return returnPos;
    }

    private float GetXPos()
    {
        if (transform.position.x >= -5f && transform.position.x <= 4f)
        {
            return transform.position.x;
        }
        if (transform.position.x < -5f)
        {
            return Random.Range(-5f, -4f);
        }
        return Random.Range(3.5f, 4f);
    }

    private float GetZPos()
    {
        if (transform.position.z >= -5.45f && transform.position.z <= -1.95f)
        {
            return transform.position.z;
        }
        if (transform.position.z < -5.45f)
        {
            return Random.Range(-5.45f, -5.3f);
        }
        return Random.Range(-5.45f, -4.95f);
    }

    public void PickUpAnimation(bool activate)
    {
        animator.SetBool("Pickup", activate);
        /*
        if (activate)
        {
            rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        }
        else
        {
            rb.constraints = RigidbodyConstraints.None;
        }
        */
    }

    public void InteractWithBall(GameObject ballObj)
    {
        Debug.Log("Starting interaction on golem");
        ballInteraction = true;
        ball = ballObj;
        timeLimit = Time.time + interactionLimit;
    }

    private void KickBall()
    {
        CheckForPickup();
        //Debug.Log(String.Format("Time limit is {0} and current time is {1}",timeLimit,Time.time));
        if (ballInteraction && Time.time < timeLimit)
        {
            animator.SetBool("Idle", false);
            //transform.DOMove(ball.transform.position, 4.0f, false);
            //transform.position = Vector3.MoveTowards(transform.position, ball.transform.position, 2.6f * Time.deltaTime);
            FollowTargetWithRotation(ball.transform,0f,12f);
            transform.LookAt(ball.transform);
            
        }
        else
        {
            Debug.Log("Ending interaction");
            animator.SetBool("Idle", true);
            ballInteraction = false;
            GoToNewPosition(GetNewPosition(firstTime));
        }
    }

    public bool TrinketInteraction()
    {
        return ballInteraction;
    }

    private void FollowTargetWithRotation(Transform target, float distanceToStop, float speed)
    {
        //Debug.Log(Vector3.Distance(transform.position,target.position));
        if(Vector3.Distance(transform.position, target.position) > distanceToStop)
        {
            transform.LookAt(target);
            Vector3 direction = (target.transform.position - transform.position).normalized;
            rb.MovePosition(transform.position + direction * 5f * Time.deltaTime);
            //rb.MovePosition(target.transform.position);
        }
    }
}
