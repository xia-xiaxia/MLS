using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.AI;
using static Node; //使得在当前文件中可以直接使用 State 枚举，而不需要每次都写 Node.State

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

public class Selector : Node //选择器：只要有一个子节点成功就返回成功
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

public class Sequence : Node //序列：只要有一个子节点失败就返回失败
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

public class Allocator : Node //分配器（自设节点）：决定执行哪个子节点，默认首子节点返回Failure
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

public class ExecuteAll : Node //（自设节点）执行所有子节点，永远返回Success
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


public class Inverter : Node //装饰节点：翻转子节点的状态
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

public class PrioritySelector : Selector //优先级选择器
{
    List<Node> sortedChildren;
    List<Node> SortedChildren => sortedChildren ??= SortChildren();//??= 是空合并运算符，如果 sortedChildren 为空，则执行 SortChildren() 否则直接返回这个值

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








public class UntilFail : Node //装饰节点：直到子节点返回失败
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
//public class RandomSelector : PrioritySelector//随机选择器，继承自优先级选择器，只是在 SortChildren() 方法中对子节点进行了随机排序
//{
//    protected override List<Node> SortChildren() => children.Shuffle().ToList();

//    public RandomSelector(string name, int priority = 0) : base(name, priority) { }
//}
public class ElderSelector : Node //选择器：只要有一个子节点成功就返回成功
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
public class ElderSequence : Node //序列：只要有一个子节点失败就返回失败
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
