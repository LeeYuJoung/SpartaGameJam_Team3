using Ant.AI;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace yjlee.Ant
{
    public class AlertSystem : MonoBehaviour
    {
        private PathFinding2 pathFinding;

        // fov�� 45��� 45�� �����ȿ� �ִ� aesteriod�� �ν��� �� ����.
        [SerializeField] private float fov = 45f;
        // radius�� 10�̶�� ������ 10 �������� aesteriod���� �ν��� �� ����.
        [SerializeField] private float radius = 10f;
        private float alertThreshold;

        //private static readonly int blinking = Animator.StringToHash("isBlinking");
        public Transform aestreiod;

        private Color color = new Color(0, 1, 0, 0.3f);
        private void Start()
        {
            // FOV�� �������� ��ȯ�ϰ� �ڻ��� ���� ���
            alertThreshold = Mathf.Cos(fov / 2 * Mathf.Deg2Rad);
            pathFinding = GetComponent<PathFinding2>();
        }

        private void Update()
        {
            //if (aestreiod == null)
            //    return;

            //CheckAlert();
        }

        private void CheckAlert()
        {
            // ��ü�� ������ ����
            Vector2 targetDir = aestreiod.position - transform.position;

            if (targetDir.magnitude <= radius) // ������Ʈ�� �÷��̾��� Ž�� ������ ���� �� 
            {
                // ����
                float dot = Vector2.Dot(transform.up, targetDir.normalized);

                // ������ ���� ���� / 2 �ڻ��� ������ ũ�ٸ� => �÷��̾ �ٶ󺸴� ���⿡ �� �����ٸ�
                if (dot >= alertThreshold)
                {
                    // Ž��
                    pathFinding.target = aestreiod.gameObject;
                }
                else
                {

                }
            }
            else
            {

            }
        }

        private void OnDrawGizmos() // �� â���� ��ä�� ���� �׸���
        {
            Handles.color = color;

            // �þ��� ���� ���� ���� ���
            Vector2 startDirection = Quaternion.Euler(0, 0, fov / 2) * transform.up;

            // DrawSolidArc �Լ��� �̿��Ͽ� �þ� ������ ��Ÿ���� ��ä�� �׸���
            Handles.DrawSolidArc(transform.position, Vector3.back, startDirection, fov, radius);
        }
    }
}
