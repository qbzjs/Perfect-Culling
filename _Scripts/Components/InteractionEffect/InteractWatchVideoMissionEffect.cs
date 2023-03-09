using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class InteractWatchVideoMissionEffect : InteractMissionEffect
{
    public static RecordMissionDailyInfo record_MissionDaily ;
    public override void Init(GameObject ob1, ResponseInteraction ob2, Action on_done)
    {
        onDone = on_done;
        RecordMissionDailyInfo record_mission = new RecordMissionDailyInfo();
        RecordMissionDailyInfo[] recordMissionDailyInfos = QuestManager.getRecordMissionDailyInfoArray;
        int length = recordMissionDailyInfos.Length;
        for (int i = 0; i < length; i++)
        {
            RecordMissionDailyInfo record = recordMissionDailyInfos[i];
            if (record.target_object_name.ToLower().Contains("tivi"))
            {
                record_mission = record;
            }
        }
        Dictionary<KeyCode, List<string>> listAvailableKey = new Dictionary<KeyCode, List<string>>();
        listAvailableKey.Add(KeyCode.Space, null);
        InputRegisterEvent.Instance.SetlstKeyAvailable(listAvailableKey);
        PlayVideo(ob1,ob2, record_mission.video_name, record_mission);
    }
    private void PlayVideo(GameObject ob1,ResponseInteraction ob2,string video_Name, RecordMissionDailyInfo record_mission)
    {
        VideoPlayer videoPlayer = ob2.transform.GetChild(0).GetComponent<VideoPlayer>();
        PlayVideo(ob1, ob2, videoPlayer,video_Name, record_mission);
    }

    private void PlayVideo(GameObject ob1, ResponseInteraction ob2, VideoPlayer video,string video_Name, RecordMissionDailyInfo record_mission)
    {
        VideoManager.instance.RegisterVideo(video_Name, (url, url_local) =>
        {
            StartCoroutine(PrepareVideo(ob1, ob2, video, url_local, record_mission));
        });
        VideoManager.instance.StartDownloadVideo();
    }

    IEnumerator PrepareVideo(GameObject ob1, ResponseInteraction ob2, VideoPlayer video_player, string url, RecordMissionDailyInfo record_mission)
    {
        video_player.errorReceived += VideoPlayer_errorReceived;
        video_player.source = VideoSource.Url;
        video_player.url = url;
        video_player.Prepare();
        yield return new WaitUntil(() => video_player.isPrepared);
        TPRLSoundManager.Instance.SetMute(true);
        video_player.Play();
        video_player.isLooping = false;
        float yOffsetBetweenWatchMissionPointAndPlayer = 3f;
        Vector3 targetPosition = new Vector3(record_mission.target_position[0], record_mission.target_position[1] - yOffsetBetweenWatchMissionPointAndPlayer, record_mission.target_position[2]);
        Vector3 cameraPosition = new Vector3(record_mission.camera_position[0], record_mission.camera_position[1], record_mission.camera_position[2]);
        Quaternion cameraRotation = Quaternion.Euler(new Vector3(record_mission.camera_rotation[0], record_mission.camera_rotation[1], record_mission.camera_rotation[2]));
        CharacterController characterController = ob1.GetComponent<CharacterController>();
        characterController.enabled = false;
        ob1.transform.localRotation = Quaternion.identity;
        ob1.transform.localPosition = targetPosition;
        characterController.enabled = true;
        CameraRotationComponent cameraRotationComponent = ob1.GetComponent<CameraRotationComponent>();
        cameraRotationComponent.RotationFromDefaultPositionToNewPosition(cameraPosition, cameraRotation);
        yield return new WaitUntil(() => !video_player.isPlaying);
        cameraRotationComponent.RotationToDefaultPosition();
        //QuestManager.CompleteQuest(true);
        QuestManager.CompleteQuestDaily(record_MissionDaily,true);
        //QuestManager.InitMission(record_mission.next_mission_id);
        TPRLSoundManager.Instance.SetMute(false);
        Destroy(ob2.GetComponent<ResponseWatchVideoMissionInteractionComponent>());
    }
    void VideoPlayer_errorReceived(VideoPlayer source, string message)
    {
        Debug.Log("error: " + message);
    }
}
