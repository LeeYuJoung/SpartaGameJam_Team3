using Ant.AI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using yjlee.Ant;

namespace kmu
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

    public class AntContoller : MonoBehaviour
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

            Application.targetFrameRate = 65;
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

            hit = Physics2D.Raycast(transform.position, transform.right, 1.5f, 1 << 7);
            if (hit.collider != null)
            {
                Debug.Log("Right");
                transform.Rotate(new Vector3(0.0f, 0.0f, CheckDir(hit.transform)));
                //transform.rotation = Quaternion.Euler(0.0f, 0.0f, CheckDir(hit.transform));
                return;
            }

            hit = Physics2D.Raycast(transform.position, transform.up, 1.5f, 1 << 7);
            if (hit.collider != null)
            {
                Debug.Log("Forward");
                return;
            }

            hit = Physics2D.Raycast(transform.position, -transform.right, 1.5f, 1 << 7);
            if (hit.collider != null)
            {
                Debug.Log("Left");
                transform.Rotate(new Vector3(0.0f, 0.0f, CheckDir(hit.transform)));
                //transform.rotation = Quaternion.Euler(0.0f, 0.0f, CheckDir(hit.transform));
                return;
            }

            Debug.Log("None");
            pathFinding.isWalking = false;
            pathFinding.target = null;

            antRigidbody.velocity = Vector3.zero;
            antRigidbody.angularVelocity = 0.0f;
        }

        public float CheckDir(Transform target)
        {
            Vector2 dir = transform.position - target.position;
            Debug.Log(dir);

            Debug.Log(transform.rotation.eulerAngles.y);
            if (transform.rotation.eulerAngles.y == 180)
            {
                if (dir.x > 0.0f)
                {
                    Debug.Log("Dir Right");
                    if (dir.y < 0.2f)
                        return 90.0f;
                    else
                        return -90.0f;
                }
                else if (dir.x < 0.0f)
                {
                    Debug.Log("Dir Left");
                    if (dir.y < 0.2f)
                        return 90.0f;
                    else
                        return -90.0f;
                }
                else
                {
                    Debug.Log("Dir Forward");
                    return 0;
                }
            }
            else
            {
                if (dir.x > 0.0f)
                {
                    Debug.Log("Dir Right");
                    if (dir.y < 0.0f)
                        return -90.0f;
                    else
                        return 90.0f;
                }
                else if (dir.x < 0.0f)
                {
                    Debug.Log("Dir Left");
                    if (dir.y < 0.0f)
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
        }

        public void CheckGoal()
        {

        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Ant"))
            {
                pathFinding.isWalking = false;
                pathFinding.target = null;

                antRigidbody.velocity = Vector3.zero;
                antRigidbody.angularVelocity = 0.0f;
            }
            else if (collision.CompareTag("Stop"))
            {
                Debug.Log("Stop Hit");
                Search();

                if (beforeDestination != null)
                    beforeDestination.SetActive(true);

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

        //private void OnTriggerExit2D(Collider2D collision)
        //{
        //    if (collision.CompareTag("Stop"))
        //    {
        //        if (beforeDestination != null)
        //            beforeDestination.SetActive(true);
        //    }
        //}
    }
}
