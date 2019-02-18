﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Quilt : CollectableItem {
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        ClickEvent += Inventory.InventoryInstance.AddItem;
        // ClickEvent += ImageChange;
    }

    // Update is called once per frame
    protected override void Update()
    {
        
    }

    public override void Clicked() {
        if( ClickEventHandlerInvoker( item, presentState, gameObject ) ) {
            switch( presentState ) {
            case ItemManager.PresentState.Dropped:
                presentState = ItemManager.PresentState.Gotten;
                ClickEventHandlerReset();
                ClickEvent += UiManager.UiManagerInstance.OpenMessageBox;
                return;
            case ItemManager.PresentState.Gotten:
                break;
            }
        }
    }
}
