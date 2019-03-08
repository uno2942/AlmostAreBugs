﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using System.Linq;

public class ScriptWindow : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

    private static ScriptWindow scriptWindow;
    private static bool mShuttingDown = false;
    private static object mLock = new object();
    
    private const float DELAYTIME=5.0f;
    private float passedTime=0;
    private bool IsWriteEventTriggered = true;
    private bool IsMouseOnScriptWindow = false;

    public static ScriptWindow ScriptWindowInstance
    {
        get
        {
            if( mShuttingDown ) {
                Debug.LogWarning( "ScriptWindow is already destroyed." );
                return null;
            }
            lock( mLock ) {
                if( scriptWindow == null ) {
                    scriptWindow = (ScriptWindow) FindObjectOfType<ScriptWindow>();
                    if( scriptWindow == null ) {
                        Debug.LogWarning( "ScriptWindow gameObject does not exists." );
                        return null;
                    }
                }
                return scriptWindow;
            }
        }
    }

    // Start is called before the first frame update
    void Start() {
    }

    // Update is called once per frame
    void Update() {
        if( IsMouseOnScriptWindow )
            ScriptWindowOn();
        if( IsWriteEventTriggered ) {
            passedTime += Time.deltaTime;
            if( passedTime > DELAYTIME ) {
                StartCoroutine( "FadeOut" );
                IsWriteEventTriggered = false;
            }
        }
    }
    public void ScriptPrinterForClickItem( ItemManager.ItemList item, ItemManager.PresentState presentState, GameObject gObject ) {
        foreach( var srpt in Parser.ParserInstance.loadedDataForScriptOnclicks.ScriptOnclick ) {
            if( srpt.target == item.ToString() ) {
                Write( srpt.objectOnclick );
                break;
            }
            else if(item== ItemManager.ItemList.Fax ) {
                if( srpt.target == item.ToString() + "On" )  {
                    if( GameObject.Find( "ButtonOnGame" ).GetComponent<ButtonOnGame>().IsValid == true )
                        Write( srpt.objectOnclick );
                }
                else if( srpt.target == item.ToString() + "Off" ) {
                    if( GameObject.Find( "ButtonOnGame" ).GetComponent<ButtonOnGame>().IsValid == false )
                        Write( srpt.objectOnclick );
                }
            } else if( item == ItemManager.ItemList.TV ) {
                if( srpt.target == item.ToString() + "On" ) {
                    if( GameObject.Find( "TV" ).GetComponent<TV>().IsOpened == true )
                        Write( srpt.objectOnclick );
                } else if( srpt.target == item.ToString() + "Off" ) {
                    if( GameObject.Find( "TV" ).GetComponent<TV>().IsOpened == false )
                        Write( srpt.objectOnclick );
                }
            } else if( item == ItemManager.ItemList.ButtonOnGame ) {
                if( srpt.target == item.ToString() + "On" ) {
                    if( GameObject.Find( "ButtonOnGame" ).GetComponent<ButtonOnGame>().IsOn == true )
                        Write( srpt.objectOnclick );
                } else if( srpt.target == item.ToString() + "Off" ) {
                    if( GameObject.Find( "ButtonOnGame" ).GetComponent<ButtonOnGame>().IsOn == false )
                        Write( srpt.objectOnclick );
                }
            }
        }
    }

    public string ItemDescriptionForCheckedItem( ItemManager.ItemList item) {
        foreach( var srpt in Parser.ParserInstance.loadedDataForItemScripts.ItemScript ) {
            if( srpt.codeName == item.ToString() ) {
                return srpt.boxScript;
            }
        }
        return "";
    }

    public string ItemNameForCheckedItem( ItemManager.ItemList item ) {
        foreach( var srpt in Parser.ParserInstance.loadedDataForItemScripts.ItemScript ) {
            if( srpt.codeName == item.ToString() ) {
                return srpt.stringName;
            }
        }
        return "";
    }

    public void Write(string str ) {

        scriptWindow.GetComponentInChildren<TextMeshProUGUI>().text += (str+'\n');
        ScriptWindowOn();
        /*foreach(var strLine in str.Split('\n')) {
            WriteALine(strLine)
        }
        */
    }
    public void WriteALine( string str )
    {
        if( str == null )
            Debug.LogError( "string is null" );
        if( str.Split( '\n' ).Length > 1 )
            Debug.LogError( "string is over one line" );
            // '\n'+gameObject의 Text(mesh pro)에 한 줄을 넣는 부분이 들어가야함.
            ScriptWindowOn();
    }

    public void ScriptWindowOn() {
        IsWriteEventTriggered = true;
        Color color1 = scriptWindow.GetComponent<Image>().color;
        color1.a = 1.0f;
        scriptWindow.GetComponent<Image>().color = color1;

        Color color2 = scriptWindow.GetComponentInChildren<TextMeshProUGUI>().color;
        color2.a = 1.0f;
        scriptWindow.GetComponentInChildren<TextMeshProUGUI>().color = color2;
        passedTime = 0;
    }

    IEnumerator FadeOut() {
        Color color1=scriptWindow.GetComponent<Image>().color;
        Color color2= scriptWindow.GetComponentInChildren<TextMeshProUGUI>().color;
        while( color1.a>0f) {
            color1.a -= 0.05f;
            scriptWindow.GetComponent<Image>().color = color1;
            color2.a -= 0.05f;
            scriptWindow.GetComponentInChildren<TextMeshProUGUI>().color = color2;
            yield return new WaitForSeconds( 0.02f );
        }
    }

    public void OnPointerEnter( PointerEventData eventData ) {
        IsMouseOnScriptWindow = true;
    }
    public void OnPointerExit(PointerEventData eventData ) {
        IsMouseOnScriptWindow = false;
    }

    private void OnApplicationQuit() {
        mShuttingDown = true;
    }


    private void OnDestroy() {
        mShuttingDown = true;
    }
}
