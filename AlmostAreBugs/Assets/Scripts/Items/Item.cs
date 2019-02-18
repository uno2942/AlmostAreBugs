﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{

    public ItemManager.PresentState presentState;
    public ItemManager.ItemList item;
    public delegate void ClickEventHandler( ItemManager.ItemList item, ItemManager.PresentState presentState, GameObject gObject );
    public event ClickEventHandler ClickEvent;
    // Start is called before the first frame update
    protected virtual void Start() 
    {
        presentState = ItemManager.PresentState.Default;
        ClickEvent += ScriptWindow.ScriptWindowInstance.ScriptPrinterForClickItem;
        //ClickEvent에 Subscriber를 붙여줍시다.
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        
    }

    public virtual void Clicked() {
        ClickEventHandlerInvoker( item, presentState, gameObject );
    }
    public virtual void ImageChange( ItemManager.ItemList item, ItemManager.PresentState presentState, GameObject gObject) {
    }

    protected bool ClickEventHandlerInvoker( ItemManager.ItemList item, ItemManager.PresentState presentState, GameObject gObject ) {
        if( GameManager.GameManagerInstance.IsWating ) {
            if( presentState == ItemManager.PresentState.Gotten )
                GameManager.GameManagerInstance.CollectableItemChecked( item, presentState, gameObject );
        return false;    
        }
        ClickEvent?.Invoke( item, presentState, gObject );
        return true;
    }

    protected virtual void ClickEventHandlerReset() {
        ClickEvent = null;
        ClickEvent += ScriptWindow.ScriptWindowInstance.ScriptPrinterForClickItem;
    }
}