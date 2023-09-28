using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartWindow : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Bird.GetInstance().OnStarted += StartWindow_OnStarted;
    }

    private void StartWindow_OnStarted(object sender, System.EventArgs e) {
        Hide();
    }

    private void Hide() {
        gameObject.SetActive(false);
    }

    private void Show() {
        gameObject.SetActive(true);
    }
}
