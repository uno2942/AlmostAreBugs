﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    private GameObject mainCamera;
    // Start is called before the first frame update
    void Start()
    {
        mainCamera = GameObject.Find( "Main Camera" );
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
