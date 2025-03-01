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

        // fov가 45라면 45도 각도안에 있는 aesteriod를 인식할 수 있음.
        [SerializeField] private float fov = 45f;
        // radius가 10이라면 반지름 10 범위에서 aesteriod들을 인식할 수 있음.
        [SerializeField] private float radius = 10f;
        private float alertThreshold;

        //private static readonly int blinking = Animator.StringToHash("isBlinking");
        public Transform aestreiod;

        private Color color = new Color(0, 1, 0, 0.3f);
        private void Start()
        {
            // FOV를 라디안으로 변환하고 코사인 값을 계산
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
            // 물체의 방향을 구함
            Vector2 targetDir = aestreiod.position - transform.position;

            if (targetDir.magnitude <= radius) // 오브젝트가 플레이어의 탐지 범위에 들어올 때 
            {
                // 내적
                float dot = Vector2.Dot(transform.up, targetDir.normalized);

                // 내적한 값이 각도 / 2 코사인 값보다 크다면 => 플레이어가 바라보는 방향에 더 가깝다면
                if (dot >= alertThreshold)
                {
                    // 탐지
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

        private void OnDrawGizmos() // 씬 창에서 부채꼴 범위 그리기
        {
            Handles.color = color;

            // 시야의 시작 방향 벡터 계산
            Vector2 startDirection = Quaternion.Euler(0, 0, fov / 2) * transform.up;

            // DrawSolidArc 함수를 이용하여 시야 범위를 나타내는 부채꼴 그리기
            Handles.DrawSolidArc(transform.position, Vector3.back, startDirection, fov, radius);
        }
    }
}
