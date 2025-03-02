using System.Collections;
using UnityEngine;

namespace KMU
{
    public class MoveBlock : MonoBehaviour
    {
        [SerializeField] private GameObject moveBlock; // 움직일 수 있는 블록
        [SerializeField] private Ant.AI.Grid grid;

        private Vector3 originalScale; // 원래 블록의 크기
        private Vector3 zeroScale; // 0이 된 블록의 크기
        private bool isSizeDown = false; // 현재 줄어드는 중인지 확인

        // Block 이동시 변경할 레이어
        public LayerMask blockedLayer;
        public LayerMask unBlockedLayer;

        // 추적할 개미의 수
        public int antCount = 0;

        // 벽 이동 지속 시간
        private float duration = 1f;

        // 코루틴 중복 실행 방지
        private Coroutine sizeCoroutine = null;

        private void Start()
        {
            blockedLayer = LayerMask.NameToLayer("Block");
            unBlockedLayer = LayerMask.NameToLayer("Default");

            originalScale = moveBlock.transform.localScale; // 블록 초기 크기
            zeroScale = new Vector3(moveBlock.transform.localScale.x, 0, moveBlock.transform.localScale.z); // 블록 0크기
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Ant"))
            {
                antCount++;
                if (sizeCoroutine == null || !isSizeDown) // 줄어드는 중이 아니라면 실행
                {
                    if (sizeCoroutine != null) StopCoroutine(sizeCoroutine); // 기존 코루틴 종료 
                    sizeCoroutine = StartCoroutine(BlockSizeDown());
                }
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.CompareTag("Ant"))
            {
                antCount--;
                if ((sizeCoroutine == null || isSizeDown) &&antCount <=0) // 커지는 중이 아니라면 실행
                {
                    if (sizeCoroutine != null) StopCoroutine(sizeCoroutine);
                    sizeCoroutine = StartCoroutine(BlockSizeUp());
                }
            }
        }

        // 블록 크기 => 0
        private IEnumerator BlockSizeDown()
        {
            isSizeDown = true;
            float elapsedTime = 0f;
            Vector3 startScale = moveBlock.transform.localScale; // 현재 크기에서 시작
            moveBlock.layer = unBlockedLayer; // 충돌하지 않는 레이어로 바꿔줌

            while (elapsedTime < duration)
            {
                moveBlock.transform.localScale = Vector3.Lerp(startScale, zeroScale, elapsedTime / duration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            moveBlock.transform.localScale = zeroScale;

            grid.UpdateGrid(); // 그리드 업데이트
            sizeCoroutine = null;
        }

        // 블록 크기 => 원래대로
        private IEnumerator BlockSizeUp()
        {
            isSizeDown = false;
            float elapsedTime = 0f;
            Vector3 startScale = moveBlock.transform.localScale; // 현재 크기에서 시작
            moveBlock.layer = blockedLayer; // 충돌하는 레이어로 바꿔줌

            while (elapsedTime < duration)
            {
                moveBlock.transform.localScale = Vector3.Lerp(startScale, originalScale, elapsedTime / duration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            moveBlock.transform.localScale = originalScale;

            grid.UpdateGrid(); // 그리드 업데이트
            sizeCoroutine = null;
        }
    }
}
