using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class SkillSingleLineTrackStyle : SkillTrackStyleBase
{
    private const string MenuAssetPath ="Assets/SkillEditor/Editor/Track/Assets/SinglineTrackStyle/SingleLineTrackMenu.uxml"; 
    private const string TrackAssetPath ="Assets/SkillEditor/Editor/Track/Assets/SinglineTrackStyle/SingleLineTrackContent.uxml";

    private const string RightMenuPath =
        "Assets/SkillEditor/Editor/Track/Assets/SinglineTrackStyle/RightClickMenu.uxml";

    private VisualElement rightMenu;
    public void Init(VisualElement menuParent,VisualElement contentParent,string title,Action AddAction)
    {
        this.menuParent = menuParent;
        this.contentParent = contentParent;
        menuRoot=     AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(MenuAssetPath).Instantiate().Query().ToList()[1];
        menuParent.Add(menuRoot);

        titleLabel = (Label)menuRoot;
        titleLabel.text = title;
        contentRoot=        AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(TrackAssetPath).Instantiate().Query().ToList()[1];
        contentParent.Add(contentRoot);
        
        menuRoot.RegisterCallback<MouseDownEvent>(ItemMouseDown);
        
        
        rightMenu = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(RightMenuPath).Instantiate().Query().ToList()[1];
        SkillEditorWindow.Instance.Root.Add(rightMenu);
        rightMenu.style.display = DisplayStyle.None;
        rightMenu.RegisterCallback<MouseLeaveEvent>(RightMenuMouseLeave);

        rightMenu.Q<Button>("RightClickMenuAdd").clicked += () =>
        {
            AddAction();
            rightMenu.style.display = DisplayStyle.None;
        };
    }

    private void RightMenuMouseLeave(MouseLeaveEvent evt)
    {
        rightMenu.style.display = DisplayStyle.None;
    }

    private void ItemMouseDown(MouseDownEvent evt)
    {
        if (evt.button == (int)MouseButton.RightMouse)
        {
            rightMenu.style.display = DisplayStyle.Flex;
            rightMenu.transform.position =new Vector3( Event.current.mousePosition.x-10,menuRoot.layout.y+80,0);

        }
    }
}
