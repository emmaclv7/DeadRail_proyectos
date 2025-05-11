using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PointHandler : MonoBehaviour
{
   

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePostion = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePostion, Vector2.zero);
            if (hit.collider != null)
            {
                Debug.Log("hit");
                ExecuteClick();
            }
        }
    
    }

    private void ExecuteClick () {
        if (tag == "Start")
        {
            SceneManager.LoadScene("MainGame");
        }
        else if (tag == "MainMenu")
        {
            SceneManager.LoadScene("MainMenu");
        }
        else if (tag == "Exit") { Application.Quit(); }
        else {}
    }
}
