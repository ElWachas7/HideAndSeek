using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineOfSight))]
public class ChaseEnemy : MonoBehaviour
{
    private LineOfSight _los;
    public Transform target;
    public float stamina;
    public float speed;
    private bool recharging;
    public Transform[] wayPoints;
    private int currentWP = 0;

    private QuestionNode root;
    void Start()
    {
        
        ActionNode idle = new ActionNode(Idle);
        ActionNode patrol = new ActionNode(Patrol);
        ActionNode follow = new ActionNode(Follow);

        QuestionNode isInLos = new QuestionNode(IsInLos, follow, patrol);
        //QuestionNode isRecharging = new QuestionNode(IsRecharging, idle, isInLos);

       // root = isRecharging;
        
    }
    void Update()
    {
        root.Execute();
        /*if (recharging)
        {
            Idle();
        }
        else
        {
            if (_los.CheckRange(target) && _los.CheckAngle(target) && _los.CheckView(target))
                Follow();
            else
                Patrol();
        }*/

    }

    private bool IsInLos() => _los.CheckRange(target) && _los.CheckAngle(target) && _los.CheckView(target);

    private void Idle()
    {
        
    }
    private void Follow()
    {
        var dir = target.position - transform.position;
        transform.position += dir.normalized * speed * Time.deltaTime;
        stamina -= Time.deltaTime;

        //Aplicar Seering Behaviour

    }
    private void Patrol()
    {
        if (Vector3.Distance(transform.position, wayPoints[currentWP].position) <= 0.5f)
        {
            currentWP = (currentWP + 1) % wayPoints.Length;
        }
        var dir = wayPoints[currentWP].position - transform.position;
        transform.position += dir.normalized * speed * Time.deltaTime;
        stamina -= Time.deltaTime;

    }
}
