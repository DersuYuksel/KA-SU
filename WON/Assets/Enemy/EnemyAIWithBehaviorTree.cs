using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class Node
{
    public abstract bool Evaluate(); // Her d���m, ba�ar�l� olup olmad���n� kontrol eder
}

public class Selector : Node
{
    private List<Node> _nodes;

    public Selector(List<Node> nodes)
    {
        _nodes = nodes;
    }

    public override bool Evaluate()
    {
        foreach (var node in _nodes)
        {
            if (node.Evaluate())
            {
                return true; // E�er bir d���m ba�ar�l� olursa true d�ner
            }
        }
        return false; // Hi�bir d���m ba�ar�l� de�ilse false d�ner
    }
}

public class Sequence : Node
{
    private List<Node> _nodes;

    public Sequence(List<Node> nodes)
    {
        _nodes = nodes;
    }

    public override bool Evaluate()
    {
        foreach (var node in _nodes)
        {
            if (!node.Evaluate())
            {
                return false; // E�er bir d���m ba�ar�s�z olursa false d�ner
            }
        }
        return true; // T�m d���mler ba�ar�l�ysa true d�ner
    }
}

public class ConditionNode : Node
{
    private Func<bool> _condition;

    public ConditionNode(Func<bool> condition)
    {
        _condition = condition;
    }

    public override bool Evaluate()
    {
        return _condition.Invoke(); // Ko�ulu de�erlendir ve sonucu d�nd�r
    }
}

public class ActionNode : Node
{
    private Action _action;

    public ActionNode(Action action)
    {
        _action = action;
    }

    public override bool Evaluate()
    {
        _action.Invoke(); // Aksiyonu ger�ekle�tir
        return true; // Ba�ar�l� oldu�u varsay�l�r
    }
}

public class EnemyAIWithBehaviorTree : MonoBehaviour
{
    public Transform player;
    public float chaseRange = 10f;
    public float attackRange = 2f;
    public Transform[] patrolPoints;
    public float patrolSpeed = 2f;
    public float chaseSpeed = 4f;
    private Animator anim;

    private NavMeshAgent agent;
    private int currentPatrolIndex = 0;
    private Node behaviorTree;

    void Start()
    {
        anim = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();

        // Ko�ullar
        ConditionNode isPlayerInAttackRange = new ConditionNode(() => Vector3.Distance(player.position, transform.position) <= attackRange);
        ConditionNode isPlayerInChaseRange = new ConditionNode(() => Vector3.Distance(player.position, transform.position) <= chaseRange);

        // Aksiyonlar
        ActionNode attackPlayer = new ActionNode(() =>
        {
            agent.isStopped = true;
            Debug.Log("Sald�r!");
            anim.SetFloat("Z", 1);
        });

        ActionNode chasePlayer = new ActionNode(() =>
        {
            agent.isStopped = false;
            agent.speed = chaseSpeed;
            agent.SetDestination(player.position);
            Debug.Log("Takip ediliyor!");
            anim.SetFloat("Z", 0);
        });

        ActionNode patrol = new ActionNode(() =>
        {
            agent.isStopped = false;
            agent.speed = patrolSpeed;
            if (agent.remainingDistance < 0.5f && !agent.pathPending)
            {
                currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
                agent.SetDestination(patrolPoints[currentPatrolIndex].position);
            }
            Debug.Log("Devriye geziyor...");
            anim.SetFloat("Z", 0);

        });

        // Behavior Tree Yap�s�
        Sequence attackSequence = new Sequence(new List<Node> { isPlayerInAttackRange, attackPlayer });
        Sequence chaseSequence = new Sequence(new List<Node> { isPlayerInChaseRange, chasePlayer });
        Selector patrolOrChase = new Selector(new List<Node> { chaseSequence, patrol });

        // En �st d�zey behavior tree (Selector t�m dallar� deneyecek)
        behaviorTree = new Selector(new List<Node> { attackSequence, patrolOrChase });
    }

    void Update()
    {
        behaviorTree.Evaluate();
    }
}
