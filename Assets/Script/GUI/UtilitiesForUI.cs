using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using OnlyCornect;
using UnityEngine;

public class UtilitiesForUI : MonoBehaviour
{
    // --------------------------------------------------------------------------------------------------------------------------------------
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
    public const int CLUE_PICTURE_WIDTH = 1680;
    public const int CLUE_PICTURE_HEIGHT = 960;

    public const float PICTURE_ANSWER_OVERLAY_ALPHA = 0.65f;

    public Color OVERLAY_LIGHT;
    public Color OVERLAY_TIME;
    public Color OVERLAY_TEAM;
    public Color OVERLAY_DARK;

    public Sprite TEAM_BOX_A;
    public Sprite TEAM_BOX_B;

    public static Dictionary<string, Sprite> Pictures = new Dictionary<string, Sprite>();

    // --------------------------------------------------------------------------------------------------------------------------------------
    public static void LoadPictures(QuizData quiz)
    {
        //Create an array of file paths from which to choose
        string folderPath = Application.streamingAssetsPath + "/Pictures/";
        string[] filePaths = Directory.GetFiles(folderPath, "*.png");

        for (int i = 0; i < filePaths.Length; i++)
        {
            //Converts desired path into byte array
            byte[] pngBytes = System.IO.File.ReadAllBytes(filePaths[i]);

            //Creates texture and loads byte array data to create image
            Texture2D tex = new Texture2D(CLUE_PICTURE_WIDTH, CLUE_PICTURE_HEIGHT);
            tex.LoadImage(pngBytes);

            //Creates a new Sprite based on the Texture2D
            Sprite sprite = Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100.0f);

            Pictures.Add(Path.GetFileNameWithoutExtension(filePaths[i]), sprite);
        }
    }
}
