using UnityEngine;

namespace Ant.AI
{
    // Grid 안에 들어갈 Node
    public class Node
    {
        public Vector2 worldPosition;
        public int gridX;
        public int gridY;

        public Node parent;  // 길 추적을 위한 parent 변수
        public int gCost;    // 출발지로부터 현재 노드까지의 최단거리
        public int hCost;    // 목적지까지의 예상거리

        public bool isWalkable;

        // gCost와 hCost를 합친 최종점수 반환
        public int fCost
        {
            get { return gCost + hCost; }
        }

        public Node(bool walkable, Vector2 WorldPos, int x, int y)
        {
            isWalkable = walkable;     // 지나갈 수 있는 노드인지
            worldPosition = WorldPos;  // 노드의 게임 내 위치값
            gridX = x;                 // 노드의 x좌표 값
            gridY = y;                 // 노드의 y좌표 값
        }
    }
}
