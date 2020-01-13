using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MapManager : MonoBehaviour
{
    public static MapManager instance;
    public enum Direction { Pause, Up, Down, Left, Right };
    MapGrid grid;
    private void Awake()
    {
        instance = this;
        grid = GetComponent<MapGrid>();
    }

    public void CreateMap(int stage)
    {
        SoundManager.PlaySound(SoundManager.Sound.StageStart);
        grid.CreateMap(stage);
    }

    public bool IsMoveAbleNode(Vector3 pos)
    {
        MapNode node = grid.GetNodeFromWorldPoint(pos);
        if (node != null && (node.type == 1|| node.type==2))
            return true;
        return false;
    }
    public bool IsTranslateAbleNode(Vector3 targetPos, Vector3 searchPos, Direction dir)
    {
        MapNode targetNode = grid.GetNodeFromWorldPoint(targetPos);
        if (IsMoveAbleNode(searchPos) && (targetNode.type == 4|| targetNode.type==5))
            return true;
        return false;
    }
    public bool IsGoalNode(Vector3 targetPos, Vector3 searchPos, Direction dir)
    {
        MapNode searchNode = grid.GetNodeFromWorldPoint(searchPos);
        MapNode targetNode = grid.GetNodeFromWorldPoint(targetPos);
        if (searchNode != null && searchNode.type == 2&& (targetNode.type == 4|| targetNode.type==5))
            return true;
        return false;
    }

    public void MoveBlock(Vector3 targetPos, Vector3 searchPos)
    {
        grid.MoveBlock(targetPos, searchPos);
    }

    public void GoalBlock(Vector3 targetPos, Vector3 searchPos)
    {
        grid.GoalBlock(targetPos, searchPos);
    }

    public bool CheckClearable()
    {
        return grid.IsClearable();
    }
}
