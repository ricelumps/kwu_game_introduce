using UnityEngine;
using UnityEngine.InputSystem;

public class Dongle : MonoBehaviour
{
    RectTransform rect;
    Canvas canvas;

    public bool isDrag;
    Rigidbody2D rigid;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        rect = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
    }

    void Update()
    {
        if (isDrag)
        {
            if (Mouse.current == null)
            {
                return;
            }

            Vector2 localPos;

            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvas.transform as RectTransform,
                Mouse.current.position.ReadValue(),
                null,
                out localPos
            );

            // 캔버스 절반 너비
            float canvasHalfWidth = ((canvas.transform as RectTransform).rect.width / 2f) - 50;

            // 자기 자신의 절반 너비
            float halfWidth = rect.rect.width / 2f;

            // 이동 제한
            float leftBorder = -canvasHalfWidth + halfWidth;
            float rightBorder = canvasHalfWidth - halfWidth;

            localPos.x = Mathf.Clamp(localPos.x, leftBorder, rightBorder);
            localPos.y = 445;

            rect.anchoredPosition = Vector2.Lerp(rect.anchoredPosition, localPos, 0.2f);
        }

        
    }

    public void Drag()
    {
        isDrag = true;
    }

    public void Drop()
    {
        isDrag = false;
        rigid.simulated = true;
    }
}
