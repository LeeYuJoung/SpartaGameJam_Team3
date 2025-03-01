using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Ant.AI
{
    public class PathFinding : MonoBehaviour
    {
        public GameObject target; // 현재 이동 목표
        public Grid grid;
        public Queue<Vector2> wayQueue = new Queue<Vector2>();
        public float moveSpeed;
        public float range;
        public Vector2 dir;
        public bool isWalking;
        public bool walkable = true;

        private void Awake()
        {
            walkable = true;
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

            // 🔥 목표가 바뀔 때마다 경로를 즉시 탐색
            StartFindPaath((Vector2)transform.position, (Vector2)target.transform.position);
        }

        // ✅ 새로운 경로 탐색
        public void StartFindPaath(Vector2 startPos, Vector2 targetPos)
        {
            StopAllCoroutines();
            StartCoroutine(FindPath(startPos, targetPos));
        }

        #region 길찾기 로직
        IEnumerator FindPath(Vector2 startPos, Vector2 targetPos)
        {
            Node startNode = grid.NodeFromWorldPoint(startPos);
            Node targetNode = grid.NodeFromWorldPoint(targetPos);
            bool pathSuccess = false;

            if (!targetNode.isWalkable)
                Debug.Log(":: UnWalkable StartNode ::");

            if (targetNode.isWalkable)
            {
                List<Node> openSet = new List<Node>();
                HashSet<Node> closedSet = new HashSet<Node>();
                openSet.Add(startNode);

                while (openSet.Count > 0)
                {
                    Node currentNode = openSet[0];
                    for (int i = 1; i < openSet.Count; i++)
                    {
                        if (openSet[i].fCost < currentNode.fCost || openSet[i].fCost == currentNode.fCost && openSet[i].hCost < currentNode.hCost)
                        {
                            currentNode = openSet[i];
                        }
                    }

                    openSet.Remove(currentNode);
                    closedSet.Add(currentNode);

                    if (currentNode == targetNode)
                    {
                        if (!pathSuccess)
                        {
                            PushWay(RetracePath(startNode, targetNode));
                        }
                        pathSuccess = true;
                        break;
                    }

                    foreach (Node neighbour in grid.GetNeighbours(currentNode))
                    {
                        if (!neighbour.isWalkable || closedSet.Contains(neighbour))
                            continue;

                        int newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour);
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

            if (pathSuccess)
            {
                isWalking = true;
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

            // ✅ 목표에 도착하면 자동으로 새로운 목표 설정
            isWalking = false;
            SendMessage("OnTargetReached", SendMessageOptions.DontRequireReceiver);
        }
        #endregion

        private void PushWay(Vector2[] array)
        {
            wayQueue.Clear();
            foreach (Vector2 item in array)
            {
                wayQueue.Enqueue(item);
            }
        }

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
            return SimplifyPath(path);
        }

        private Vector2[] SimplifyPath(List<Node> path)
        {
            List<Vector2> wayPoints = new List<Vector2>();
            for (int i = 0; i < path.Count; i++)
            {
                wayPoints.Add(path[i].worldPosition);
            }
            return wayPoints.ToArray();
        }

        private int GetDistance(Node nodeA, Node nodeB)
        {
            int distX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
            int distY = Mathf.Abs(nodeA.gridY - nodeB.gridY);
            return (distX > distY) ? (14 * distY + 10 * (distX - distY)) : (14 * distX + 10 * (distY - distX));
        }
    }
}
