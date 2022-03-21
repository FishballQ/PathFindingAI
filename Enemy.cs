using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{   
    // define three AI states
    public enum State
    {
        Idle,
        Warn,
        Find
    };

    public Grid grid;
    public FindPath findPath;
    public Transform player;
    public State state = State.Idle;              // default state is idle
    public LayerMask EnemyLayer;                  // enemy layer is using in ray cast collision
    private List<Node> find = new List<Node>();   // chasing path when AI find player              
    private Vector3 lastPlayerPos;                //player last position
    private Animator ani;                         // AI animator
    private Vector3 friend;                       //other AI members

    [Header("Enemy Parameter")]
    public float rotateSpeed = 2;
    public float moveSpeed = 2;
    public float viewDistance = 20;
    public float warningDistance = 18;
    public float safeDistance = 2;
    public float callRaduis = 5;

    void Start()
    {
        ani = this.GetComponent<Animator>();
    }

    void FixedUpdate()
    {
        StateMachine();
    }

     //check if AI chased player
     bool FoundPlayer (Vector3 start, Vector3 end)
    {
        findPath.FindingPath(this.transform.position, player.position);
        find = grid.path;
        // continue going if AI has not reached the path final point
        if (find.Count > 0 && transform.position != find[0].worldPos){
            RotateForward(find[0].worldPos);
            transform.Translate(Vector3.Normalize(find[0].WorldPos - transform.position) * Time.deltaTime * moveSpeed, Space.World);
            return false;
        }

        else return true;   // reached the final point
    }

    // look at target
    private void RotateForward(Vector3 target)
    {
        float angle = Vector3.SignedAngle(transform.forward, Vector3.Normalize((target - transform.position)), transform.up);
        transform.Rotate(new Vector3(0, angle * rotateSpeed * Time.deltaTime, 0), Space.World);
    }

    private void CallFriend()
    {
        GameObject[] friends = GameObject.FindGameObjectsWithTag("Enemy"); 
        // surround all AIs
        foreach (var f in friends)
        {
            float distanceF = Vector3.Distance(transform.position, f.transform.position);
            float distanceP = Vector3.Distance(transform.position, player.position);
            // AIs in the callRaduis range can recieve the calling signal
            if (distanceF < callRaduis)
            {
                Enemy enemyAI = f.GetComponent<Enemy>();
                // if self calling, no further process
                if (enemyAI == this){
                    continue;
                }
                // if called by someone else, change to warn state
                if (enemyAI.state == State.Idle){
                    enemyAI.friend = transform.position; // move to calling position
                    enemyAI.state = State.Warn;
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        Vector3 leftView = Quaternion.AngleAxis(30, Vector3.up) * transform.forward;
        Vector3 rightView = Quaternion.AngleAxis(-30, Vector3.up) * transform.forward;

        Gizmos.DrawRay(transform.position, leftView * 20);  // draw left view range boundary
        Gizmos.DrawRay(transform.position, rightView * 20); // right view range boundary

        //draw the path
        if (find != null)
        {
            foreach (var item in find){
                Gizmos.color = Color.black;
                Gizmos.DrawCube(item.worldPos, Vector3.one * (0.4f - 0.1f));
            }
        }
    }

    private void StateMachine()
    {
        // behaviours in different states
        switch (state)
        {
            case State.Idle:
                 BehaviourWhenIdle();
                 break;
            case State.Warn:
                 BehaviourWhenWarn();
                 break;
            case State.Find:
                 BehaviourWhenFind();
                 break;
        }
        
        // AI view distance
        float distance = Vector3.Distance(transform.position, player.position);
        float angle = Vector3.Dot(transform.forward, Vector3.Normalize(player.position - transform.position));

        if (state != State.Find)
        {
            // distance < vew distance, and angle < 30, player in the view range of the enemy
            if (distance <= viewDistance && angle > 0.87f)
            {
                // check if any wall in the view distance 
                //Debug.Log("find");
                Ray ray = new Ray(transform.position, player.position - transform.position);
                RaycastHit hit;
                Physics.Raycast(ray, out hit, viewDistance, ~EnemyLayer);
                if (hit.collider == null){
                    return;
                }
                // find player
                else if (hit.collider.tag == "Player"){
                    state = State.Find;
                }
            }

            // player is not in the view distance
            else state = State.Idle;
        }
        
        //too far to keep finding player
        else 
        {
            if (distance > viewDistance){
                state = State.Idle;
            }
        }        

    } 

    private void BehaviourWhenIdle()
    {
        ani.SetInteger("State", 0);   // change animation
        transform.Translate(transform.forward * Time.deltaTime *Random.Range(1f,2f)); // move around
        transform.Rotate(new Vector3(0, Random.Range(14, 46) * Time.deltaTime, 0));  
    }

    private void BehaviourWhenWarn()
    {
        ani.SetInteger("State", 1);
        Ray ray = new Ray(transform.position, player.position - transform.position);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Vector3.Distance(player.position, transform.position), ~EnemyLayer))
        {
            // chase player if find them
            if (hit.collider.tag == "Player"){
                RotateForward(player.position);
            }

            // move towards calling position
            else {
                bool found = FoundPlayer(transform.position, friend);
                // reached calling position
                if (found) { 
                    RotateForward(player.position);  // start chasing player
                }
            }
        }

        // move towards calling position as cannot see player
        else {
            bool found = FoundPlayer(transform.position, friend);
            if (found) { 
                    RotateForward(player.position); 
            }
        }
    }

    private void BehaviourWhenFind()
    {
        // distance between player and AI
        float distance = Vector3.Distance(transform.position, player.position);
        // chase player and call other AIs
        if (distance >= warningDistance)
        {
            CallFriend();
            ani.SetInteger("State", 1);
            FoundPlayer(transform.position, player.position);
        }

        // chase player
        else if (distance > safeDistance && distance < warningDistance)
        {
            RotateForward(player.position);
            ani.SetInteger("State", 2);
            FoundPlayer(transform.position, player.position);
        }
        
        // be nervous and go back
        else if (distance <= safeDistance)
        {
            ani.SetInteger("State", 3);
            RotateForward(player.position);
            transform.Translate((transform.position - player.position).normalized * moveSpeed * 0.5f * Time.deltaTime, Space.World);
        }

    }

}
