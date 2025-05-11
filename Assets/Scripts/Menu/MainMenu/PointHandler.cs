using UnityEngine;
using UnityEngine.SceneManagement;

public class PointHandler : MonoBehaviour
{
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);

            if (hit.collider != null)
            {
                ExecuteClick(hit.collider.gameObject.tag);
            }
        }
    }

    private void ExecuteClick(string objectTag)
    {
        switch (objectTag)
        {
            case "Start":
                SceneManager.LoadScene("MainGame");
                break;

            case "MainMenu":
                SceneManager.LoadScene("MainMenu");
                break;

            case "Exit":
                Application.Quit();
                break;

            default:
                Debug.Log("Unknown Tag" + objectTag);
                break;
        }
    }
}
