using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;

public class WandEventModule : BaseInputModule
{

    // singleton makes it easy to access the instanced fields from other code without needing a pointer
    // e.g.  if (WandEventModule.singleton != null && WandEventModule.singleton.controlAxisUsed) ...
    private static WandEventModule _singleton;
    public static WandEventModule singleton
    {
        get
        {
            return _singleton;
        }
    }

    // name of button to use for click/submit
    public string submitButtonName = "WandButton";


    // guiRaycastHit is helpful if you have other places you want to use look input outside of UI system
    // you can use this to tell if the UI raycaster hit a UI element
    private bool _guiRaycastHit;
    public bool guiRaycastHit
    {
        get
        {
            return _guiRaycastHit;
        }
    }

    // buttonUsed is helpful if you use same button elsewhere
    // you can use this boolean to see if the UI used the button press or not
    private bool _buttonUsed;
    public bool buttonUsed
    {
        get
        {
            return _buttonUsed;
        }
    }

    // the UI element to use for the cursor
    // the cursor will appear on the plane of the current UI element being looked at - so it adjusts to depth correctly
    // recommended to use a simple Image component (typical mouse cursor works pretty well) and you MUST add the 
    // Unity created IgnoreRaycast component (script included in example) so that the cursor will not be see by the UI
    // event system
    public RectTransform cursor;

    // interal vars
    private PointerEventData lookData;
    private GameObject currentLook;
    private GameObject currentPressed;
    private GameObject currentDragging;

    // use screen midpoint as locked pointer location, enabling look location to be the "mouse"
    private PointerEventData GetLookPointerEventData()
    {
        Vector2 lookPosition;
        lookPosition.x = Screen.width / 2;
        lookPosition.y = Screen.height / 2;
        if(lookData == null) {
            lookData = new PointerEventData(eventSystem);
        }
        lookData.Reset();
        lookData.delta = Vector2.zero;
        lookData.position = lookPosition;
        lookData.scrollDelta = Vector2.zero;
        eventSystem.RaycastAll(lookData, m_RaycastResultCache);
        lookData.pointerCurrentRaycast = FindFirstRaycast(m_RaycastResultCache);
        if(lookData.pointerCurrentRaycast.gameObject != null) {
            _guiRaycastHit = true;
        }
        else {
            _guiRaycastHit = false;
        }
        m_RaycastResultCache.Clear();
        return lookData;
    }

    // update the cursor location and whether it is enabled
    // this code is based on Unity's DragMe.cs code provided in the UI drag and drop example
    private void UpdateCursor(PointerEventData lookData)
    {
        if(cursor != null) {
            if(lookData.pointerEnter != null) {
                RectTransform draggingPlane = lookData.pointerEnter.GetComponent<RectTransform>();
                Vector3 globalLookPos;
                if(RectTransformUtility.ScreenPointToWorldPointInRectangle(draggingPlane, lookData.position, lookData.enterEventCamera, out globalLookPos)) {
                    cursor.gameObject.SetActive(true);
                    cursor.position = globalLookPos;
                    cursor.rotation = draggingPlane.rotation;
                }
                else {
                    cursor.gameObject.SetActive(false);
                }
            }
            else {
                cursor.gameObject.SetActive(false);
            }
        }
    }

    // clear the current selection
    private void ClearSelection()
    {
        if(eventSystem.currentSelectedGameObject) {
            eventSystem.SetSelectedGameObject(null);
        }
    }

    // select a game object
    private void Select(GameObject go)
    {
        ClearSelection();
        if(ExecuteEvents.GetEventHandler<ISelectHandler>(go)) {
            eventSystem.SetSelectedGameObject(go);
        }
    }

    // send update event to selected object
    // needed for InputField to receive keyboard input
    private bool SendUpdateEventToSelectedObject()
    {
        if(eventSystem.currentSelectedGameObject == null)
            return false;
        BaseEventData data = GetBaseEventData();
        ExecuteEvents.Execute(eventSystem.currentSelectedGameObject, data, ExecuteEvents.updateSelectedHandler);
        return data.used;
    }

    // Process is called by UI system to process events
    public override void Process()
    {
        _singleton = this;

        // send update events if there is a selected object - this is important for InputField to receive keyboard events
        SendUpdateEventToSelectedObject();

        // see if there is a UI element that is currently being looked at
        PointerEventData lookData = GetLookPointerEventData();
        currentLook = lookData.pointerCurrentRaycast.gameObject;

        // handle enter and exit events (highlight)
        // using the function that is already defined in BaseInputModule
        HandlePointerExitAndEnter(lookData, currentLook);

        // update cursor
        UpdateCursor(lookData);

        
        // button down handling
        _buttonUsed = false;
        if(getReal3D.Input.GetButtonDown(submitButtonName)) {
            ClearSelection();
            lookData.pressPosition = lookData.position;
            lookData.pointerPressRaycast = lookData.pointerCurrentRaycast;
            lookData.pointerPress = null;
            if(currentLook != null) {
                currentPressed = currentLook;
                GameObject newPressed = null;
                    
                newPressed = ExecuteEvents.ExecuteHierarchy(currentPressed, lookData, ExecuteEvents.pointerDownHandler);
                if(newPressed == null) {
                    // some UI elements might only have click handler and not pointer down handler
                    newPressed = ExecuteEvents.ExecuteHierarchy(currentPressed, lookData, ExecuteEvents.pointerClickHandler);
                    if(newPressed != null) {
                        currentPressed = newPressed;
                    }
                }
                else {
                    currentPressed = newPressed;
                    // we want to do click on button down at same time, unlike regular mouse processing
                    // which does click when mouse goes up over same object it went down on
                    // reason to do this is head tracking might be jittery and this makes it easier to click buttons
                    ExecuteEvents.Execute(newPressed, lookData, ExecuteEvents.pointerClickHandler);
                }
                    
                if(newPressed != null) {
                    lookData.pointerPress = newPressed;
                    currentPressed = newPressed;
                    Select(currentPressed);
                    _buttonUsed = true;
                }

                if(ExecuteEvents.Execute(currentPressed, lookData, ExecuteEvents.beginDragHandler) || true) {
                    lookData.pointerDrag = currentPressed;
                    currentDragging = currentPressed;
                }
                    
            }
        }
        

        // have to handle button up even if looking away
        if(getReal3D.Input.GetButtonUp(submitButtonName)) {
            if(currentDragging) {
                ExecuteEvents.Execute(currentDragging, lookData, ExecuteEvents.endDragHandler);
                if(currentLook != null) {
                    ExecuteEvents.ExecuteHierarchy(currentLook, lookData, ExecuteEvents.dropHandler);
                }
                lookData.pointerDrag = null;
                currentDragging = null;
            }
            if(currentPressed) {
                ExecuteEvents.Execute(currentPressed, lookData, ExecuteEvents.pointerUpHandler);
                lookData.rawPointerPress = null;
                lookData.pointerPress = null;
                currentPressed = null;
            }
        }

        // drag handling
        if(currentDragging != null) {
            ExecuteEvents.Execute(currentDragging, lookData, ExecuteEvents.dragHandler);
        }

    }
}
