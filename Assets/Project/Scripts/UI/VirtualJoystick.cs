using UnityEngine;
using UnityEngine.EventSystems;

public class VirtualJoystick : MonoBehaviour,
IPointerDownHandler,
IDragHandler,
IPointerUpHandler
{
    
    [SerializeField] private RectTransform _handle;
    [SerializeField] private float _maxRadius = 100f;
    public Vector2 inputVector;
        
    private RectTransform _baseRect;

    private void Awake()
    {
        _baseRect = GetComponent<RectTransform>();
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        //Debug.Log("POINTER DOWN");
        OnDrag(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        //Debug.Log("DRAG");
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            _baseRect,
            eventData.position,
            null,
            out localPoint
            
        );

        localPoint = Vector2.ClampMagnitude(localPoint, _maxRadius);

        _handle.anchoredPosition = localPoint;
        inputVector = localPoint / _maxRadius;
        //Debug.Log(inputVector);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        _handle.anchoredPosition = Vector2.zero;
        inputVector = Vector2.zero;
    }

}
