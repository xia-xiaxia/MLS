using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.AI;
using static Node; //ʹ���ڵ�ǰ�ļ��п���ֱ��ʹ�� State ö�٣�������Ҫÿ�ζ�д Node.State

public class Node // Enemy Behavior Tree Node
{
    public enum State { Success, Failure, Running }

    public readonly string name;
    public readonly int priority;
    public readonly List<Node> children = new();
    protected int currentChild;

    public Node(string name = "Node", int priority = 0)
    {
        this.name = name;
        this.priority = priority;
    }

    public void AddChild(Node child) => children.Add(child);

    public virtual State Execute() => children[currentChild].Execute();

    public virtual void Reset()
    {
        currentChild = 0;
        foreach (var child in children)
        {
            child.Reset();
        }
    }
}

public class Selector : Node //ѡ������ֻҪ��һ���ӽڵ�ɹ��ͷ��سɹ�
{
    public Selector(string name, int priority = 0) : base(name, priority) { }

    public override State Execute()
    {
        Reset();
        while (currentChild < children.Count)
        {
            switch (children[currentChild].Execute())
            {
                case State.Running:
                    //Debug.Log(name + " Selector " + " " + currentChild + " Running");
                    return State.Running;
                case State.Success:
                    //Debug.Log(name + " Selector " + " " + currentChild + " Success");
                    Reset();
                    return State.Success;
                default:
                    //Debug.Log(name + " Selector " + " " + currentChild + " Failure");
                    currentChild++;
                    break;
            }
        }
        return State.Failure;
    }
}

public class Sequence : Node //���У�ֻҪ��һ���ӽڵ�ʧ�ܾͷ���ʧ��
{
    public Sequence(string name, int priority = 0) : base(name, priority) { }

    public override State Execute()
    {
        Reset();
        while (currentChild < children.Count)
        {
            switch (children[currentChild].Execute())
            {
                case State.Running:
                    //Debug.Log(name + " Sequence " + " " + currentChild + " Running");
                    return State.Running;
                case State.Failure:
                    Reset();
                    //Debug.Log(name + " Sequence " + " " + currentChild + " Failure");
                    return State.Failure;
                default:
                    //Debug.Log(name + " Sequence " + " " + currentChild + " Success");
                    currentChild++;
                    break;
            }
        }
        return State.Success;
    }
}

public class Leaf : Node
{
    readonly IStrategy strategy;

    public Leaf(string name, IStrategy strategy, int priority = 0) : base(name, priority)
    {
        // Preconditions.CheckNotNull(strategy);
        this.strategy = strategy;
    }

    public override State Execute() => strategy.Execute();

    public override void Reset() => strategy.Reset();
}

public class Allocator : Node //������������ڵ㣩������ִ���ĸ��ӽڵ㣬Ĭ�����ӽڵ㷵��Failure
{
    private AllocatorDelegate allocatorDelegate;

    public Allocator(string name, AllocatorDelegate allocatorDelegate, int priority = 0) : base(name, priority)
    {
        this.allocatorDelegate = allocatorDelegate;
        AddChild(new Leaf("default", new ConditionStrategy(() => false)));
    }
    public override State Execute()
    {
        return children[allocatorDelegate()].Execute();
    }
}
public delegate int AllocatorDelegate();

public class ExecuteAll : Node //������ڵ㣩ִ�������ӽڵ㣬��Զ����Success
{
    public ExecuteAll(string name, int priority = 0) : base(name, priority) { }
    public override State Execute()
    {
        Reset();
        while (currentChild < children.Count)
        {
            children[currentChild].Execute();
            currentChild++;
        }
        return State.Success;
    }
}


public class Inverter : Node //װ�νڵ㣺��ת�ӽڵ��״̬
{
    public Inverter(string name) : base(name) { }
    public Inverter(string name, Node node) : base(name) => AddChild(node);

    public override State Execute()
    {
        switch (children[0].Execute())
        {
            case State.Running:
                return State.Running;
            case State.Failure:
                return State.Success;
            default:
                return State.Failure;
        }
    }
}

public class PrioritySelector : Selector //���ȼ�ѡ����
{
    List<Node> sortedChildren;
    List<Node> SortedChildren => sortedChildren ??= SortChildren();//??= �ǿպϲ����������� sortedChildren Ϊ�գ���ִ�� SortChildren() ����ֱ�ӷ������ֵ

    protected virtual List<Node> SortChildren() => children.OrderByDescending(child => child.priority).ToList();

    public PrioritySelector(string name, int priority = 0) : base(name, priority) { }

    public override void Reset()
    {
        base.Reset();
        sortedChildren = null;
    }

    public override State Execute()
    {
        foreach (var child in SortedChildren)
        {
            switch (child.Execute())
            {
                case State.Running:
                    return State.Running;
                case State.Success:
                    Reset();
                    return State.Success;
                default:
                    continue;
            }
        }

        Reset();
        return State.Failure;
    }
}








public class UntilFail : Node //װ�νڵ㣺ֱ���ӽڵ㷵��ʧ��
{
    public UntilFail(string name) : base(name) { }

    public override State Execute()
    {
        if (children[0].Execute() == State.Failure)
        {
            Reset();
            return State.Failure;
        }

        return State.Running;
    }
}
//public class RandomSelector : PrioritySelector//���ѡ�������̳������ȼ�ѡ������ֻ���� SortChildren() �����ж��ӽڵ�������������
//{
//    protected override List<Node> SortChildren() => children.Shuffle().ToList();

//    public RandomSelector(string name, int priority = 0) : base(name, priority) { }
//}
public class ElderSelector : Node //ѡ������ֻҪ��һ���ӽڵ�ɹ��ͷ��سɹ�
{
    public ElderSelector(string name, int priority = 0) : base(name, priority) { }

    public override State Execute()
    {
        if (currentChild < children.Count)
        {
            switch (children[currentChild].Execute())
            {
                case State.Running:
                    return State.Running;
                case State.Success:
                    Reset();
                    return State.Success;
                default:
                    currentChild++;
                    return State.Running;
            }
        }

        Reset();
        return State.Failure;
    }
}
public class ElderSequence : Node //���У�ֻҪ��һ���ӽڵ�ʧ�ܾͷ���ʧ��
{
    public ElderSequence(string name, int priority = 0) : base(name, priority) { }

    public override State Execute()
    {
        if (currentChild < children.Count)
        {
            switch (children[currentChild].Execute())
            {
                case State.Running:
                    return State.Running;
                case State.Failure:
                    currentChild = 0;
                    return State.Failure;
                default:
                    currentChild++;
                    return currentChild == children.Count ? State.Success : State.Running;
            }
        }
        Reset();
        return State.Success;
    }
}
