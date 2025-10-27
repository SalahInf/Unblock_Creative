using UnityEngine;

public class HandFollowCursor : MonoBehaviour
{
    public RectTransform handUI; // Assign the hand UI RectTransform
    [SerializeField] Animation handAnimation; // Animator for the hand animations
    public Canvas canvas; // Assign the Canvas
    [SerializeField] private float animSpeed=3f;


    void Start()
    {
        if(Application.isEditor is false)
            Destroy(gameObject);

        if (handUI == null)
        {
            Debug.LogError("Hand UI RectTransform is not assigned.");
            return;
        }
        handAnimation["HandTap"].speed = animSpeed;
        handUI.gameObject.SetActive(true);
    }

    void Update()
    {
        // Get the mouse position in screen space
        Vector2 mousePosition = Input.mousePosition;

        // Clamp the mouse position to stay within the canvas
        RectTransform canvasRect = canvas.GetComponent<RectTransform>();
        Vector2 clampedPosition = ClampToCanvas(mousePosition, canvasRect);

        // Convert the clamped screen position to local position
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,
            clampedPosition,
            canvas.worldCamera,
            out Vector2 localPosition);

        // Set the hand UI's position
        handUI.anchoredPosition = localPosition;

        // Play animations based on mouse input
        if (Input.GetMouseButtonDown(0))
        {
            handAnimation.Play("HandTap");
        }
    }

    [SerializeField] private float minVal=2;
    [SerializeField] private float maxVal=2;
    private Vector2 ClampToCanvas(Vector2 screenPosition, RectTransform canvasRect)
    {
        // Convert canvasRect.position (Vector3) to Vector2 for 2D operations
        Vector2 canvasPosition = new Vector2(canvasRect.position.x, canvasRect.position.y);

        // Calculate min and max boundaries
        Vector2 min = canvasPosition - (canvasRect.rect.size / minVal) * canvas.scaleFactor;
        Vector2 max = canvasPosition + (canvasRect.rect.size / minVal) * canvas.scaleFactor;

        // Clamp the screen position
        screenPosition.x = Mathf.Clamp(screenPosition.x, min.x, max.x);
        screenPosition.y = Mathf.Clamp(screenPosition.y, min.y, max.y);

        return screenPosition;
    }

}