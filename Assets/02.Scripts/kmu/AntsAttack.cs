using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KMU
{

    public class AntsAttack : MonoBehaviour
    {
        private AntColor antColor;

        public AntColor AntColor => antColor;

        // 개미 색상 설정
        public void SetAntColor(AntColor antColor)
        {
            this.antColor = antColor;
            Debug.Log("이 개미의 상성 : " + antColor);
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
            if (collision.CompareTag("Ant"))
            {
                AntsMove ants = collision.GetComponent<AntsMove>();

                // 공격영역에 접근시 상성 비교
                if (IsAttackOther(ants))
                {
                    Destroy(collision.gameObject);
                }

            }
        }
    }
}

