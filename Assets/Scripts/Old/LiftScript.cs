using UnityEngine;

public class LiftScript : MonoBehaviour
{
    [SerializeField]private float speed = 10.0f;
    [SerializeField] private float height = 100.0f;
    [SerializeField] private float step = 0;
    private Vector3 startPosition;
    private Vector3 targetPosition;
    private bool moveToUp = true;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startPosition = this.transform.position;
        targetPosition = startPosition + Vector3.up * height;
    }

    // Update is called once per frame
    void Update()
    {
        if(step > height) { moveToUp = false; }
        if(step < 0) { moveToUp = true; }

        if(moveToUp)//ã¸
        {
            step += speed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(startPosition, targetPosition, step);
        }
        else//‰º~
        {
            step -= speed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(startPosition, targetPosition, step);
        }
    }
}
