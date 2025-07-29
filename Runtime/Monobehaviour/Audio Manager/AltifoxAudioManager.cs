using UnityEngine;
using System.Collections.Generic;
using AltifoxTools;
using AltifoxAudio;
using UnityEngine.Audio;

public partial class AltifoxAudioManager : MonoBehaviour
{
    // Public fields
    [Header("-- Configuration --")]
    public int _numberOfSingleBufferSourcesToPool = 20;
    public int _numberOfDoubleBufferSourcesToPool = 20;
    public string _singleBufferAudioSourcesNamePrefix = "AltifoxAS__SB_";
    public string _doubleBufferAudioSourcesNamePrefix = "AltifoxAS__DB_";
    public static AltifoxAudioManager Instance { get; private set; }

    //Private fields
    private List<AltifoxAudioSource> audioSourcesSB = new List<AltifoxAudioSource>();

    private List<AltifoxDoubleBufferAudioSource> audioSourcesDB = new List<AltifoxDoubleBufferAudioSource>();

    private Dictionary<AltifoxSoundBase, int> SFXinstanceCount = new Dictionary<AltifoxSoundBase, int>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(this.gameObject);
        InitAltifoxAudioSources();
    }

    public void AddReferenceInCount(AltifoxSoundBase altifoxSFX)
    {
        if (SFXinstanceCount.TryGetValue(altifoxSFX, out int currentCount))
        {
            SFXinstanceCount[altifoxSFX] = currentCount + 1;
        }
        else
        {
            SFXinstanceCount.Add(altifoxSFX, 1);
        }
    }

    public void SubstractReferenceInCount(AltifoxSoundBase altifoxSFX)
    {
        if (SFXinstanceCount.TryGetValue(altifoxSFX, out int currentCount))
        {
            int newCount = currentCount - 1;

            if (newCount <= 0)
            {
                SFXinstanceCount.Remove(altifoxSFX);
            }
            else
            {
                SFXinstanceCount[altifoxSFX] = newCount;
            }
        }
    }

    public int GetSFXInstanceCount(AltifoxSoundBase altifoxSFX)
    {
        if (SFXinstanceCount.TryGetValue(altifoxSFX, out int currentCount))
        {
            return currentCount;
        }
        return 0;
    }

    // [ Audio source management]
    // --------------------------
    public AltifoxAudioSourceBase RequestSBAltifoxAudioSource()
    {
        foreach (AltifoxAudioSource AS in audioSourcesSB)
        {
            if (!AS.gameObject.activeSelf)
            {
                AS.gameObject.SetActive(true);
                return AS;
            }
        }
        Debug.LogWarning("trying to assign an audio source, but none from the pool is available!");
        return null;
    }

    public AltifoxAudioSourceBase RequestDBAltifoxAudioSource()
    {
        foreach (AltifoxAudioSourceBase AS in audioSourcesDB)
        {
            if (!AS.gameObject.activeSelf)
            {
                AS.gameObject.SetActive(true);
                return AS;
            }
        }
        Debug.LogWarning("trying to assign an audio source, but none from the pool is available!");
        return null;
    }

    private void InitAltifoxAudioSources()
    {
        for (int i = 0; i < _numberOfSingleBufferSourcesToPool; i++)
        {
            GameObject newAudioSourceObject = new GameObject(_singleBufferAudioSourcesNamePrefix + i);
            newAudioSourceObject.transform.SetParent(this.transform);
            AltifoxAudioSource newAudioSource = newAudioSourceObject.AddComponent<AltifoxAudioSource>();
            newAudioSourceObject.SetActive(false);
            audioSourcesSB.Add(newAudioSource);
        }

        for (int i = 0; i < _numberOfDoubleBufferSourcesToPool; i++)
        {
            GameObject newAudioSourceObject = new GameObject(_doubleBufferAudioSourcesNamePrefix + i);
            newAudioSourceObject.transform.SetParent(this.transform);
            AltifoxDoubleBufferAudioSource newAudioSource = newAudioSourceObject.AddComponent<AltifoxDoubleBufferAudioSource>();
            newAudioSourceObject.SetActive(false);
            audioSourcesDB.Add(newAudioSource);
        }
    }

    public void ReleaseAltifoxAudioSource(AltifoxAudioSourceBase sourceToRelease)
    {
        if (sourceToRelease != null)
        {
            sourceToRelease.Stop();
            sourceToRelease.clip = null;
            sourceToRelease.transform.position = this.transform.position;
            sourceToRelease.transform.SetParent(this.transform, false);
            sourceToRelease.gameObject.SetActive(false);
        }
    }
}
