using Ant.AI;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;

namespace yjlee.Ant
{
    public enum AntColor
    {
        None,
        Red,
        Green, 
        Blue
    }

    public enum AntState
    {
        Idle,      // 기본
        Move,      // 이동
        Right,     // 오른쪾으로 이동
        Left,      // 왼쪽으로 이동 
        DeadEnd,   // 막다른길
        Search,    // 탐색
        Follow,    // 따르다
        GetEaten   // 잡아 먹히다
    }

    public class AntController : MonoBehaviour
    {
        public AntColor antColor = AntColor.None;
        public AntState antState = AntState.Idle;

        private PathFinding2 pathFinding;
        public Transform[] destinations;

        private void Awake()
        {
            pathFinding = GetComponent<PathFinding2>();
            pathFinding.moveSpeed = 20.0f;
            DestinationSettings();
        }

        private void Update()
        {
            switch (antState)
            {
                case AntState.Idle:
                    break;
                 case AntState.Move:
                    break;
                case AntState.Right:
                    break;
                case AntState.Left:
                    break;
                case AntState.DeadEnd:
                    break;
                 case AntState.Follow:
                    break;
                case AntState.GetEaten:
                    break;
            }
        }

        public void DestinationSettings()
        {
            RaycastHit2D hit;
            hit = Physics2D.Raycast(transform.position, Vector2.up, 10.0f, 1 << 6);

            if(hit.collider != null)
            {
                Debug.Log("Destination Set");
                pathFinding.target = hit.transform.gameObject;
            }
        }

        public void Search()
        {
            pathFinding.isWalking = false;
            pathFinding.target = null;

            RaycastHit2D hit;

            hit = Physics2D.Raycast(transform.position, Vector2.left, 1.0f, 1 << 7);
            if (hit.collider != null)
            {
                Debug.Log("Left");
                pathFinding.isWalking = true;
                transform.rotation = Quaternion.Euler(0f, 0f, 90f);
                DestinationSettings();
                return;
            }

            hit = Physics2D.Raycast(transform.position, Vector2.up, 1.0f, 1 << 7);
            if (hit.collider != null)
            {
                Debug.Log("forward");
                pathFinding.isWalking = true;
                transform.rotation = Quaternion.Euler(0f, 0f, 0f);
                DestinationSettings();
                return;
            }

            hit = Physics2D.Raycast(transform.position, Vector2.right, 1.0f, 1 << 7);
            if (hit.collider != null)
            {
                Debug.Log("right");
                pathFinding.isWalking = true;
                transform.rotation = Quaternion.Euler(0f, 0f, -90f);
                DestinationSettings();
                return;
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("FakeGoal"))
            {
                Search();
            }
        }
    }
}
