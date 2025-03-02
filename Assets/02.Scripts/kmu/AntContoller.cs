using Ant.AI;
using KMU;
using System;
using System.Collections;
using System.Collections.Generic;
using Team.manager;
using UnityEngine;
using yjlee.Ant;

namespace kmu
{
    public enum AntColor
    {
        Red,
        Green,
        Blue
    }

    public enum AntState
    {
        Idle,      // �⺻
        Move,      // �̵�
        Right,     // �����U���� �̵�
        Left,      // �������� �̵� 
        DeadEnd,   // ���ٸ���
        Search,    // Ž��
        Follow,    // ������
        GetEaten   // ��� ������
    }

    public class AntContoller : MonoBehaviour
    {
        public AntColor ?antColor = null;
        public AntState antState = AntState.Idle;

        private PathFinding pathFinding;
        private Rigidbody2D antRigidbody;
        private Collider2D antCollider;
        public Transform[] destinations;

        public GameObject beforeDestination;

        public Animator controller;
        public RuntimeAnimatorController[] antAnim;
        public SpriteRenderer antSpriteRenderer;
        public Sprite[] antSprite;

        public bool isStop = false;
        private float eatTime = 5f;


        private void Awake()
        {
            pathFinding = GetComponent<PathFinding>();
            antRigidbody = GetComponent<Rigidbody2D>();
            antCollider = GetComponent<Collider2D>();

            pathFinding.moveSpeed = 20.0f;
            DestinationSettings();

            Application.targetFrameRate = 65;
        }

        private void Update()
        {
            if (GameManager.Instance.isGameStart) // ���� ���� ��ư�� ������ ���̵��� ������.
            {
                pathFinding.isWalking = true;
                pathFinding.walkable = true;
            }

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

        public void SetAntColor(AntColor antColor)
        {
            this.antColor = antColor;
            antSpriteRenderer.sprite = antSprite[(int)antColor];
            controller.runtimeAnimatorController = antAnim[(int)antColor];
            Debug.Log(antColor);
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

        private bool IsAttackOther(kmu.AntContoller ants)
        {
            if (antColor == kmu.AntColor.Red && ants.antColor == kmu.AntColor.Green) // ���̰� �������� ��
            {
                return true;
            }
            else if (antColor == kmu.AntColor.Blue && ants.antColor == kmu.AntColor.Red) // ���̰� �Ķ����� ��
            {
                return true;
            }
            else if (antColor == kmu.AntColor.Green && ants.antColor == kmu.AntColor.Blue) // ���̰� �ʷϻ��� ��
            {
                return true;
            }
            return false;
        }

        private IEnumerator EatCoroutine(Collider2D food)
        {

            pathFinding.isEating = true; // �̵� ����
            antCollider.enabled = false; // �������� �ݶ��̴� ��Ȱ��ȭ
            yield return new WaitForSeconds(eatTime);

            pathFinding.isEating = false; // �̵�����
            antCollider.enabled = true; // �������� �ݶ��̴� Ȱ��ȭ

            Destroy(food.gameObject);

        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (!GameManager.Instance.isGameStart) return;

            if (collision.CompareTag("Ant")) // �� ����
            {
                kmu.AntContoller ant = collision.GetComponent<kmu.AntContoller>();

                // ���ݿ����� ���ٽ� �� ��
                if (IsAttackOther(ant))
                {
                    Destroy(collision.gameObject);
                }

            }
            else if (collision.CompareTag("Food")) // ���� ����
            {
                collision.enabled = false;
                StartCoroutine(EatCoroutine(collision));
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
                Goal goal = collision.GetComponent<Goal>();

                if (goal.goalColor == antColor)
                {
                    Debug.Log("Goal");
                    pathFinding.isWalking = false;
                    pathFinding.target = null;

                    antRigidbody.velocity = Vector3.zero;
                    antRigidbody.angularVelocity = 0.0f;

                    goal.antCount--;
                    goal.antCountText.text = goal.antCount.ToString();

                    Destroy(gameObject);

                    GameManager.Instance.GameClear();
                }
                else
                {
                    pathFinding.isWalking = false;
                    pathFinding.target = null;

                    antRigidbody.velocity = Vector3.zero;
                    antRigidbody.angularVelocity = 0.0f;

                    GameManager.Instance.GameOver();
                }


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
