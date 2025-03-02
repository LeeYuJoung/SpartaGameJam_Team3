using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Ant.AI
{
     /* A* Algorithm 개요
     * OPEN SET : 평가되어야 할 노드 집합
     * CLOSED SET : 이미 평가된 노드 집합
     * 
     * 1. OPEN SET에서 가장 낮은 fCost를 가진 노드 획득 후 CLOSED SET 삽입
     * 2. 이 노드가 목적지라면 반복문 탈출
     * 3. 이 노드의 주변 노드들을 CLOSED SET에 넣고, 주변 노드의 f값 계산 (주변 노드의 g값 보다 작다면 f값으로 g값 최신화)
     * 4. 1번 반복
     */
    public class PathFinding : MonoBehaviour
    {
        public GameObject target;

        // Map을 격자로 분할
        public Grid grid;
        // 남은거리를 넣을 Queue 생성
        public Queue<Vector2> wayQueue = new Queue<Vector2>();

        // 플레이어 이동/회전 속도 등을 저장할 변수
        public float moveSpeed;
        // 장애물 판단시 멈출게 할 범위
        public float range;

        public Vector2 dir;

        public bool isWalking;
        // 상호작용 시 walkable를 false 상태로 변환
        public bool walkable = true;

        private void Awake()
        {
            walkable = false;
            isWalking = false;
        }

        private void Start()
        {
            grid = GameObject.Find("Grid").GetComponent<Grid>();
        }

        private void FixedUpdate()
        {
            if (!walkable || target == null)
            {
                StopAllCoroutines();
                return;
            }

            StartFindPaath((Vector2)transform.position, (Vector2)target.transform.position);
        }

        // Start to target 이동
        public void StartFindPaath(Vector2 startPos, Vector2 targetPos)
        {
            StopAllCoroutines();
            StartCoroutine(FindPath(startPos, targetPos));
        }

        #region 길찾기 로직
        IEnumerator FindPath(Vector2 startPos, Vector2 targetPos)
        {
            // Start, target의 좌표를 grid로 분할한 좌표로 지정
            Node startNode = grid.NodeFromWorldPoint(startPos);
            Node targetNode = grid.NodeFromWorldPoint(targetPos);

            bool pathSuccess = false;

            if (!targetNode.isWalkable)
                Debug.Log(":: UnWalkable StartNode ::");

            // walkable한 targetNode인 경우 길찾기 시작
            if (targetNode.isWalkable)
            {
                // openSet, closedSet 생성
                // closedSet은 이미 계산 고려한 노드들
                // openSet은 계산할 가치가 있는 노드들
                List<Node> openSet = new List<Node>();
                HashSet<Node> closedSet = new HashSet<Node>();

                openSet.Add(startNode);

                // closedSet에서 가장 최저의 f를 가지는 노드를 뺌
                while (openSet.Count > 0)
                {
                    // currentNode를 계산 후 openSet에서 빼야 함
                    Node currentNode = openSet[0];

                    // 모든 openSet에 대해, current보다 f값이 작거나, h(휴리스틱)값이 작으면 그것을 current로 지정
                    for (int i = 1; i < openSet.Count; i++)
                    {
                        if (openSet[i].fCost < currentNode.fCost || openSet[i].fCost == currentNode.fCost && openSet[i].hCost < currentNode.hCost)
                        {
                            currentNode = openSet[i];
                        }
                    }

                    // openSet에서 current를 뺀 후, closed에 추가
                    openSet.Remove(currentNode);
                    closedSet.Add(currentNode);

                    // 방금 들어온 노드가 목적지인 경우
                    if (currentNode == targetNode)
                    {
                        // seeker가 위치한 지점이 target이 아닌 경우
                        if (!pathSuccess)
                        {
                            PushWay(RetracePath(startNode, targetNode));
                        }

                        pathSuccess = true;
                        break;
                    }

                    // current의 상하좌우 노드들에 대해 g, hCost를 고려
                    foreach (Node neighbour in grid.GetNeighbours(currentNode))
                    {
                        if (!neighbour.isWalkable || closedSet.Contains(neighbour))
                            continue;

                        // fCost 생성
                        int newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour);

                        // 이웃으로 가는 fCost가 이웃의 g보다 짧거나, 방문해볼 openSet에 그 값이 없다면
                        if (newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                        {
                            neighbour.gCost = newMovementCostToNeighbour;
                            neighbour.hCost = GetDistance(neighbour, targetNode);
                            neighbour.parent = currentNode;

                            if (!openSet.Contains(neighbour))
                                openSet.Add(neighbour);
                        }
                    }
                }
            }

            yield return null;

            // 길을 찾았을 경우(계산이 다 끝난 경우) 이동
            if (pathSuccess)
            {
                // 이동하려는 변수 On
                isWalking = true;

                // wayQueue를 따라 이동
                while (wayQueue.Count > 0)
                {
                    dir = wayQueue.First() - (Vector2)transform.position;
                    gameObject.GetComponent<Rigidbody2D>().velocity = dir.normalized * moveSpeed * 5 * Time.deltaTime;

                    if ((Vector2)transform.position == wayQueue.First())
                    {
                        wayQueue.Dequeue();
                    }

                    yield return new WaitForSeconds(0.02f);
                }
            }
        }
        #endregion

        // WayQueue에 새로운 Path를 넣어주기
        private void PushWay(Vector2[] array)
        {
            wayQueue.Clear();
            foreach (Vector2 item in array)
            {
                wayQueue.Enqueue(item);
            }
        }

        // 현재 Queue에 거꾸로 저장되어있으므로, 역순으로 wayQueue를 뒤집어줌
        private Vector2[] RetracePath(Node startNode, Node endNode)
        {
            List<Node> path = new List<Node>();
            Node currentNode = endNode;

            while (currentNode != startNode)
            {
                path.Add(currentNode);
                currentNode = currentNode.parent;
            }

            path.Reverse();
            grid.path = path;
            Vector2[] wayPointns = SimplifyPath(path);

            return wayPointns;
        }

        // Node에서 Vector 정보만 추출
        private Vector2[] SimplifyPath(List<Node> path)
        {
            List<Vector2> wayPoints = new List<Vector2>();

            for (int i = 0; i < path.Count; i++)
            {
                wayPoints.Add(path[i].worldPosition);
            }

            return wayPoints.ToArray();
        }

        // custom gCost 또는 휴리스틱 추정치를 계산하는 함수
        // 매개변수로 들어오는 값에 따라 기능이 변함
        private int GetDistance(Node nodeA, Node nodeB)
        {
            int distX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
            int distY = Mathf.Abs(nodeA.gridY - nodeB.gridY);

            return (distX > distY) ? (14 * distY + 10 * (distX - distY)) : (14 * distX + 10 * (distY - distX));
        }
    }
}