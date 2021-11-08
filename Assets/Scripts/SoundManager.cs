using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SoundManager : MonoBehaviour {

    public static SoundManager instance = null;

    public AudioMixer audioMixer;

    public float lowPitchRange = 0.95f;
    public float highPitchRange = 1.05f;

    //public AudioMixerGroup audio;

    private GameObject ConfigurationScreen;

    private float volumeMaster;
    private float volumeSFX;
    private float volumeMusic;

    private Slider sliderVolumeMaster;
    private Slider sliderVolumeSFX;
    private Slider sliderVolumeMusic;

    private Button buttonRestoreDefault;
    private Button buttonDone;

    private AudioSource soundtrack;

    private List<AudioSource> audioSourceEffects;

    void Awake() {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);

        soundtrack = gameObject.GetComponent<AudioSource>();

        Load();
    }

    void Start()
    {
        SetAudioMixer();
    }

    public void InitGame()
    {
        audioSourceEffects = new List<AudioSource>();
    }

    public void InitMenu()
    {
        audioSourceEffects = new List<AudioSource>();

        ConfigurationScreen = GameObject.Find("Canvas/ConfigurationScreen");
        
        sliderVolumeMaster = GameObject.Find("Canvas/ConfigurationScreen/BackgroundConfiguration/SliderVolumeMaster").GetComponent<Slider>();
        sliderVolumeSFX = GameObject.Find("Canvas/ConfigurationScreen/BackgroundConfiguration/SliderVolumeSFX").GetComponent<Slider>();
        sliderVolumeMusic = GameObject.Find("Canvas/ConfigurationScreen/BackgroundConfiguration/SliderVolumeMusic").GetComponent<Slider>();
        buttonRestoreDefault = GameObject.Find("Canvas/ConfigurationScreen/BackgroundConfiguration/ButtonRestoreDefault").GetComponent<Button>();
        buttonDone = GameObject.Find("Canvas/ConfigurationScreen/BackgroundConfiguration/ButtonDone").GetComponent<Button>();

        sliderVolumeMaster.onValueChanged.AddListener(SetVolumeMaster);
        sliderVolumeMaster.value = volumeMaster;
        sliderVolumeSFX.onValueChanged.AddListener(SetVolumeSFX);
        sliderVolumeSFX.value = volumeSFX;
        sliderVolumeMusic.onValueChanged.AddListener(SetVolumeMusic);
        sliderVolumeMusic.value = volumeMusic;

        buttonRestoreDefault.onClick.AddListener(RestoreDefault);
        buttonDone.onClick.AddListener(Done);

        ConfigurationScreen.SetActive(false);
        
    }

    public void PlayerMusic(AudioClip music)
    {
        soundtrack.clip = music;
        soundtrack.PlayDelayed(1f);
    }

    public void StopMusic()
    {
        soundtrack.Stop();
    }

    public void SetVolumeMaster(float volumeMaster)
    {
        this.volumeMaster = volumeMaster;
        SetAudioMixer();
    }

    public void SetVolumeSFX(float volumeSFX)
    {
        this.volumeSFX = volumeSFX;
        SetAudioMixer();
    }

    public void SetVolumeMusic(float volumeMusic)
    {
        this.volumeMusic = volumeMusic;
        SetAudioMixer();
    }

    public void SetAudioMixer()
    {
        audioMixer.SetFloat("MasterVol", volumeMaster);
        audioMixer.SetFloat("SFXVol", volumeSFX);
        audioMixer.SetFloat("MusicVol", volumeMusic);
    }

    public void RestoreDefault()
    {
        volumeMaster = 0;
        volumeSFX = 0;
        volumeMusic = 0;

        sliderVolumeMaster.value = volumeMaster;
        sliderVolumeSFX.value = volumeSFX;
        sliderVolumeMusic.value = volumeMusic;

        SetAudioMixer();
    }

    public void Done()
    {
        SetAudioMixer();
        Save();
    }

    public void SetAudioEffectPerson(AudioSource audioSource)
    {
        audioSource.loop = false;
        audioSource.pitch = Random.Range(lowPitchRange, highPitchRange);

        bool playing = false;

        foreach (AudioSource audio in audioSourceEffects)
        {
            if (audio.clip == audioSource.clip)
            {
                playing = true;
            }
        }
        if (!playing)
        {
            audioSourceEffects.Add(audioSource);
            audioSource.Play();
        }
    }

    public void RemoveAudioEffectPerson()
    {
        for(int i = 0; i < audioSourceEffects.Count; i++)
        {
            if (!audioSourceEffects[i].isPlaying)
            {
                audioSourceEffects.Remove(audioSourceEffects[i]);
            }
        }
    }

    public void Save()
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/773HSaveSoundConfiguration.ssth");

        SaveSoundConfiguration saveSound = new SaveSoundConfiguration();
        saveSound.volumeMaster = volumeMaster;
        saveSound.volumeSFX = volumeSFX;
        saveSound.volumeMusic = volumeMusic;

        bf.Serialize(file, saveSound);
        file.Close();
    }

    public void Load()
    {
        if (File.Exists(Application.persistentDataPath + "/773HSaveSoundConfiguration.ssth"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/773HSaveSoundConfiguration.ssth", FileMode.Open);
            SaveSoundConfiguration saveSound = (SaveSoundConfiguration)bf.Deserialize(file);
            file.Close();

            volumeMaster = saveSound.volumeMaster;
            volumeSFX = saveSound.volumeSFX;
            volumeMusic = saveSound.volumeMusic;
        }
        else
        {
            volumeMaster = 0;
            volumeSFX = 0;
            volumeMusic = 0;
        }
    }
}

[System.Serializable]
public class SaveSoundConfiguration
{
    public float volumeMaster;
    public float volumeSFX;
    public float volumeMusic;
}
