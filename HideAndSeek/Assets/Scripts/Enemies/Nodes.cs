using System;
using System.Collections.Generic;
using UnityEngine;
using static Nodes;
using System.Collections.Generic;

public class Nodes
{
    public enum NodeState
    {
        Success,
        Failure,
        Running
    }
}

public class ActionNode : Nodes, ITreeNode
{
    private Func<NodeState> action;

    public ActionNode(Func<NodeState> action)
    {
        this.action = action;
    }
    public NodeState Execute()
    {
        return action != null ? action() : NodeState.Failure;
    }
}

public class QuestionNode : ITreeNode
{
    private ITreeNode trueNode;
    private ITreeNode flaseNode;
    private Func<bool> question;

    public QuestionNode(Func<bool> question, ITreeNode trueNode, ITreeNode falseNode)
    {
        this.question = question;
        this.trueNode = trueNode;
        this.flaseNode = falseNode;
    }
    public NodeState Execute()
    {
        if (question.Invoke())
            return trueNode.Execute();
        else
            return flaseNode.Execute();
    }
}

public class SequenceNode : Nodes, ITreeNode
{
    private List<ITreeNode> sequence;
    private int currentIndex = 0;
    public SequenceNode(List<ITreeNode> sequence)
    {
        this.sequence = sequence;
    }
    public NodeState Execute()
    {
        while (currentIndex < sequence.Count)
        {
            var state = sequence[currentIndex].Execute();

            if (state == NodeState.Running)
                return NodeState.Running;

            if (state == NodeState.Failure)
            {
                currentIndex = 0;
                return NodeState.Failure;
            }
            currentIndex++;
        }
        currentIndex = 0;
        return NodeState.Success;
    }
    public void Add(ITreeNode node)
    {
        sequence.Add(node);
    }
}

