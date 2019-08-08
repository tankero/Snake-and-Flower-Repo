using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{

    public Button PlayButton;
    public Button ScoreButton;
    public Button QuitButton;

    
    // Start is called before the first frame update
    void Start()
    {
        LevelManager Manager = GameObject.Find("Level Manager").GetComponent<LevelManager>();
        PlayButton.onClick.AddListener(() => Manager.OnPlay());

        QuitButton.onClick.AddListener(() => Manager.OnQuit());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
