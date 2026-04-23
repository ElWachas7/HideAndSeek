using System;
using System.Collections;
using System.Collections.Generic;
using TreeEditor;
using UnityEngine;

public class Nodes : MonoBehaviour
{
    
}

public class ActionNode : ITreeNode
{
    private Action action;

    public ActionNode(Action action)
    {
        this.action = action;
    }

    public void Execute()
    {
        action?.Invoke();
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
    public void Execute()
    {
        if (question.Invoke())
            trueNode.Execute();
        else
            flaseNode.Execute();
    }
}
