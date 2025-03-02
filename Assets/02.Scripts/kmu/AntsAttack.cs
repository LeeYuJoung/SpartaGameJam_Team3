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

        // ���� ���� ����
        public void SetAntColor(AntColor antColor)
        {
            this.antColor = antColor;
        }

        // ���� �� ��
        private bool IsAttackOther(AntsMove ants)
        {
            if (antColor == AntColor.Red && ants.antColor == AntColor.Green) // ���̰� �������� ��
            {
                return true;
            }
            else if (antColor == AntColor.Blue && ants.antColor == AntColor.Red) // ���̰� �Ķ����� ��
            {
                return true;
            }
            else if (antColor == AntColor.Green && ants.antColor == AntColor.Blue) // ���̰� �ʷϻ��� ��
            {
                return true;
            }
            return false;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("AttackZone")) // �� ����
            {
                AntsMove ants = collision.GetComponentInParent<AntsMove>();

                // ���ݿ����� ���ٽ� �� ��
                if (IsAttackOther(ants))
                {
                    Destroy(collision.gameObject);
                }

            }
            else if (collision.CompareTag("Food")) // ���� ����
            {
                collision.enabled = false;
                StartCoroutine(EatCoroutine(collision));
            }
            else if (collision.CompareTag("Goal")) // ������ ����
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

            pathfinding.isEating = true; // �̵� ����
            attackZone.enabled = false; // �������� �ݶ��̴� ��Ȱ��ȭ
            yield return new WaitForSeconds(eatTime);

            pathfinding.isEating = false; // �̵�����
            attackZone.enabled = true; // �������� �ݶ��̴� Ȱ��ȭ

            Destroy(food.gameObject);

        }
    }
}

