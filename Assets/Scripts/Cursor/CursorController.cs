using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorController : MonoBehaviour
{
    public enum CursorState { MENU = 0, GAMING = 1 }
        
    public CursorState ActiveCursorState
    {
        get { return activeCursorState; }
        set
        {
            activeCursorState = value;
            CursorActiveStatePropertyChanged();
        }
    }    
    [SerializeField] private CursorState activeCursorState;
    [SerializeField] private Texture2D[] cursors;
    

    void Start()
    {        
        CursorActiveStatePropertyChanged();
    }

    private void CursorActiveStatePropertyChanged()
    {
        Cursor.SetCursor(cursors[(int)activeCursorState], Vector2.zero, CursorMode.ForceSoftware);        
    }
}
