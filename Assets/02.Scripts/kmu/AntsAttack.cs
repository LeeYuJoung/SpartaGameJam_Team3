using Ant.AI;
using KMU;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KMU
{

    public class AntsAttack : MonoBehaviour
    {
        private AntColor antColor;
        [SerializeField] private Collider2D attackZone;
        private PathFinding pathfinding;
        private float eatTime = 5f;

        private void Start()
        {
            pathfinding = GetComponent<PathFinding>();
        }

        // 개미 색상 설정
        public void SetAntColor(AntColor antColor)
        {
            this.antColor = antColor;
        }

        // 개미 상성 비교
        private bool IsAttackOther(AntsMove ants)
        {
            if (antColor == AntColor.Red && ants.antColor == AntColor.Green) // 개미가 빨간색일 때
            {
                return true;
            }
            else if (antColor == AntColor.Blue && ants.antColor == AntColor.Red) // 개미가 파란색일 때
            {
                return true;
            }
            else if (antColor == AntColor.Green && ants.antColor == AntColor.Blue) // 개미가 초록색일 때
            {
                return true;
            }
            return false;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("AttackZone")) // 적 공격
            {
                AntsMove ants = collision.GetComponentInParent<AntsMove>();

                // 공격영역에 접근시 상성 비교
                if (IsAttackOther(ants))
                {
                    Destroy(collision.gameObject);
                }

            }
            else if (collision.CompareTag("Food")) // 먹이 반응
            {
                collision.enabled = false;
                StartCoroutine(EatCoroutine(collision));
            }
            else if (collision.CompareTag("Goal")) // 목적지 도착
            {
                Goal goal = collision.GetComponent<Goal>();

                if (goal.goalColor == antColor)
                {
                    Destroy(gameObject);
                    pathfinding.isWalking = false;
                }
            }

            
        }

        private IEnumerator EatCoroutine(Collider2D food)
        {

            pathfinding.isEating = true; // 이동 정지
            attackZone.enabled = false; // 어택존의 콜라이더 비활성화
            yield return new WaitForSeconds(eatTime);

            pathfinding.isEating = false; // 이동시작
            attackZone.enabled = true; // 어택존의 콜라이더 활성화

            Destroy(food.gameObject);

        }
    }
}

