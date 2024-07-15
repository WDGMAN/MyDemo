using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class SkillMultilineTrackStyle : SkillTrackStyleBase
{
    #region 常量

    private const string MenuAssetPath =
        "Assets/SkillEditor/Editor/Track/Assets/MultilineTrackStyle/MultilineTrackMenu.uxml";

    private const string TrackAssetPath =
        "Assets/SkillEditor/Editor/Track/Assets/MultilineTrackStyle/MultilineTrackContent.uxml";

    private const float headHeight = 30f;
    private const float itemHeight = 30f;

    #endregion

    private VisualElement menuItemParent; //子轨道的菜单父物体


    private List<ChildTrack> childTrackList = new List<ChildTrack>();
    private Action addChildTrackAction;
    private Func<int, bool> deleteChildTrackFunc;
    private Action<int, int> swapChildTrackAction;
    private Action<ChildTrack, string> updateTrackNameAction;

    public void Init(VisualElement menuParent, VisualElement contentParent, string title, Action addChildTrackAction,
        Func<int, bool> deleteChildTrackFunc, Action<int, int> swapChildTrackAction,
        Action<ChildTrack, string> updateTrackNameAction)
    {
        this.deleteChildTrackFunc = deleteChildTrackFunc;
        this.addChildTrackAction = addChildTrackAction;
        this.swapChildTrackAction = swapChildTrackAction;
        this.updateTrackNameAction = updateTrackNameAction;

        this.menuParent = menuParent;
        this.contentParent = contentParent;

        menuRoot = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(MenuAssetPath).Instantiate().Query().ToList()[1];
        menuParent.Add(menuRoot);


        titleLabel = menuRoot.Q<Label>("Title");
        titleLabel.text = title;
        menuItemParent = menuRoot.Q<VisualElement>("TrackMenuList");
        menuItemParent.RegisterCallback<MouseDownEvent>(ItemParentMouseDown);
        menuItemParent.RegisterCallback<MouseMoveEvent>(ItemParentMouseMove);
        menuItemParent.RegisterCallback<MouseUpEvent>(ItemParentMouseUp);
        menuItemParent.RegisterCallback<MouseOutEvent>(ItemParentMouseOut);

        contentRoot = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(TrackAssetPath).Instantiate().Query().ToList()[1];
        contentParent.Add(contentRoot);

        //添加子轨道的按钮
        Button addButton = menuRoot.Q<Button>("AddButton");
        addButton.clicked += AddButtonClick;
        UpdateSize();
    }

    #region 子轨道鼠标交互

    private bool isDragging = false;
    private int selectTrackIndex = -1;

    private void ItemParentMouseOut(MouseOutEvent evt)
    {
        //检测鼠标位置是否真的离开了我们的范围
        if (!menuItemParent.contentRect.Contains(evt.localMousePosition))
        {
            isDragging = false;
        }
    }

    private void ItemParentMouseUp(MouseUpEvent evt)
    {
        isDragging = false;
    }

    private void ItemParentMouseMove(MouseMoveEvent evt)
    {
        if (selectTrackIndex == -1 || isDragging == false) return;
        float mousePosition = evt.localMousePosition.y - itemHeight / 2;
        int mouseTrackIndex = GetChildIndexByMousePosition(mousePosition);
        if (mouseTrackIndex != selectTrackIndex)
        {
            SwapChildTrack(selectTrackIndex, mouseTrackIndex);
            selectTrackIndex = mouseTrackIndex; //把选中的轨道更新为鼠标所在的轨道
        }
    }

    private void ItemParentMouseDown(MouseDownEvent evt)
    {
        //关闭旧的
        if (selectTrackIndex != -1)
        {
            childTrackList[selectTrackIndex].UnSelect();
        }

        //通过高度推导出当前交互的是第几个
        float mousePosition = evt.localMousePosition.y - itemHeight / 2;
        selectTrackIndex = GetChildIndexByMousePosition(mousePosition);
        childTrackList[selectTrackIndex].Select();


        isDragging = true;
    }

    private int GetChildIndexByMousePosition(float mousePositionY)
    {
        int trackIndex = Mathf.RoundToInt(mousePositionY / itemHeight);
        trackIndex = Mathf.Clamp(trackIndex, 0, childTrackList.Count - 1);
        return trackIndex;
    }

    #endregion


    private void SwapChildTrack(int index1, int index2)
    {
        if (index1 != index2)
        {
            //不验证有效性 如果出错 说明本身逻辑就有问题
            ChildTrack childTrack1 = childTrackList[index1];
            ChildTrack childTrack2 = childTrackList[index2];
            childTrackList[index1] = childTrack2;
            childTrackList[index2] = childTrack1;
            UpdateChilds();
            //上级轨道的实际数据变更
            swapChildTrackAction(index1, index2);
        }
    }

    private void UpdateSize()
    {
        float height = headHeight + (childTrackList.Count * itemHeight);
        contentRoot.style.height = height;
        menuRoot.style.height = height;
        menuItemParent.style.height = childTrackList.Count * itemHeight;
    }

    //添加子轨道
    private void AddButtonClick()
    {
        addChildTrackAction?.Invoke();
    }

    public ChildTrack AddChildTrack()
    {
        ChildTrack childTrack = new ChildTrack();
        childTrack.Init(menuItemParent, childTrackList.Count, contentRoot, DeleteChildTrack, DeleteChildTrackAndData,
            updateTrackNameAction);
        childTrackList.Add(childTrack);
        UpdateSize();
        return childTrack;
    }


    //删除子轨道以及子轨道对应的数据
    private void DeleteChildTrackAndData(ChildTrack childTrack)
    {
        if (deleteChildTrackFunc == null) return;
        int index = childTrack.GetIndex();
        if (deleteChildTrackFunc(index))
        {
            childTrack.DoDestory();
            childTrackList.RemoveAt(index);
            //所有子轨道都需要更新一下索引
            UpdateChilds(index);
            UpdateSize();
        }
    }

    //删除子轨道
    private void DeleteChildTrack(ChildTrack childTrack)
    {
        int index = childTrack.GetIndex();
        childTrack.DoDestory();
        childTrackList.RemoveAt(index);
        //所有子轨道都需要更新一下索引
        UpdateChilds(index);
        UpdateSize();
    }

    private void UpdateChilds(int startIndex = 0)
    {
        for (int i = startIndex; i < childTrackList.Count; i++)
        {
            childTrackList[i].SetIndex(i);
        }
    }

    //多行轨道中的子轨道
    public class ChildTrack
    {
        private const string childTrackMenuItemAssetPath =
            "Assets/SkillEditor/Editor/Track/Assets/MultilineTrackStyle/MultilineTrackMenuItem.uxml";

        private const string childTrackContentAssetPath =
            "Assets/SkillEditor/Editor/Track/Assets/MultilineTrackStyle/MultilineTrackContentItem.uxml";

        public VisualElement menuRoot;
        public VisualElement trackRoot;


        public VisualElement menuParent;
        public VisualElement trackParent;

        private TextField trackNameField;

        private Action<ChildTrack> deleteAction;
        private Action<ChildTrack> destoryAction;
        private Action<ChildTrack, string> updateTrackNameAction;
        private static Color normalColor = new Color(0, 0, 0, 0);
        private static Color selectColor = Color.green;

        private int index;
        private VisualElement content;

        public void Init(VisualElement menuParent, int index, VisualElement trackParent,
            Action<ChildTrack> deleteAction, Action<ChildTrack> destoryAction,
            Action<ChildTrack, string> updateTrackNameAction)
        {
            this.menuParent = menuParent;
            this.trackParent = trackParent;
            this.deleteAction = deleteAction;
            this.destoryAction = destoryAction;
            this.updateTrackNameAction = updateTrackNameAction;

            menuRoot = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(childTrackMenuItemAssetPath).Instantiate().Query()
                .ToList()[1];
            menuParent.Add(menuRoot);


            trackNameField = menuRoot.Q<TextField>("NameField");
            trackNameField.RegisterCallback<FocusInEvent>(TrackNameFieldFocusIn);
            trackNameField.RegisterCallback<FocusOutEvent>(TrackNameFieldFocusOut);

            Button deleteButton = menuRoot.Q<Button>("DeleteButton");
            deleteButton.clicked += () => destoryAction(this);

            trackRoot = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(childTrackContentAssetPath).Instantiate().Query()
                .ToList()[1];
            trackParent.Add(trackRoot);
            SetIndex(index);
            UnSelect();
        }

        private string oldTrackNameFiledValue;

        private void TrackNameFieldFocusIn(FocusInEvent evt)
        {
            oldTrackNameFiledValue = trackNameField.value;
        }

        private void TrackNameFieldFocusOut(FocusOutEvent evt)
        {
            if (oldTrackNameFiledValue != trackNameField.value)
            {
                updateTrackNameAction?.Invoke(this, trackNameField.value);
            }
        }


        public void InitContent(VisualElement content)
        {
            this.content = content;
            trackRoot.Add(content);
        }

        public void SetTrackName(string name)
        {
            trackNameField.value = name;
        }

        public int GetIndex()
        {
            return index;
        }

        public void SetIndex(int index)
        {
            this.index = index;
            float height = 0;
            Vector3 menuPos = trackRoot.transform.position;
            height = itemHeight * index;
            menuPos.y = height;
            menuRoot.transform.position = menuPos;

            Vector3 trackPos = trackRoot.transform.position;
            height = index * itemHeight + headHeight;
            trackPos.y = height;
            trackRoot.transform.position = trackPos;
        }

        public void Destory()
        {
            deleteAction(this);
        }

        public void DoDestory()
        {
            if (menuParent != null && menuRoot != null)
            {
                menuParent.Remove(menuRoot);
                trackParent.Remove(trackRoot);
            }
        }

        public void Select()
        {
            menuRoot.style.backgroundColor = selectColor;
        }

        public void UnSelect()
        {
            menuRoot.style.backgroundColor = normalColor;
        }
    }
}