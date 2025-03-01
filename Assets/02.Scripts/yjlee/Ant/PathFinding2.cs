using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Ant.AI
{
    public class PathFinding2 : MonoBehaviour
    {
        public GameObject target;

        // Map�� ���ڷ� ����
        public Grid grid;
        // �����Ÿ��� ���� Queue ����
        public Queue<Vector2> wayQueue = new Queue<Vector2>();

        // �÷��̾� �̵�/ȸ�� �ӵ� ���� ������ ����
        public float moveSpeed;
        // ��ֹ� �Ǵܽ� ����� �� ����
        public float range;

        public Vector2 dir;

        public bool isWalking;
        // ��ȣ�ۿ� �� walkable�� false ���·� ��ȯ
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

            StartFindPaath((Vector2)transform.position, (Vector2)target.transform.position);
        }

        // Start to target �̵�
        public void StartFindPaath(Vector2 startPos, Vector2 targetPos)
        {
            StopAllCoroutines();
            StartCoroutine(FindPath(startPos, targetPos));
        }

        #region ��ã�� ����
        IEnumerator FindPath(Vector2 startPos, Vector2 targetPos)
        {
            // Start, target�� ��ǥ�� grid�� ������ ��ǥ�� ����
            Node startNode = grid.NodeFromWorldPoint(startPos);
            Node targetNode = grid.NodeFromWorldPoint(targetPos);

            bool pathSuccess = false;

            if (!targetNode.isWalkable)
                Debug.Log(":: UnWalkable StartNode ::");

            // walkable�� targetNode�� ��� ��ã�� ����
            if (targetNode.isWalkable)
            {
                // openSet, closedSet ����
                // closedSet�� �̹� ��� ����� ����
                // openSet�� ����� ��ġ�� �ִ� ����
                List<Node> openSet = new List<Node>();
                HashSet<Node> closedSet = new HashSet<Node>();

                openSet.Add(startNode);

                // closedSet���� ���� ������ f�� ������ ��带 ��
                while (openSet.Count > 0)
                {
                    // currentNode�� ��� �� openSet���� ���� ��
                    Node currentNode = openSet[0];

                    // ��� openSet�� ����, current���� f���� �۰ų�, h(�޸���ƽ)���� ������ �װ��� current�� ����
                    for (int i = 1; i < openSet.Count; i++)
                    {
                        if (openSet[i].fCost < currentNode.fCost || openSet[i].fCost == currentNode.fCost && openSet[i].hCost < currentNode.hCost)
                        {
                            currentNode = openSet[i];
                        }
                    }

                    // openSet���� current�� �� ��, closed�� �߰�
                    openSet.Remove(currentNode);
                    closedSet.Add(currentNode);

                    // ��� ���� ��尡 �������� ���
                    if (currentNode == targetNode)
                    {
                        // seeker�� ��ġ�� ������ target�� �ƴ� ���
                        if (!pathSuccess)
                        {
                            PushWay(RetracePath(startNode, targetNode));
                        }

                        pathSuccess = true;
                        break;
                    }

                    // current�� �����¿� ���鿡 ���� g, hCost�� ���
                    foreach (Node neighbour in grid.GetNeighbours(currentNode))
                    {
                        if (!neighbour.isWalkable || closedSet.Contains(neighbour))
                            continue;

                        // fCost ����
                        int newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour);

                        // �̿����� ���� fCost�� �̿��� g���� ª�ų�, �湮�غ� openSet�� �� ���� ���ٸ�
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

            // ���� ã���� ���(����� �� ���� ���) �̵�
            if (pathSuccess)
            {
                // �̵��Ϸ��� ���� On
                isWalking = true;

                // wayQueue�� ���� �̵�
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

        // WayQueue�� ���ο� Path�� �־��ֱ�
        private void PushWay(Vector2[] array)
        {
            wayQueue.Clear();
            foreach (Vector2 item in array)
            {
                wayQueue.Enqueue(item);
            }
        }

        // ���� Queue�� �Ųٷ� ����Ǿ������Ƿ�, �������� wayQueue�� ��������
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

        // Node���� Vector ������ ����
        private Vector2[] SimplifyPath(List<Node> path)
        {
            List<Vector2> wayPoints = new List<Vector2>();

            for (int i = 0; i < path.Count; i++)
            {
                wayPoints.Add(path[i].worldPosition);
            }

            return wayPoints.ToArray();
        }

        // custom gCost �Ǵ� �޸���ƽ ����ġ�� ����ϴ� �Լ�
        // �Ű������� ������ ���� ���� ����� ����
        private int GetDistance(Node nodeA, Node nodeB)
        {
            int distX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
            int distY = Mathf.Abs(nodeA.gridY - nodeB.gridY);

            return (distX > distY) ? (14 * distY + 10 * (distX - distY)) : (14 * distX + 10 * (distY - distX));
        }
    }
}
