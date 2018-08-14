using UnityEngine;
using System.Collections;

public class AudioManager : MonoBase
{
	static AudioManager m_Instance;

	const float MAX_MUSIC_VOLUME = 0.5F;
	private int lastMusicId;
	private int lastAudioId;
	private int lastAudioTIme;
	private ArrayList loadingList;


	public static AudioManager Instance {
		get {
			if (m_Instance == null) {
				m_Instance = GameObject.Find ("AudioManager").GetComponent<AudioManager> ();
			}
			return m_Instance;
		}
		set {
			m_Instance = value;
		}
		
	}
	
	/// <summary>
	/// 获取或设置背景音乐开关
	/// </summary>
	public  bool IsMusicOpen {
		get {
			return isMusicOpen;
		}
		set {
			PlayerPrefs.SetInt ("AudioManager_IsMusicOpen", value ? 1 : 0);
			PlayerPrefs.Save ();
			isMusicOpen = value;
			if (value) {
				stopMusic = false;
				audioSource.volume = MAX_MUSIC_VOLUME;
			} else {
				stopMusic = true;
			}
		}
	}
	
	/// <summary>
	/// 获取或设置音效开关
	/// </summary>
	public  bool IsAudioOpen {
		get {
			return isAudioOpen;
		}
		set {
			PlayerPrefs.SetInt ("AudioManager_IsAudioOpen", value ? 1 : 0);
			PlayerPrefs.Save ();
			isAudioOpen = value;
		}
	}
	
	/// <summary>
	/// 播放音乐
	/// </summary>
	/// <param name='id'>
	/// 音乐文件的文件名
	/// </param>
	public  void PlayMusic (int id)
	{
		if (AudioManager.Instance == null || AudioManager.Instance.audioSource == null) 
			return;

		if (lastMusicId == id)
			return;
		lastMusicId = id;
		
		float delayTime = 0;
		if (AudioManager.Instance.audioSource.clip != null) {
			Instance.stopMusic = true;
			delayTime = 1f;
		}
		AudioManager.Instance.StartCoroutine (Utils.DelayRun (() =>
		{

			if (ResourcesManager.Instance.allowLoadFromRes) {
				//直接读取资源
				AudioManager.Instance.audioSource.clip = Resources.Load ("audio/audio_" + id, typeof(AudioClip)) as AudioClip;
				AudioManager.Instance.audioSource.Play ();
				if (AudioManager.Instance.IsMusicOpen)
					AudioManager.Instance.audioSource.volume = MAX_MUSIC_VOLUME;
			} else {
				ResourcesData data = ResourcesManager.Instance.getResource ("audio/audio_" + id);
				if (data == null) {
					//支线读取等回调
					ResourcesManager.Instance.cacheData ("audio/audio_" + id, (list) => {
						data = ResourcesManager.Instance.getResource ("audio/audio_" + id);
						//缓存了读还是空,放弃
						if (data == null)
							return;

						AudioManager.Instance.audioSource.clip = data.ResourcesBundle.mainAsset as AudioClip;
						AudioManager.Instance.audioSource.Play ();
						if (AudioManager.Instance.IsMusicOpen)
							AudioManager.Instance.audioSource.volume = MAX_MUSIC_VOLUME;
					}, "base");


				} else {
					Instance.audioSource.clip = data.ResourcesBundle.mainAsset as AudioClip;
					Instance.audioSource.Play ();
					if (AudioManager.Instance.IsMusicOpen)
						Instance.audioSource.volume = MAX_MUSIC_VOLUME;

				}
			}
 

		}, delayTime));
	}
	
	/// <summary>
	/// 播放音效
	/// </summary>
	/// <param name='id'>
	/// 音效文件的文件名
	/// </param>
	/// <param name='downMusic'>
	/// 是否压低背景音乐
	/// </param>
	public void PlayAudio (int id)
	{
//		Debug.LogWarning("IsAudioOpen+1111111111111111111!");
		if (!AudioManager.Instance.IsAudioOpen)
			return;
		if (lastAudioId == id && Time.renderedFrameCount == lastAudioTIme) {
			return;
		}

		if (ResourcesManager.Instance.allowLoadFromRes) {
			AudioClip clip = Resources.Load ("audio/audio_" + id) as AudioClip;
			if (clip != null) {
				lastAudioTIme = Time.renderedFrameCount;
				lastAudioId = id;
				Instance.NextAudioSource ().PlayOneShot (clip);
			}
		} else {
			ResourcesData data = ResourcesManager.Instance.getResource ("audio/audio_" + id);
			if (data == null) {
				if(isLoading(id))
					return;
				addLoading(id);
				//支线读取等回调
				ResourcesManager.Instance.cacheData ("audio/audio_" + id, (list) => {
					delLoading(id);
					data = ResourcesManager.Instance.getResource ("audio/audio_" + id);
					//缓存了读还是空,放弃
					if (data == null)
						return;
					AudioClip clip = data.ResourcesBundle.mainAsset as AudioClip;
					if (clip != null) {
						lastAudioTIme = Time.renderedFrameCount;
						lastAudioId = id;
						Instance.NextAudioSource ().PlayOneShot (clip);
					}
				}, "other");
				
			} else {
				AudioClip clip = data.ResourcesBundle.mainAsset as AudioClip;
				if (clip != null) {
					lastAudioTIme = Time.renderedFrameCount;
					lastAudioId = id;
					Instance.NextAudioSource ().PlayOneShot (clip);
				}
			}
		}
	}

	public int maxAudioCount = 3; //最多同时播放的音效数量
	
	AudioSource audioSource;
	bool isMusicOpen;
	bool isAudioOpen;
	bool stopMusic;
	bool startMusic;
	AudioSource[] sources;
	int curAudioIndex;

	void Awake ()
	{
		DontDestroyOnLoad (gameObject);
	}

	public void init ()
	{
		audioSource = GetComponent<AudioSource> ();
		sources = new AudioSource[maxAudioCount];
		for (int i = 0; i < maxAudioCount; i++) {
			GameObject obj = NGUITools.AddChild (gameObject);
			obj.name = i.ToString ();
			AudioSource sc = obj.AddComponent<AudioSource> ();
			sc.playOnAwake = false;
			sources [i] = sc;
		}
		loadSettnigs ();
		if (!isMusicOpen)
			audioSource.volume = 0;

		PlayMusic (1);
	}
	
	AudioSource NextAudioSource ()
	{
		if (curAudioIndex >= maxAudioCount) {
			curAudioIndex = 0;
		}
		return sources [curAudioIndex++];
	}

	
	// Update is called once per frame
	void Update ()
	{
		if (stopMusic) {
			audioSource.volume -= 0.02f;
			if (audioSource.volume <= 0) {
				stopMusic = false;
				audioSource.volume = 0f;
			}
		}
	}
	
	//读档
	private void loadSettnigs ()
	{
	
		IsMusicOpen = PlayerPrefs.GetInt ("AudioManager_IsMusicOpen", 1) == 1;
		IsAudioOpen = PlayerPrefs.GetInt ("AudioManager_IsAudioOpen", 1) == 1;
		Debug.LogWarning ("------------------------ Load Audio Settings ------------------------");
		//	Debug.LogWarning ("----IsMusicOpen ----- "+IsMusicOpen);
		//	Debug.LogWarning ("----IsAudioOpen ----- "+IsAudioOpen);
	}

	private void addLoading (object name)
	{
		if (loadingList == null)
			loadingList = new ArrayList ();
		if (loadingList.Contains (name))
			return;
		loadingList.Add (name);
	}

	private void delLoading (object name)
	{
		if (loadingList == null || loadingList.Count < 1)
			return;
		loadingList.Remove (name);
	}

	private bool isLoading (object name)
	{
		if (loadingList == null || loadingList.Count < 1)
			return false;
		return loadingList.Contains (name);
	}
}
