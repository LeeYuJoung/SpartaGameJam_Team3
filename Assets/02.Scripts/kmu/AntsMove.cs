using Ant.AI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AntsColor { Red = 0, Blue = 1, Green = 2 }
namespace KMU
{
    public class AntsMove : MonoBehaviour
    {
        private PathFinding pathfinding;
        [SerializeField] AntsColor antsColor;
        [SerializeField] private int antsColorIndex;

        [SerializeField] GameObject[] targets;
        private int fakeIndex;

        private void Awake()
        {
            pathfinding = GetComponent<PathFinding>();
            fakeIndex = Random.Range(0, targets.Length);
            pathfinding.target = targets[fakeIndex];
        }

        private void Update()
        {
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

        private void FakeTarget()
        {
            fakeIndex = Random.Range(0, targets.Length);
            pathfinding.StartFindPaath(transform.position, targets[fakeIndex].transform.position);
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Goal"))
            {
                Goal goal = collision.GetComponent<Goal>();
                if (goal.goalColor == antsColorIndex)
                {
                    Destroy(gameObject);
                    pathfinding.isWalking = false;
                }
            }
            else if (collision.CompareTag("FakeGoal"))
            {
                FakeTarget();
            }
        }

    }
}