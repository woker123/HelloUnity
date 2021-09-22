using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace CustomGUI
{
    public class TextItemScrollArea
    {
        static Vector2 m_scrollPos = new Vector2(0, 0);
        public static void Draw(Rect position, string[] items, int perItemHeight = 20)
        {
            GUILayout.BeginArea(position);
            m_scrollPos = GUILayout.BeginScrollView(m_scrollPos, true, true);
                GUILayout.BeginVertical();
                for(int i = 0; i < items.Length; ++i)
                    GUILayout.TextField(items[i], GUILayout.MinHeight(perItemHeight), GUILayout.MaxHeight(perItemHeight));
                GUILayout.EndVertical();
            GUILayout.EndScrollView();
            GUILayout.EndArea();
        }
    }

}
