using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class RoomNodeSO : ScriptableObject
{
    [HideInInspector] public string id;
    [HideInInspector] public List<string> parentRoomNodeIDList = new List<string>();
    [HideInInspector] public List<string> childRoomNodeIDList = new List<string>();
    [HideInInspector] public RoomNodeGraphSO roomNodeGraph;
    public RoomNodeTypeSO roomNodeType;
    [HideInInspector] public RoomNodeTypeListSO roomNodeTypeList;

    #region Editor Code

    //the following code should only be run in the Unity Editor
#if UNITY_EDITOR

    [HideInInspector] public Rect rect;
    [HideInInspector] public bool isLeftClickDragging = false;
    [HideInInspector] public bool isSelected = false;

    /// <summary>
    /// Initialise node
    /// </summary>
    /// <param name="rect"></param>
    /// <param name="nodeGraph"></param>
    /// <param name="roomNodeType"></param>
    public void Initialise(Rect rect, RoomNodeGraphSO nodeGraph, RoomNodeTypeSO roomNodeType)
    {
        this.rect = rect;
        this.id = Guid.NewGuid().ToString();
        this.name = "RoomNode";
        this.roomNodeGraph = nodeGraph;
        this.roomNodeType = roomNodeType;

        // Load room node type list
        roomNodeTypeList = GameResources.Instance.roomNodeTypeList;
    }

    /// <summary>
    /// Draw a node with the nodestyle
    /// </summary>
    /// <param name="nodeStyle"></param>
    public void Draw(GUIStyle nodeStyle)
    {
        // Draw Node Box
        GUILayout.BeginArea(rect, nodeStyle);

        // Start Region To Detect Popup Selection Changes
        EditorGUI.BeginChangeCheck();

        // if the room node has a parent or is of type entrance then display a label else display a popup
        if (parentRoomNodeIDList.Count > 0 || roomNodeType.isEntrance)
        {
            // Display a label that can't be changed
            EditorGUILayout.LabelField(roomNodeType.roomNodeTypeName);
        }
        else
        {
            // Display a popup using the RoomNodeType name values that can be selected from (default to the currently set roomNodeType)

            int selected = roomNodeTypeList.list.FindIndex(x => x == roomNodeType);

            int selection = EditorGUILayout.Popup("", selected, GetRoomNodeTypesToDisplay());

            roomNodeType = roomNodeTypeList.list[selection];

            // If the room type selection has changed making child connections potentially invalid
            if (roomNodeTypeList.list[selected].isCorridor && !roomNodeTypeList.list[selection].isCorridor || 
                !roomNodeTypeList.list[selected].isCorridor && roomNodeTypeList.list[selection].isCorridor || 
                !roomNodeTypeList.list[selected].isBossRoom && roomNodeTypeList.list[selection].isBossRoom)
            {
                // If a room node type has been changed and it already has children then delete the parent-child links since we need to revalidate any
                if (childRoomNodeIDList.Count > 0)
                {
                    for (int i = childRoomNodeIDList.Count - 1; i >= 0; i--)
                    {
                        // Get child room node
                        RoomNodeSO childRoomNode = roomNodeGraph.GetRoomNode(childRoomNodeIDList[i]);

                        // If the child room node is not null
                        if (childRoomNode != null)
                        {
                            // Remove childID from parent room node
                            RemoveChildRoomNodeIDFromRoomNode(childRoomNode.id);

                            // Remove parentID frokm child room node
                            childRoomNode.RemoveParentRoomNodeIDFromRoomNode(id);
                        }
                    }
                }
            }
        }

        if (EditorGUI.EndChangeCheck())
            EditorUtility.SetDirty(this);

        GUILayout.EndArea();
    }

    public string[] GetRoomNodeTypesToDisplay()
    {
        string[] roomArray = new string[roomNodeTypeList.list.Count];

        for (int i = 0; i < roomNodeTypeList.list.Count; i++)
        {
            if (roomNodeTypeList.list[i].displayInNodeGraphEditor)
            {
                roomArray[i] = roomNodeTypeList.list[i].roomNodeTypeName;
            }
        }

        return roomArray;
    }

    public void ProcessEvents(Event currentEvent)
    {
        switch (currentEvent.type)
        {
            // Process Mouse Down Events
            case EventType.MouseDown:
                ProcessMouseDownEvent(currentEvent);
                break;
           
            // Process Mouse Up Events
            case EventType.MouseUp:
                ProcessMouseUpEvent(currentEvent);
                break;
            // Process Mouse Drag Events
            case EventType.MouseDrag:
                ProcessMouseDragEvent(currentEvent);
                break;

            default:
                break;
        }
    }

    /// <summary>
    /// Process mouse down event
    /// </summary>
    /// <param name="currentEvent"></param>
    private void ProcessMouseDownEvent(Event currentEvent)
    {
        // left click down
        if (currentEvent.button == 0)
        {
            ProcessLeftClickDownEvent();
        }
        else if (currentEvent.button == 1)
        {
            ProcessRightClickDownEvent(currentEvent);
        }
    }

    /// <summary>
    /// Process left click down event
    /// </summary>
    private void ProcessLeftClickDownEvent()
    {
        // Selects the node being moved/selected within the Unity Editor
        Selection.activeObject = this;

        // Toggle node selection
        if (isSelected == true)
        {
            isSelected = false;
        }
        else
        {
            isSelected = true;
        }
    }

    private void ProcessRightClickDownEvent(Event currentEvent)
    {
        roomNodeGraph.SetNodeToDrawConnectionLineFrom(this, currentEvent.mousePosition);
    }

    /// <summary>
    /// Process mouse up event
    /// </summary>
    /// <param name="currentEvent"></param>
    private void ProcessMouseUpEvent(Event currentEvent)
    {
        // If left click up
        if (currentEvent.button == 0)
        {
            ProcessLeftClickUpEvent();
        }
    }

    /// <summary>
    /// Process left click up event
    /// </summary>
    private void ProcessLeftClickUpEvent()
    {
        if (isLeftClickDragging)
        {
            isLeftClickDragging = false;
        }
    }

    private void ProcessMouseDragEvent(Event currentEvent)
    {
        // process left click drag event
        if (currentEvent.button == 0)
        {
            ProcessLeftMouseDragEvent(currentEvent);
        }
    }

    /// <summary>
    ///  Process left mouse drag movement
    /// </summary>
    /// <param name="currentEvent"></param>
    private void ProcessLeftMouseDragEvent(Event currentEvent)
    {
        isLeftClickDragging = true;

        DragNode(currentEvent.delta);
        GUI.changed = true;
    }

    /// <summary>
    ///  Drag node
    /// </summary>
    /// <param name="delta"></param>
    public void DragNode(Vector2 delta)
    {
        rect.position += delta;
        EditorUtility.SetDirty(this);
    }

    public bool AddChildRoomNodeIDToRoomNode(string childID)
    {
        // Check child node can be added validly to parent
        if (IsChildRoomValid(childID))
        {
            childRoomNodeIDList.Add(childID);
            return true;
        }

        return false;
    }

    /// <summary>
    /// Check the child node can be validly added to the parent node - return true if it can, otherwise return false
    /// </summary>
    /// <param name="childID"></param>
    /// <returns></returns>
    public bool IsChildRoomValid(string childID)
    {
        bool isConncectedBossNodeAlready = false;
        // Check if there is already a connected bood room in the node graph
        foreach (RoomNodeSO roomNode in roomNodeGraph.roomNodeList)
        {
            if (roomNode.roomNodeType.isBossRoom && roomNode.parentRoomNodeIDList.Count > 0)
                isConncectedBossNodeAlready = true;
        }

        // if the child node has a type of boss room and there is already a connected boos room node, return false
        if (roomNodeGraph.GetRoomNode(childID).roomNodeType.isBossRoom && isConncectedBossNodeAlready)
            return false;

        // If the child node has a type of none, return false
        if (roomNodeGraph.GetRoomNode(childID).roomNodeType.isNone)
            return false;

        // If the node already has a child with this child ID, return false
        if (childRoomNodeIDList.Contains(childID))
            return false;

        // If this node ID and the child ID are the same, return false
        if (id == childID)
            return false;

        // If this childID is already in the parentID list, return false
        if (parentRoomNodeIDList.Contains(childID))
            return false;

        // If the child node already has a parent, return false
        if (roomNodeGraph.GetRoomNode(childID).parentRoomNodeIDList.Count > 0)
            return false;

        // If child is a corridor and this node is a corridor, return false
        if (roomNodeGraph.GetRoomNode(childID).roomNodeType.isCorridor && roomNodeType.isCorridor)
            return false;

        // If child is not a corridor and this node is not a corridor, return false
        if (!roomNodeGraph.GetRoomNode(childID).roomNodeType.isCorridor && !roomNodeType.isCorridor)
            return false;

        // If adding a corridor check that this node has < the maximum permitted child members
        if (roomNodeGraph.GetRoomNode(childID).roomNodeType.isCorridor && childRoomNodeIDList.Count >= Settings.maxChildCorridors)
            return false;

        // If the child room is an entrance, return false - the entrance must always be the top level parent node
        if (roomNodeGraph.GetRoomNode(childID).roomNodeType.isEntrance)
            return false;

        // If adding a room to a corridor check that this corridor node doesn't already have a room added
        if (!roomNodeGraph.GetRoomNode(childID).roomNodeType.isCorridor && childRoomNodeIDList.Count > 0)
            return false;

        return true;
    }

    public bool AddParentRoomNodeIDToRoomNode(string parentID)
    {
        parentRoomNodeIDList.Add(parentID);
        return true;
    }

    public bool RemoveChildRoomNodeIDFromRoomNode(string childID)
    {
        // If the node contains the child ID, then remove it
        if (childRoomNodeIDList.Contains(childID))
        {
            childRoomNodeIDList.Remove(childID);
            return true;
        }
        return false;
    }

    public bool RemoveParentRoomNodeIDFromRoomNode(string parentID)
    {
        // If the node contains the parent ID, then removes it
        if (parentRoomNodeIDList.Contains(parentID))
        {
            parentRoomNodeIDList.Remove(parentID);
            return true;
        }
        return false;
    }

#endif

    #endregion Editor Code
}
