using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Playables;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.Timeline;

public class ToggleAvatar : MonoBehaviour
{
    public TimelineAsset m_taSam, m_taLuna;
    public PlayableDirector m_playableDirector;
    public GameObject m_sam, m_luna;

    private AsyncOperationHandle<GameObject> m_hSam, m_hLuna;

    private AsyncOperationHandle<TimelineAsset> m_htaSam, m_htaLuna;
    // Start is called before the first frame update
    void Start()
    {
        m_playableDirector = gameObject.AddComponent<PlayableDirector>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private async void LoadTimeline(string _ta, string _avatar, int _type)
    {
        AsyncOperationHandle<TimelineAsset> handle =
            Addressables.LoadAssetAsync<TimelineAsset>(_ta);
        await handle.Task;
        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            TimelineAsset ta = handle.Result;
            if (_type == 0)
                m_htaSam = handle;
            else
            {
                m_htaLuna = handle;
            }
            LoadFbx(ta, _avatar, _type);
        }
    }
    private async void LoadFbx(TimelineAsset _ta, string _key, int type)
    {
        if (m_playableDirector.isActiveAndEnabled)
        {
            m_playableDirector.Stop();
            //m_playableDirector.ClearGenericBinding();
        }
        
        m_playableDirector.playableAsset = _ta;
        AsyncOperationHandle<GameObject> handle =
            Addressables.InstantiateAsync(_key);
        await handle.Task;
        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            GameObject fbxObject = handle.Result;
            // Do something with the loaded FBX object
            //_timeline.
            var track = _ta.GetOutputTrack(0);
            m_playableDirector.SetGenericBinding(track, fbxObject);
            m_playableDirector.Play();
            if (type == 0)
            {
                m_sam = fbxObject;
                m_hSam = handle;
            }
            else
            {
                m_luna = fbxObject;
                m_hLuna = handle;
            }
        }
        else
        {
            Debug.LogError($"Failed to load FBX: {handle.OperationException}");
        }
    }

    public void OnSam()
    {
        if (m_luna)
        {
            Object.Destroy(m_luna);
            Addressables.Release(m_htaLuna);
            Addressables.ReleaseInstance(m_hLuna);
            Resources.UnloadUnusedAssets();
            m_luna = null;
        }
        LoadTimeline("Assets/character/model/NPC_Sam/PlayerSam.playable"
            , "Assets/character/model/NPC_Sam/NPC_Sam.FBX", 0);
    }

    public void OnLuna()
    {
        if (m_sam)
        {
            Object.Destroy(m_sam);
            Addressables.Release(m_htaSam);
            Addressables.ReleaseInstance(m_hSam);
            Resources.UnloadUnusedAssets();
            m_sam = null;
        }
        LoadTimeline("Assets/character/model/NPC_Luna/PlayerLuna.playable"
            , "Assets/character/model/NPC_Luna/NPC_Luna.FBX", 1);
    }
}
