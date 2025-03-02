using Ant.AI;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.Searcher.SearcherWindow.Alignment;
using static UnityEditor.ShaderGraph.Internal.KeywordDependentCollection;

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
        private Rigidbody2D antRigidbody;
        public Transform[] destinations;

        public GameObject beforeDestination;

        public bool isStop = false;

        private void Awake()
        {
            pathFinding = GetComponent<PathFinding2>();
            antRigidbody = GetComponent<Rigidbody2D>();

            pathFinding.moveSpeed = 20.0f;
            DestinationSettings();
        }

        private void Update()
        {
            Debug.DrawRay(transform.position, transform.up * 10.0f, Color.yellow);
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
            hit = Physics2D.Raycast(transform.position, transform.up, 10.0f, 1 << 6);

            if (hit.collider != null)
            {
                Debug.Log(hit.collider.name);
                pathFinding.target = hit.transform.gameObject;
                pathFinding.isWalking = true;
            }
        }

        public void Search()
        {
            pathFinding.isWalking = false;
            pathFinding.target = null;

            antRigidbody.velocity = Vector3.zero;
            antRigidbody.angularVelocity = 0.0f;

            RaycastHit2D hit;

            hit = Physics2D.Raycast(transform.position, -transform.right, 1.0f, 1 << 7);
            if (hit.collider != null)
            {
                Debug.Log("Left");
                transform.Rotate(0, 0, CheckDir(hit.transform));
                //transform.rotation = Quaternion.Euler(0f, 0f, CheckDir(hit.transform));
                return;
            }

            hit = Physics2D.Raycast(transform.position, transform.up, 1.0f, 1 << 7);
            if (hit.collider != null)
            {
                Debug.Log("forward");
                //transform.Rotate(0, 0, CheckDir(hit.transform));
                //transform.rotation = Quaternion.Euler(0f, 0f, CheckDir(hit.transform));
                return;
            }

            hit = Physics2D.Raycast(transform.position, transform.right, 1.0f, 1 << 7);
            if (hit.collider != null)
            {
                Debug.Log("right");
                transform.Rotate(0, 0, CheckDir(hit.transform));
                //transform.rotation = Quaternion.Euler(0f, 0f, CheckDir(hit.transform));
                return;
            }

            pathFinding.isWalking = false;
            pathFinding.target = null;

            antRigidbody.velocity = Vector3.zero;
            antRigidbody.angularVelocity = 0.0f;
        }

        public float CheckDir(Transform target)
        {
            Vector2 dir = transform.position - target.position;
            dir = dir.normalized;
            Debug.Log(dir);

            if(dir.x > 0.5f)
            {
                Debug.Log("Dir X Left");
                if (dir.y < 0.0f)
                    return 90.0f;
                else
                    return -90.0f;
            }
            else if(dir.x < -0.5f)
            {
                Debug.Log("Dir X Right");
                if (dir.y < 0.0f)
                    return -90.0f;
                else
                    return 90.0f;
            }
            else if (dir.y > 0.5f)
            {
                Debug.Log("Dir Y Left");
                if (dir.x > 0.0f)
                    return 90.0f;
                else
                    return -90.0f;
            }
            else if (dir.y < -0.5f)
            {
                Debug.Log("Dir Y Right");
                if (dir.x > 0.0f)
                    return -90.0f;
                else
                    return 90.0f;
            }
            else
            {
                Debug.Log("Dir Forward");
                return 0;
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Stop"))
            {
                Debug.Log("Stop Hit");
                Search();

                if (beforeDestination != null)
                {
                    beforeDestination.SetActive(true);
                }

                beforeDestination = collision.transform.parent.gameObject;
                beforeDestination.SetActive(false);
                DestinationSettings();
            }
            else if (collision.CompareTag("Goal"))
            {
                Debug.Log("Goal");
                pathFinding.isWalking = false;
                pathFinding.target = null;

                antRigidbody.velocity = Vector3.zero;
                antRigidbody.angularVelocity = 0.0f;
            }
        }
    }
}
