// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using GooglePlayGames;
// using GooglePlayGames.BasicApi;

// public class GooglePlayManager : MonoBehaviour
// {
//     public static GooglePlayManager Instance;

//     void Awake()
//     {
//         Instance = this;
//         PlayGamesPlatform.InitializeInstance(new PlayGamesClientConfiguration.Builder().Build());
//         PlayGamesPlatform.DebugLogEnabled = true;
//         PlayGamesPlatform.Activate();
//         OnLogin();
//     }
//     public void OnLogin()
//     {
//         if (!Social.localUser.authenticated)
//         {
//             Social.localUser.Authenticate((bool bSuccess) =>
//             {
//                 if (bSuccess)
//                 {
//                     Debug.Log("Success : " + Social.localUser.userName);
//                 }
//                 else
//                 {
//                     Debug.Log("Fall");
//                 }
//                 GameSystemManager.Instance.isGPGSLoad = true;
//             });
//         }
//     }

//     public void OnLogOut()
//     {
//         ((PlayGamesPlatform)Social.Active).SignOut();
//     }

//     // 리더보드에 점수등록 후 보기
//     public void OnShowLeaderBoard()
//     {
//         bool success = true;
//         // 현재 스테이지 랭킹
//         Social.ReportScore(PlayerData.currentStage, GPGSIds.leaderboard_current_stage, (bool bSuccess) =>
//         {
//             success &= bSuccess;
//         });
//         // 퍼펙트 클리어 랭킹
//         Social.ReportScore(PlayerData.clearStageInfoList.FindAll(x => x.stageNumber > 0 && x.totalMove == int.Parse(MapDataManager.GetMapClearCount(x.stageNumber))).Count, GPGSIds.leaderboard_clear_minimum_number_of_moves, (bool bSuccess) =>
//         {
//             success &= bSuccess;
//         });
//         // 취소 횟수
//         Social.ReportScore(PlayerData.undoTryCount, GPGSIds.leaderboard_total_undo, (bool bSuccess) =>
//         {
//             success &= bSuccess;
//         });
//         // 재시도 횟수
//         Social.ReportScore(PlayerData.retryCount, GPGSIds.leaderboard_total_retries, (bool bSuccess) =>
//         {
//             success &= bSuccess;
//         });
//         // 플레이 타임
//         Social.ReportScore((long)PlayerData.totalPlayTime, GPGSIds.leaderboard_total_play_time, (bool bSuccess) =>
//         {
//             success &= bSuccess;
//         });
//         if (success)
//             Debug.Log("성공");
//         else
//             Debug.Log("리더보드 열기 실패");
//         SoundManager.PlaySound(SoundManager.Sound.AlertPopup);
//         Social.ShowLeaderboardUI();
//     }
//     // 업적보기
//     public void OnShowAchievement()
//     {
//         SoundManager.PlaySound(SoundManager.Sound.AlertPopup);
//         Social.ShowAchievementsUI();
//     }

//     // 업적추가
//     public void OnAddAchievement(int archiveNumber)
//     {
//         string[] achivementIds = { GPGSIds.achievement_welcome_to_push_cube, GPGSIds.achievement_push_cube_master, GPGSIds.achievement_push_cube_expert, GPGSIds.achievement_push_cube__friends_of_all, GPGSIds.achievement_push_cube_legend, GPGSIds.achievement_push_cube__challenger, GPGSIds.achievement_weclome_to_hobook_games };
//         string archivementIds = achivementIds[archiveNumber];
//         Social.ReportProgress(archivementIds, 100.0f, (bool bSuccess) =>
//         {
//             if (bSuccess)
//                 Debug.Log(archivementIds);
//             else
//                 Debug.Log("AddAchievement Fall");
//         });
//     }
// }

