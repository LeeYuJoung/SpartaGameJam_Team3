using UnityEngine;

namespace Ant.AI
{
    // Grid �ȿ� �� Node
    public class Node
    {
        public Vector2 worldPosition;
        public int gridX;
        public int gridY;

        public Node parent;  // �� ������ ���� parent ����
        public int gCost;    // ������κ��� ���� �������� �ִܰŸ�
        public int hCost;    // ������������ ����Ÿ�

        public bool isWalkable;

        // gCost�� hCost�� ��ģ �������� ��ȯ
        public int fCost
        {
            get { return gCost + hCost; }
        }

        public Node(bool walkable, Vector2 WorldPos, int x, int y)
        {
            isWalkable = walkable;     // ������ �� �ִ� �������
            worldPosition = WorldPos;  // ����� ���� �� ��ġ��
            gridX = x;                 // ����� x��ǥ ��
            gridY = y;                 // ����� y��ǥ ��
        }
    }
}
