using System;
using UnityEngine;

[RequireComponent(typeof(Events))]
public class InputsHandler : MonoBehaviour
{
	public Camera cam;
    bool isMobile;
    bool mouseClickStarted;
    GameObject lastClickedBtn;
    public GameObject[] buttons;
	public GameObject eventOb;
	public string escapeEventName = string.Empty;

    private void Start()
    {
        isMobile = (PlatformChecker.IsAndroid() || PlatformChecker.IsIOS());
        if (cam == null)
        {
            cam = Camera.main;
        }
        if (eventOb == null)
        {
            eventOb = cam.gameObject;
        }
        if (buttons.Length == 0)
        {
            buttons = GameObject.FindGameObjectsWithTag("UIButton");
        }
        Events component = GetComponent<Events>();
        if (component == null)
        {
            gameObject.AddComponent<Events>();
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !string.IsNullOrEmpty(escapeEventName))
        {
            eventOb.SendMessage(escapeEventName, this);
        }
        if (isMobile)
        {
            OnScreenTouch();
        }
        else
        {
            OnMouseClick();
        }
    }

    private void OnScreenTouch()
    {
        if (Input.touchCount == 0)
        {
            return;
        }
        Touch touch = Input.GetTouch(0);
        if (touch.phase == TouchPhase.Began || touch.phase == TouchPhase.Moved)
        {
            Vector3 position = touch.position;
            position.z = 0f - cam.transform.position.z;
            Vector3 v = cam.ScreenToWorldPoint(position);
            bool isTouchBegain = true;
            if (touch.phase == TouchPhase.Moved)
            {
                isTouchBegain = false;
            }
            RaycastHit2D raycastHit2D = Physics2D.Raycast(v, Vector3.zero);
            if (raycastHit2D.collider != null)
            {
                ScreenClickHandle(raycastHit2D.collider.gameObject, isTouchBegain);
            }
            else
            {
                ScreenClickHandle(null, isTouchBegain);
            }
        }
        else if (touch.phase == TouchPhase.Ended)
        {
            ScreenClickEndedHandle();
        }
    }

    private void OnMouseClick()
    {
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = 0f - cam.transform.position.z;
        Vector3 v = cam.ScreenToWorldPoint(mousePosition);
        //Vector2 v = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D raycastHit2D = Physics2D.Raycast(v, Vector2.zero);
        //Debug.Log("test");
        if (raycastHit2D.collider != null)
        {
            //Debug.Log(raycastHit2D.collider.name);
        }
        
        if (raycastHit2D.collider != null)
        {
            if (Input.GetMouseButtonDown(0))
            {
                ScreenClickHandle(raycastHit2D.collider.gameObject, true);
                mouseClickStarted = true;
            }
            else if (mouseClickStarted)
            {
                ScreenClickHandle(raycastHit2D.collider.gameObject, false);
            }
        }
        else
        {
            ScreenClickHandle(null, false);
        }
        if (Input.GetMouseButtonUp(0) && mouseClickStarted)
        {
            ScreenClickEndedHandle();
            mouseClickStarted = false;
        }
    }

    private void ScreenClickHandle(GameObject ob, bool isTouchBegain)
    {
        string a = string.Empty;
        if (ob != null)
        {
            a = ob.name;
        }
        foreach (GameObject gameobj in buttons)
        {
            if (!(gameobj == null))
            {
                Button component = gameobj.GetComponent<Button>();
                if (!(component == null))
                {
                    if (a == gameobj.name)
                    {
                        if (!component.isBegan)
                        {
                            component.isBegan = true;
                            lastClickedBtn = gameobj;
                            if (component.hoverIcon != null)
                            {
                                component.spriteRendererComp.sprite = component.hoverIcon;
                            }
                            if (component.clickReleaseSFx != null)
                            {
                                AudioSource.PlayClipAtPoint(component.clickReleaseSFx, Vector3.zero);
                            }
                        }
                    }
                    else if (!isTouchBegain && component.isBegan)
                    {
                        lastClickedBtn = null;
                        component.isBegan = false;
                        if (component.normalIcon != null)
                        {
                            component.spriteRendererComp.sprite = component.normalIcon;
                        }
                    }
                }
            }
        }
    }

    private void ScreenClickEndedHandle()
    {
        if (lastClickedBtn == null)
        {
            return;
        }
        Button component = lastClickedBtn.GetComponent<Button>();
        if (component == null)
        {
            return;
        }
        component.isBegan = false;
        if (component.resetIconOnRelease && component.normalIcon != null)
        {
            component.spriteRendererComp.sprite = component.normalIcon;
        }
        if (!string.IsNullOrEmpty(component.message))
        {
            eventOb.SendMessage(component.message, component.messageObject);
            //Debug.Log(component.messageObject);
        }
        else
        {
            Debug.LogWarning("empty message on <i>" + lastClickedBtn.name + "</i>click");
        }
        lastClickedBtn = null;
    }
}
