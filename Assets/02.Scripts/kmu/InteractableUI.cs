using kmu;
using System.Runtime.InteropServices.WindowsRuntime;
using Team.manager;
using UnityEngine;
using UnityEngine.UI;

namespace KMU
{
    public class InteractableUI : MonoBehaviour
    {
        [SerializeField] kmu.AntContoller[] antController;
        [SerializeField] private Material[] colors; // 0 = Red, 1 = Blue, 2 = Green
        [SerializeField] private Button[] spoidButtons; // 스포이드 버튼 배열

        [SerializeField] private Button gameStartButton; // 시작 버튼
        [SerializeField] private Button reStartButton; // 재시작 버튼

        [SerializeField] private GameObject shadowSpoid; // 스포이드 그림자

        [SerializeField] private Button foodButton;
        [SerializeField] private GameObject foodPrefab;
        [SerializeField] private GameObject shadowFood;
        public int foodCount = 0;
        private bool isFoodOn = false;

        private kmu.AntColor selectedColor; // 선택된 색상
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

            foodButton.onClick.AddListener(OnClickFood);

            shadowSpoid.SetActive(false);
            shadowFood.SetActive(false);

            spoidAnim.SetTrigger("DoActive"); // 스포이드 등장 애니메이션
        }

        private void Update()
        {
            UIObjectControl();
        }

        private void UIObjectControl()
        {
            Vector3 mousePosition = Input.mousePosition;

            if (isSpoidOn)
            {
                // 마우스 위치를 따라다니기
                mousePosition.z = 10f;
                shadowSpoid.transform.position = Camera.main.ScreenToWorldPoint(mousePosition);
            }
            else if (isFoodOn)
            {
                mousePosition.z = 10f;
                shadowFood.transform.position = Camera.main.ScreenToWorldPoint(mousePosition);
            }

            // 마우스를 클릭하면 개미의 색상을 변경
            if (Input.GetMouseButtonDown(0) && isSpoidOn)
            {
                Vector2 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                RaycastHit2D hit = Physics2D.Raycast(worldPoint, Vector2.zero);

                if (hit.collider != null)
                {
                    kmu.AntContoller ant = hit.collider.GetComponent<AntContoller>();

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

            else if (Input.GetMouseButtonDown(0) && isFoodOn) // 먹이 놓기
            {
                Vector2 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                RaycastHit2D hit = Physics2D.Raycast(worldPoint, Vector2.zero);

                Ant.AI.Node clickedNode = FindObjectOfType<Ant.AI.Grid>().NodeFromWorldPoint(worldPoint);
                
                if (clickedNode != null) // 그리드의 노드가 있는 곳에만
                {
                    if (!clickedNode.isWalkable || hit.collider != null) // 노드가 이동 가능하고 다른 콜라이더가 없는 곳에만
                    {
                        isFoodOn = false;
                        shadowFood.SetActive(false);
                        return;
                    }

                    Instantiate(foodPrefab, clickedNode.worldPosition, Quaternion.identity);
                    foodCount--;
                }

                isFoodOn = false;
                shadowFood.SetActive(false);
            }
        }

        // 게임 스타트 버튼
        public void CheckGameStart()
        {
            // 하나라도 색상 설정이 안되면 return
            foreach (kmu.AntContoller ants in antController)
            {
                if (ants.antColor == null) return;
            }

            gameStartButton.gameObject.SetActive(true);
                      
        }

        public void OnClickGameStart()
        {
            GameManager.Instance.isGameStart = true;

            spoidAnim.SetTrigger("DoInActive"); // 스포이드 퇴장 애니메이션

            gameStartButton.gameObject.SetActive(false);
            reStartButton.gameObject.SetActive(true);

        }


        // 스포이드 색상 적용
        private void OnClickSpoid(int colorIndex)
        {
            isSpoidOn = true;
            shadowSpoid.SetActive(true);
            selectedColor = (kmu.AntColor)colorIndex; // 선택된 색상 저장

            // 머티리얼 색상 변경
            if (shadowSpoid.TryGetComponent<Renderer>(out Renderer renderer))
            {
                renderer.material = colors[colorIndex]; // 선택한 색상의 머티리얼 적용
            }
        }

        public void OnClickFood()
        {
            if (foodCount == 0) return;
            isFoodOn = true;
            shadowFood.SetActive(true);
        }
    }
}
