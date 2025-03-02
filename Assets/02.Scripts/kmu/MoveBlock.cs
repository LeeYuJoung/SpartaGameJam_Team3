using System.Collections;
using UnityEngine;

namespace KMU
{
    public class MoveBlock : MonoBehaviour
    {
        [SerializeField] private GameObject moveBlock; // ������ �� �ִ� ���
        [SerializeField] private Ant.AI.Grid grid;

        private Vector3 originalScale; // ���� ����� ũ��
        private Vector3 zeroScale; // 0�� �� ����� ũ��
        private bool isSizeDown = false; // ���� �پ��� ������ Ȯ��

        // Block �̵��� ������ ���̾�
        public LayerMask blockedLayer;
        public LayerMask unBlockedLayer;

        // ������ ������ ��
        public int antCount = 0;

        // �� �̵� ���� �ð�
        private float duration = 1f;

        // �ڷ�ƾ �ߺ� ���� ����
        private Coroutine sizeCoroutine = null;

        private void Start()
        {
            blockedLayer = LayerMask.NameToLayer("Block");
            unBlockedLayer = LayerMask.NameToLayer("Default");

            originalScale = moveBlock.transform.localScale; // ��� �ʱ� ũ��
            zeroScale = new Vector3(moveBlock.transform.localScale.x, 0, moveBlock.transform.localScale.z); // ��� 0ũ��
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Ant"))
            {
                antCount++;
                if (sizeCoroutine == null || !isSizeDown) // �پ��� ���� �ƴ϶�� ����
                {
                    if (sizeCoroutine != null) StopCoroutine(sizeCoroutine); // ���� �ڷ�ƾ ���� 
                    sizeCoroutine = StartCoroutine(BlockSizeDown());
                }
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.CompareTag("Ant"))
            {
                antCount--;
                if ((sizeCoroutine == null || isSizeDown) &&antCount <=0) // Ŀ���� ���� �ƴ϶�� ����
                {
                    if (sizeCoroutine != null) StopCoroutine(sizeCoroutine);
                    sizeCoroutine = StartCoroutine(BlockSizeUp());
                }
            }
        }

        // ��� ũ�� => 0
        private IEnumerator BlockSizeDown()
        {
            isSizeDown = true;
            float elapsedTime = 0f;
            Vector3 startScale = moveBlock.transform.localScale; // ���� ũ�⿡�� ����
            moveBlock.layer = unBlockedLayer; // �浹���� �ʴ� ���̾�� �ٲ���

            while (elapsedTime < duration)
            {
                moveBlock.transform.localScale = Vector3.Lerp(startScale, zeroScale, elapsedTime / duration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            moveBlock.transform.localScale = zeroScale;

            grid.UpdateGrid(); // �׸��� ������Ʈ
            sizeCoroutine = null;
        }

        // ��� ũ�� => �������
        private IEnumerator BlockSizeUp()
        {
            isSizeDown = false;
            float elapsedTime = 0f;
            Vector3 startScale = moveBlock.transform.localScale; // ���� ũ�⿡�� ����
            moveBlock.layer = blockedLayer; // �浹�ϴ� ���̾�� �ٲ���

            while (elapsedTime < duration)
            {
                moveBlock.transform.localScale = Vector3.Lerp(startScale, originalScale, elapsedTime / duration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            moveBlock.transform.localScale = originalScale;

            grid.UpdateGrid(); // �׸��� ������Ʈ
            sizeCoroutine = null;
        }
    }
}
