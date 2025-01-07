using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Node
{
    public Action perform;
    protected NodeState state;
    public enum NodeState
    {
        RUNNING,
        SUCCESS,
        FAILURE
    }

    public NodeState State
    {
        get { return state; }
    }

    public virtual NodeState operation()
    {
        return NodeState.FAILURE;
    }

    public virtual NodeState execution(Func<NodeState> act)
    {
        return NodeState.FAILURE;
    }
}

public class Selector : Node
{
    private List<Node> nodes = new List<Node>();
    public override NodeState operation()
    {
        
        foreach (Node node in nodes)
        {
            switch (node.operation())
            {
                case NodeState.FAILURE:
                    continue;
                case NodeState.RUNNING:
                    state = NodeState.RUNNING;
                    return state;
                case NodeState.SUCCESS:
                    state = NodeState.SUCCESS;
                    return state;
                default:
                    continue;
            }
        }
        state = NodeState.FAILURE;
        return state;
    }
    public void add(Node child)
    {
        nodes.Add(child);
    }
}

public class Sequence : Node
{
    private List<Node> nodes = new List<Node>();
    public override NodeState operation()
    {
        foreach (Node node in nodes)
        {
            switch (node.operation())
            {
                case NodeState.FAILURE:
                    state = NodeState.FAILURE;
                    return state;
                case NodeState.RUNNING:
                    state = NodeState.RUNNING;
                    return state;
                case NodeState.SUCCESS:
                    continue;
                default:
                    state = NodeState.SUCCESS;
                    return state;
            }
        }
        state = NodeState.FAILURE;
        return state;
    }

    public void add(Node child)
    {
        nodes.Add(child);
    }
}

public class Execution : Node
{
    public int stateNum;
    Func<NodeState> action;
    public Execution(Func<NodeState> func)
    {
        //Debug.Log("execution");
        action = func;
    }
    
    public override NodeState operation()
    {
        return execution(action);
    }
    public override NodeState execution(Func<NodeState> action)
    {
        return action();
    }
}