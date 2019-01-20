using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
	public GameObject gameOverPanel;
	public FadePanel fadePanel;

	public RectTransform waveInfoBanner;
	public Text TextWaveTitle;
	public Text TextWaveEnemyCount;

	private LivingEntity mPlayerEntity;
	private Spawner mWaveSpawner;

	void Awake()
	{
		mWaveSpawner = FindObjectOfType<Spawner>();
		mWaveSpawner.OnNewWave += OnNewWave;
	}

	void Start()
	{
		mPlayerEntity = FindObjectOfType<Player>();
		mPlayerEntity.OnDeath += OnGameOver;
	}

	void OnGameOver()
	{
		Cursor.visible = true;
		fadePanel.DoFade(Color.clear, new Color(0, 0, 0, 0.9f), 1.5f);
		gameOverPanel.SetActive(true);
	}

	void OnNewWave(int waveIndex)
	{
		Spawner.Wave wave = mWaveSpawner.GetWaveByIndex(waveIndex);
		TextWaveTitle.text = "- " + wave.tag + " -";
		TextWaveEnemyCount.text = "Enemies: " + (wave.infinite ? "∞" : wave.enemyCount+"");

		StopCoroutine("AnimWaveInfoBanner");
		StartCoroutine("AnimWaveInfoBanner");
	}

	private IEnumerator AnimWaveInfoBanner()
	{
		float progress = 0.0f;
		float speed = 2.5f;
		float delayTime = 1.0f;
		float endDelayTime = Time.time + 1f / speed + delayTime;
		int moveDir = 1;
	
		waveInfoBanner.gameObject.SetActive(true);
		while (progress >= 0.0f)
		{
			progress += Time.deltaTime * speed * moveDir;
			if (progress >= 1.0f)
			{
				progress = 1.0f;
				if (Time.time > endDelayTime)
				{
					moveDir = -1;
				}
			}
			waveInfoBanner.anchoredPosition = Vector2.up * Mathf.Lerp(-270f, 0f, progress);
			yield return null;
		}
		waveInfoBanner.gameObject.SetActive(false);
	}

	//For "Play Agin" button event.
	public void StartNewGame()
	{
		SceneManager.LoadScene("Game");
	}
}