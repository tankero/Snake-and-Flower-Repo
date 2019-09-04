using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{

    public Button PlayButton;
    public Button ScoreButton;
    public Button CreditsButton;


    // Start is called before the first frame update
    void Start()
    {
        LevelManager Manager = GameObject.Find("Level Manager").GetComponent<LevelManager>();
        PlayButton.onClick.AddListener(() => Manager.OnPlay());

        CreditsButton.onClick.AddListener(() => Manager.OnCredits());

    }


}
