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

        // ���� ���� ����
        public void SetAntColor(kmu.AntColor antColor)
        {
            this.antColor = antColor;
        }

        // ���� �� ��
        private bool IsAttackOther(kmu.AntContoller ants)
        {
            if (antColor == kmu.AntColor.Red && ants.antColor == kmu.AntColor.Green) // ���̰� �������� ��
            {
                return true;
            }
            else if (antColor == kmu.AntColor.Blue && ants.antColor == kmu.AntColor.Red) // ���̰� �Ķ����� ��
            {
                return true;
            }
            else if (antColor == kmu.AntColor.Green && ants.antColor == kmu.AntColor.Blue) // ���̰� �ʷϻ��� ��
            {
                return true;
            }
            return false;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (!GameManager.Instance.isGameStart) return;

            if (collision.CompareTag("Ant")) // �� ����
            {
               kmu.AntContoller ant = collision.GetComponent<kmu.AntContoller>();

                // ���ݿ����� ���ٽ� �� ��
                if (IsAttackOther(ant))
                {
                    Destroy(collision.gameObject);
                }

            }
            else if (collision.CompareTag("Food")) // ���� ����
            {
                collision.enabled = false;
                StartCoroutine(EatCoroutine(collision));
            }
            
        }

        private IEnumerator EatCoroutine(Collider2D food)
        {

            pathfinding.isEating = true; // �̵� ����
            attackZone.enabled = false; // �������� �ݶ��̴� ��Ȱ��ȭ
            yield return new WaitForSeconds(eatTime);

            pathfinding.isEating = false; // �̵�����
            attackZone.enabled = true; // �������� �ݶ��̴� Ȱ��ȭ

            Destroy(food.gameObject);

        }
    }
}

