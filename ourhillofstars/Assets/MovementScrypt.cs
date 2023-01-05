using UnityEngine;

public class MovementScrypt : MonoBehaviour
{
    private Renderer r;
    // Start is called before the first frame update
    private void Start()
    {
        r = GetComponent<Renderer>();
    }

    // Update is called once per frame
    private void Update()
    {
        r.material.color = Color.yellow;

        if (Input.GetKey("a"))
        {
            r.material.color = Color.red;
        }
    }
}
