using Ant.AI;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using yjlee.Ant;

namespace KMU
{
    public enum AntColor { Red = 0, Blue = 1, Green = 2 }

   
    public class AntsMove : MonoBehaviour
    {
        public bool isGameStart = false;


        public AntColor ?antColor; // 개미 색상
        public GameObject antscarf;
        public GameObject[] targets; // 골인 지점

        AntsAttack antAttack;
        PathFinding pathfinding;

        private void Start()
        {
            antColor = null;
            pathfinding = GetComponent<PathFinding>();
            antAttack = GetComponent<AntsAttack>();
        }

        public void SetAntColor(AntColor antColor)
        {
            this.antColor = antColor;
            antAttack.SetAntColor(antColor);

            pathfinding.target = targets[(int)antColor]; // 목표지점 전달

        }

        private void Update()
        {
            if (isGameStart) // 게임 시작 버튼을 눌러야 개미들이 움직임.
            {
                pathfinding.isWalking = true;
                pathfinding.walkable = true;
            }

            if (pathfinding.isWalking)
            {
                RotateAnts();
            }
        }

        // 개미 이동 방향으로 회전
        private void RotateAnts()
        {
            if (pathfinding.wayQueue.Count > 0)
            {
                Vector2 nextPosition = pathfinding.wayQueue.Peek();
                Vector2 direction = (nextPosition - (Vector2)transform.position).normalized;

                float angle = Mathf.Atan2(-direction.x, direction.y) * Mathf.Rad2Deg;
                Quaternion targetRotation = Quaternion.Euler(0, 0, angle);

                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
            }
        }

    }
}
