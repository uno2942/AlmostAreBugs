﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class UiManager : MonoBehaviour
{
    private static UiManager uiManager;
    private static bool mShuttingDown = false;
    private static object mLock = new object();

    private HorizontalLayoutGroup itemPanel;
    private GameObject select;
    private bool isSelectBoxOn;
    private int pivot = 0;
    private Button[] buttons;
    private RectTransform originalImageTrans;
    private Image itemImage;
    private TextMeshProUGUI itemDescription;
    private TextMeshProUGUI itemName;
    public GameObject[] itemPanelBoxes;
    public static UiManager UiManagerInstance {
        get
        {
            if( mShuttingDown ) {
                Debug.LogWarning( "UiManager is already destroyed." );
                return null;
            }
            lock( mLock ) {
                if( uiManager == null ) {
                    uiManager = (UiManager) FindObjectOfType<UiManager>();
                    if( uiManager == null ) {
                        Debug.LogWarning( "UiManager gameObject does not exists." );
                        return null;
                    }
                }
                return uiManager;
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
    //    dialogWindow = GameObject.Find( "DialogWindow" ).GetComponent<DialogWindow>();
        itemPanel = GameObject.Find( "ItemPanel" ).GetComponent<HorizontalLayoutGroup>();
        select = GameObject.Find( "select" );
        foreach(var comps in select.GetComponentsInChildren<Image>()) {
            if( comps.transform.name == "ItemImage" ) {
                itemImage = comps;
            } else if( comps.transform.name == "OriginItemImage" )
                originalImageTrans = (RectTransform) comps.transform;
        }
        foreach( var comps in select.GetComponentsInChildren<TextMeshProUGUI>() ) {
            if( comps.transform.name == "ItemDescription" ) {
                itemDescription = comps;
            } else if( comps.transform.name == "ItemName" )
                itemName = comps;
        }
        select.SetActive( false );
        isSelectBoxOn = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OpenMessageBox( ItemManager.ItemList item, ItemManager.PresentState presentState, GameObject gObject ) {
        if( !isSelectBoxOn ) {
            isSelectBoxOn = true;
            select.SetActive( true );
            buttons = select.GetComponentsInChildren<Button>();
            //temporary code
            buttons[ 0 ].onClick.AddListener( gObject.GetComponent<CollectableItem>().Use );
            buttons[ 0 ].onClick.AddListener( GameManager.GameManagerInstance.WaitForAnotherItemForUse );
            buttons[ 1 ].onClick.AddListener( gObject.GetComponent<CollectableItem>().Mix );
            buttons[ 1 ].onClick.AddListener( GameManager.GameManagerInstance.WaitForAnotherItemForMix );
            buttons[ 2 ].onClick.AddListener( gObject.GetComponent<CollectableItem>().Cancel );
            foreach( var button in buttons ) {
                button.onClick.AddListener( GameManager.GameManagerInstance.ButtonSelected );
                button.onClick.AddListener( CloseMessageBox );
            }
            ( (RectTransform) itemImage.transform ).position = originalImageTrans.position;
            ( (RectTransform) itemImage.transform ).sizeDelta = originalImageTrans.sizeDelta;
            if(item == ItemManager.ItemList.Paper) {
                BugManager.BugManagerInstance.BugOccured( BugManager.BugList.Paper );
                ( (RectTransform) itemImage.transform ).localPosition = new Vector3( -63.8f, -18.2f, 0 );
                ( (RectTransform) itemImage.transform ).sizeDelta = new Vector2( 660.4f, 522.6f );
            }
            itemImage.sprite = gObject.GetComponent<Image>().sprite;
            itemDescription.text = ScriptWindow.ScriptWindowInstance.ItemDescriptionForCheckedItem(item);
            itemName.text = ScriptWindow.ScriptWindowInstance.ItemNameForCheckedItem( item );
            GameManager.GameManagerInstance.WaitForButtonSelect();
        }
    }

    public void CloseMessageBox() {
        if( isSelectBoxOn ) {
            isSelectBoxOn = false;
            select.SetActive( false );
            foreach( var button in buttons )
                button.onClick.RemoveAllListeners();
        }
    }

    public void AddItem( bool CheckItem, ItemManager.ItemList itemList, GameObject gObject ) {
        if( ItemManager.ItemList.Pillow == itemList ) {
            GameObject instantiatedGameObject;
            if( gObject.GetComponentInParent<Canvas>().transform.name == "PlayerCanvas" )
                ( instantiatedGameObject = Instantiate( gObject, GameObject.Find( "PlayerCanvas" ).transform ) ).name = "Pillow";
            else
                ( instantiatedGameObject = Instantiate( gObject, GameObject.Find( "Canvas" ).transform ) ).name = "Pillow";
            instantiatedGameObject.GetComponentInChildren<TextMeshProUGUI>().enabled = false;
        }
        if(ItemManager.ItemList.Gun == itemList ) {
            ((RectTransform) ( Inventory.InventoryInstance.CheckItemElement( itemList ).gObject.transform )).Rotate(new Vector3(0, 0, 90f));
        }

        if( !CheckItem ) {
            foreach( var text in gObject.GetComponentsInChildren<TextMeshProUGUI>() ) {
                text.enabled = true;
            }
        } else {
            string str = Inventory.InventoryInstance.CheckItemElement( itemList ).gObject.GetComponentInChildren<TextMeshProUGUI>().text;
            str = 'x' + ( Inventory.InventoryInstance.CheckItemElement(itemList).num ).ToString();
            Inventory.InventoryInstance.CheckItemElement( itemList ).gObject.GetComponentInChildren<TextMeshProUGUI>().text = str;
            Destroy( gObject );
        }
        PanelUpdate();
    }

    public void ChangeColorOfBackground( ItemManager.ItemList item, ItemManager.PresentState presentState, GameObject gObject ) {
        int i = pivot;
        foreach( var panelBox in itemPanelBoxes ) {
            if( Inventory.InventoryInstance.ItemsInInventory[ i % 9 ].item == item ) {
                itemPanelBoxes[ i - pivot ].GetComponent<Image>().color = new Color( 0.5f, 0.5f, 0.5f );
                return;
            }
            i += 1;
        }
    }

    public void ResetTheColorOfBackGround(  ) {
        int i = 0;
        foreach( var panelBox in itemPanelBoxes ) {
                itemPanelBoxes[ i ].GetComponent<Image>().color = new Color( 1f, 1f, 1f );
            i += 1;
        }
    }

    public void PanelUpdate() {
        int i = pivot;
        foreach(var item in Inventory.InventoryInstance.ItemsInInventory ) {
            item.gObject?.SetActive( false );
        }
        foreach( var panelBox in itemPanelBoxes ) {
            if( Inventory.InventoryInstance.ItemsInInventory[ i % 9 ].gObject != null ) {
                Inventory.InventoryInstance.ItemsInInventory[ i % 9 ].gObject.SetActive( true );
                Inventory.InventoryInstance.ItemsInInventory[ i % 9 ].gObject.transform.SetParent( panelBox.transform );
                ( (RectTransform) Inventory.InventoryInstance.ItemsInInventory[ i % 9 ].gObject.transform ).localScale = new Vector3( 100f, 100f, 108f );
                ( (RectTransform) Inventory.InventoryInstance.ItemsInInventory[ i % 9 ].gObject.transform ).sizeDelta = new Vector2( 1f, 0.9f );
                Inventory.InventoryInstance.ItemsInInventory[ i % 9 ].gObject.transform.localPosition=new Vector2( 5f, 0f );
            }
            i += 1;
        }
    }

    public void Pivotplus() {
        if( !( GameManager.GameManagerInstance.IsWatingForAnotherItemForMix || GameManager.GameManagerInstance.IsWatingForAnotherItemForUse || GameManager.GameManagerInstance.IsWatingForButton ) ) {
            pivot += 1;
            if( pivot > 8 )
                pivot -= 9;
            PanelUpdate();
        }
    }

    public void Pivotminus() {
        if( !( GameManager.GameManagerInstance.IsWatingForAnotherItemForMix || GameManager.GameManagerInstance.IsWatingForAnotherItemForUse || GameManager.GameManagerInstance.IsWatingForButton ) ) {
            pivot -= 1;
            if( pivot < 0 )
                pivot += 9;
            PanelUpdate();
        }
    }

    public void RemoveItem( ItemManager.ItemList itemList, GameObject gObject ) {
        string str = gObject.GetComponentInChildren<TextMeshProUGUI>().text;
        Destroy( gObject );
        /*
        if( int.Parse( str.Remove( 0, 1 ).ToString() ) <= 1 ) {
            str = "x0";
            gObject.GetComponentInChildren<TextMeshProUGUI>().text = str;
            
        } else {
            str = 'x' + ( int.Parse( str.Remove( 0, 1 ).ToString() ) - 1 ).ToString();
            gObject.GetComponentInChildren<TextMeshProUGUI>().text = str;
        }
        */
    }

    private void OnApplicationQuit() {
        mShuttingDown = true;
    }


    private void OnDestroy() {
        mShuttingDown = true;
    }
}