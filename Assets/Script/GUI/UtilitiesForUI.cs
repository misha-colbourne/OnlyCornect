using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UtilitiesForUI : MonoBehaviour
{
    public static UtilitiesForUI Instance
    {
        get
        {
            if (instance == null)
                instance = FindObjectOfType<UtilitiesForUI>();
            return instance;
        }
    }

    private static UtilitiesForUI instance;

    // --------------------------------------------------------------------------------------------------------------------------------------
    public Color OVERLAY_LIGHT;
    public Color OVERLAY_TIME;
    public Color OVERLAY_TEAM;
    public Color OVERLAY_DARK;

    //public static string OVERLAY_LIGHT = "#60C4FD";
    //public const string OVERLAY_TIME = "#04517D";
    //public const string OVERLAY_TEAM = "#383B3C";
    //public const string OVERLAY_DARK = "#1C2429";

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
