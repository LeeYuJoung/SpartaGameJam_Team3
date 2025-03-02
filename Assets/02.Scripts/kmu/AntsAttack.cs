using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KMU
{

    public class AntsAttack : MonoBehaviour
    {
        private AntColor antColor;

        public AntColor AntColor => antColor;

        // ���� ���� ����
        public void SetAntColor(AntColor antColor)
        {
            this.antColor = antColor;
            Debug.Log("�� ������ �� : " + antColor);
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
            if (collision.CompareTag("Ant"))
            {
                AntsMove ants = collision.GetComponent<AntsMove>();

                // ���ݿ����� ���ٽ� �� ��
                if (IsAttackOther(ants))
                {
                    Destroy(collision.gameObject);
                }

            }
        }
    }
}

