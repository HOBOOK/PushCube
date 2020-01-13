using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour
{
    public SwipeController swipeController;
    public static Controller instance;
    public Transform Target;

    public MapManager.Direction direction;
    Vector3 targetPos;
    Vector3 searchPos;

    float rotateY;
    bool isMove; // 움직이는지
    bool isMoving; // 움직이는중인지
    bool isJump;
    public bool isMoveAble { get { return isMove && !isJump; } }

    private void Awake()
    {
        instance = this;
    }

    private void Update()
    {
        if(GameManager.instance.IsPlaying)
        {
            if (swipeController.SwipeUp)
                direction = MapManager.Direction.Up;
            else if (swipeController.SwipeDown)
                direction = MapManager.Direction.Down;
            else if (swipeController.SwipeLeft)
                direction = MapManager.Direction.Left;
            else if (swipeController.SwipeRight)
                direction = MapManager.Direction.Right;
            else
            {
                direction = MapManager.Direction.Pause;
                return;
            }
            Move();
        }
    }

    void  Move()
    {
        if(Target!=null && !isMoving)
        {
            isMove = true;
            targetPos = Target.position;
            searchPos = Target.position;
            switch (direction)
            {
                case MapManager.Direction.Up:
                    targetPos.z += .4f;
                    searchPos.z += .8f;
                    rotateY = 180;
                    break;
                case MapManager.Direction.Down:
                    targetPos.z -= .4f;
                    searchPos.z -= .8f;
                    rotateY = 0;
                    break;
                case MapManager.Direction.Left:
                    targetPos.x -= .4f;
                    searchPos.x -= .8f;
                    rotateY = 90;
                    break;
                case MapManager.Direction.Right:
                    targetPos.x += .4f;
                    searchPos.x += .8f;
                    rotateY = 270;
                    break;
                case MapManager.Direction.Pause:
                    isMove = false;
                    break;
            }
            if(isMoveAble)
            {
                Target.transform.rotation = Quaternion.Euler(new Vector3(0, rotateY, 0));
                if (MapManager.instance.IsGoalNode(targetPos, searchPos, direction))
                {
                    SoundManager.PlaySound(SoundManager.Sound.PlayerGoal, Target.transform.position);
                    GameManager.instance.RecordPlay(direction,Target.position, targetPos, searchPos);
                    MapManager.instance.GoalBlock(targetPos, searchPos);
                    StartCoroutine(Moving(targetPos));
                    GameManager.instance.AddMoveCount();
                    GameManager.instance.AddPushCount();


                }
                else if (MapManager.instance.IsMoveAbleNode(targetPos))
                {
                    SoundManager.PlaySound(SoundManager.Sound.PlayerMove, Target.transform.position);
                    GameManager.instance.RecordPlay(direction, Target.position, targetPos, Vector3.zero);
                    StartCoroutine(Moving(targetPos));
                    GameManager.instance.AddMoveCount();


                }
                else if(MapManager.instance.IsTranslateAbleNode(targetPos, searchPos, direction))
                {
                    SoundManager.PlaySound(SoundManager.Sound.PlayerPush, Target.transform.position);
                    GameManager.instance.RecordPlay(direction, Target.position, targetPos, searchPos);
                    MapManager.instance.MoveBlock(targetPos, searchPos);
                    StartCoroutine(Moving(targetPos));
                    GameManager.instance.AddMoveCount();
                    GameManager.instance.AddPushCount();
                }
                else
                {
                    SoundManager.PlaySound(SoundManager.Sound.PlayerUnableMove, Target.transform.position);
                    Vibrate();
                }
            }
        }
    }
    IEnumerator Moving(Vector3 targetPos)
    {
        isMoving = true;
        while (Target.position != targetPos)
        {
            Target.position = Vector3.MoveTowards(Target.position, targetPos, 15 * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }
        isMoving = false;
        yield return null;
    }
    public void UndoMove(PlayingRecord record)
    {
        switch ((MapManager.Direction)record.direction)
        {
            case MapManager.Direction.Up:
                Target.transform.rotation = Quaternion.Euler(new Vector3(0, 180, 0));
                break;
            case MapManager.Direction.Down:
                Target.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
                break;
            case MapManager.Direction.Left:
                Target.transform.rotation = Quaternion.Euler(new Vector3(0, 90, 0));
                break;
            case MapManager.Direction.Right:
                Target.transform.rotation = Quaternion.Euler(new Vector3(0, 270, 0));
                break;
        }
        // 큐브이동
        if (record.searchPos!=Vector3.zero)
        {


            if (MapManager.instance.IsGoalNode(record.searchPos, record.targetPos, (MapManager.Direction)record.direction))
            {
                MapManager.instance.GoalBlock(record.searchPos, record.targetPos);
                SoundManager.PlaySound(SoundManager.Sound.PlayerGoal, Target.position);
            }
            else
            {
                MapManager.instance.MoveBlock(record.searchPos, record.targetPos);
                SoundManager.PlaySound(SoundManager.Sound.PlayerPush, Target.position);
            }
            StartCoroutine(Moving(record.currentPos));
        }
        // 캐릭터만 이동
        else
        {
            SoundManager.PlaySound(SoundManager.Sound.PlayerMove, Target.position);
            StartCoroutine(Moving(record.currentPos));
        }
        GameManager.instance.RemoveMoveCount();
    }
    public void Jump()
    {
        if (!isJump)
            StartCoroutine("Jumping");
    }
    IEnumerator Jumping()
    {
        isJump = true;
        Vector3 targetPos = Target.position;
        for(int i = 0; i <3; i++)
        {
            targetPos.y = 1;
            float time = .0f;
            while (time < 0.2f)
            {
                Target.position = Vector3.Lerp(Target.position, targetPos, 7 * Time.deltaTime);
                time += Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }
            targetPos.y = 0.2f;
            while (Target.position.y > targetPos.y)
            {
                Target.position = Vector3.MoveTowards(Target.position, targetPos, 3 * Time.deltaTime);
                yield return new WaitForEndOfFrame();
            }
            Target.position = targetPos;
            yield return new WaitForSeconds(.1f);
        }
        isJump = false;
        yield return null;
    }
    void Vibrate()
    {
        if(ConfigurationData.vibrate)
        {
            Handheld.Vibrate();
            Debug.Log("이동불가, 진동발생!");
        }
    }
}
