using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class PlayManager : MonoBehaviour
{
    // Start is called before the first frame update
    public LevelManager Manager;
    public Button MenuButton;

    void Start()
    {
        Manager = GameObject.Find("Level Manager").GetComponent<LevelManager>();
        MenuButton.onClick.AddListener(() => Manager?.OnMenu());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
