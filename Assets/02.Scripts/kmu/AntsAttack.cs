using Ant.AI;
using KMU;
using System;
using System.Collections;
using System.Collections.Generic;
using Team.manager;
using UnityEngine;

namespace KMU
{

    public class AntsAttack : MonoBehaviour
    {
        private kmu.AntColor antColor;
        public Collider2D attackZone;
        private PathFinding pathfinding;
        private float eatTime = 5f;

        private void Start()
        {
            pathfinding = GetComponent<PathFinding>();
        }

        // 개미 색상 설정
        public void SetAntColor(kmu.AntColor antColor)
        {
            this.antColor = antColor;
        }

        // 개미 상성 비교
        private bool IsAttackOther(kmu.AntContoller ants)
        {
            if (antColor == kmu.AntColor.Red && ants.antColor == kmu.AntColor.Green) // 개미가 빨간색일 때
            {
                return true;
            }
            else if (antColor == kmu.AntColor.Blue && ants.antColor == kmu.AntColor.Red) // 개미가 파란색일 때
            {
                return true;
            }
            else if (antColor == kmu.AntColor.Green && ants.antColor == kmu.AntColor.Blue) // 개미가 초록색일 때
            {
                return true;
            }
            return false;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (!GameManager.Instance.isGameStart) return;

            if (collision.CompareTag("Ant")) // 적 공격
            {
               kmu.AntContoller ant = collision.GetComponent<kmu.AntContoller>();

                // 공격영역에 접근시 상성 비교
                if (IsAttackOther(ant))
                {
                    Destroy(collision.gameObject);
                }

            }
            else if (collision.CompareTag("Food")) // 먹이 반응
            {
                collision.enabled = false;
                StartCoroutine(EatCoroutine(collision));
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

