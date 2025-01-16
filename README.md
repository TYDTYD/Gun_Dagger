
## Gun & Dagger

### [플레이 영상](https://youtu.be/Sm8UuIJbXFk)

### 프로젝트 소개
 - 게임 장르 : 2D 로그라이크 게임
 - 제작 기간 : 2024.03 ~ 2024.09
 - 프로젝트 목표 : 게임 공모전 출시
   
### 개발 규모
 - 팀 인원 : 아트 2명, 기획 1명, 개발 2명 총 5명
 - 나의 역할 : 클라이언트

### 기술 설명서

<details>
  <summary>
    적 AI 행동 트리 설계
  </summary>
 <pre>
  
 ```cs
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
```
 </pre>
</details>

<details>
  <summary>
    플레이어 추적 A Star 알고리즘 구현
  </summary>
 <pre>
   <code>
    private void start(){
    }
   </code>
 </pre>
</details>

<details>
  <summary>
    Object Pooling을 통한 최적화
  </summary>
 <pre>
   <code>
    private void start(){
    }
   </code>
 </pre>
</details>

<details>
  <summary>
    플레이어 근접 공격 구현
  </summary>
 <pre>
   <code>
    private void start(){
    }
   </code>
 </pre>
</details>
