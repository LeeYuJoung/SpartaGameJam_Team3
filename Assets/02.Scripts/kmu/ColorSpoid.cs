using UnityEngine;
using UnityEngine.UI;

namespace KMU
{
    public class ColorSpoid : MonoBehaviour
    {
        [SerializeField] AntsMove[] antsMove; // 스테이지의 모든 개미
        [SerializeField] private Material[] colors; // 0 = Red, 1 = Blue, 2 = Green
        [SerializeField] private Button[] spoidButtons; // 스포이드 버튼 배열

        [SerializeField] private Button gameStartButton; // 시작 버튼
        [SerializeField] private Button reStartButton; // 재시작 버튼

        [SerializeField] private GameObject shadowSpoid; // 스포이드 그림자

        private AntColor selectedColor; // 선택된 색상
        private bool isSpoidOn = false;

        private Animator spoidAnim;

        private void Start()
        {
            spoidAnim = GetComponent<Animator>();

            // 스포이드 버튼에 색상 저장 이벤트 등록
            for (int i = 0; i < spoidButtons.Length; i++)
            {
                int index = i; 
                spoidButtons[i].onClick.AddListener(() => OnClickSpoid(index));
            }

            shadowSpoid.SetActive(false);

            spoidAnim.SetTrigger("DoActive"); // 스포이드 등장 애니메이션
        }

        private void Update()
        {
            SpoidControl();
        }

        private void SpoidControl()
        {
            Vector3 mousePosition = Input.mousePosition;

            if (isSpoidOn)
            {
                // 마우스 위치를 따라다니기
                mousePosition.z = 10f;
                shadowSpoid.transform.position = Camera.main.ScreenToWorldPoint(mousePosition);
            }

            // 마우스를 클릭하면 개미의 색상을 변경
            if (Input.GetMouseButtonDown(0) && isSpoidOn)
            {
                Vector2 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                RaycastHit2D hit = Physics2D.Raycast(worldPoint, Vector2.zero);

                if (hit.collider != null)
                {
                    AntsMove ant = hit.collider.GetComponent<AntsMove>();

                    if (ant != null)
                    {
                        Renderer antColor = ant.antscarf.GetComponent<Renderer>(); // 스카프 색상

                        ant.SetAntColor(selectedColor);
                        antColor.material = colors[(int)selectedColor];

                        CheckGameStart(); // 게임 시작 조건 확인
                    }
                }

                // 스포이드 초기화
                isSpoidOn = false;
                shadowSpoid.SetActive(false);
            }
        }

        // 게임 스타트 버튼
        public void CheckGameStart()
        {
            // 하나라도 색상 설정이 안되면 return
            foreach (AntsMove ants in antsMove)
            {
                if (ants.antColor == null) return;
            }

            gameStartButton.gameObject.SetActive(true);
        }

        public void OnClickGameStart()
        {
            // 게임 시작
            foreach (AntsMove ants in antsMove)
            {
                ants.isGameStart = true;
            }

            spoidAnim.SetTrigger("DoInActive"); // 스포이드 퇴장 애니메이션

            gameStartButton.gameObject.SetActive(false);
            reStartButton.gameObject.SetActive(true);

        }


        // 스포이드 색상 적용
        private void OnClickSpoid(int colorIndex)
        {
            isSpoidOn = true;
            shadowSpoid.SetActive(true);
            selectedColor = (AntColor)colorIndex; // 선택된 색상 저장

            // 머티리얼 색상 변경
            if (shadowSpoid.TryGetComponent<Renderer>(out Renderer renderer))
            {
                renderer.material = colors[colorIndex]; // 선택한 색상의 머티리얼 적용
            }
        }
    }
}
