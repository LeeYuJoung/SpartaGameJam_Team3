using Ant.AI;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

namespace KMU
{
    public enum AntColor { Red = 3, Blue = 4, Yellow = 5 }

   
    public class AntsMove : MonoBehaviour
    {
        private float speed = 2f;
        PathFinding pathfinding;
        private int fakeIndex;
        public GameObject[] targets;
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

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

        private void FakeTarget()
        {
            fakeIndex = Random.Range(0, targets.Length);
            pathfinding.StartFindPaath(transform.position, targets[fakeIndex].transform.position);
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Goal"))
            {
                Destroy(gameObject);
                pathfinding.isWalking = false;

            }
            else if (collision.CompareTag("FakeGoal"))
            {
                FakeTarget();
            }
        }
    }
}
