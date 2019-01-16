using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
	public GameObject gameOverPanel;
	public FadePanel fadePanel;

	private LivingEntity mPlayerEntity;

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

	public void StartNewGame()
	{
		SceneManager.LoadScene("Game");
	}
}