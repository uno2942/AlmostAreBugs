﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : Item {
    private bool isOpenable;
    private bool isOpened=false;
    public bool IsOpenable { get => isOpenable; }

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        isOpenable = false;
    }

    // Update is called once per frame
    protected override void Update()
    {
        
    }

    public override void Clicked() {
        if( ClickEventHandlerInvoker( item, presentState, gameObject ) ) {
            //if() 
            {
            if( isOpened ) {
                //OpenEvent
                gameObject.GetComponent<UnityEngine.UI.Image>().sprite = Resources.Load<Sprite>( "Image/door_closed" );
                isOpened = false;
            } else {
                gameObject.GetComponent<UnityEngine.UI.Image>().sprite = Resources.Load<Sprite>( "Image/door_opened" );
                isOpened = true;
                }
            }
        }
    }

    public void OpenableTheDoor() {
        isOpenable = true;
    }
}
