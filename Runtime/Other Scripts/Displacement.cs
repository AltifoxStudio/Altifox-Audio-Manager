using UnityEngine;

public class Displacement : MonoBehaviour
{
    public float minXValue = 0;
    public float maxXValue = 400;

    public float speedFactor = 0.05f;

    private float angle = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float sinePost = (Mathf.Sin(angle) + 1f)/2;
        //Debug.Log(sinePost);
        Vector3 newPosition = transform.position;
        newPosition.x = Mathf.Lerp(minXValue, maxXValue, sinePost);
        newPosition.y = 0f;
        newPosition.z = 0f;
        transform.localPosition = newPosition;
        angle += speedFactor;
    }
}
