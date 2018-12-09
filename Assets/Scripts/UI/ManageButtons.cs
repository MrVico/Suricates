using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ManageButtons : MonoBehaviour {

	[SerializeField] Button startButton;
	[SerializeField] Button pauseButton;
	[SerializeField] Button playSpeedButton;
	[SerializeField] Button musicButton;

	[SerializeField] Sprite normalSpeedSprite;
	[SerializeField] Sprite fastSpeedSprite;
	[SerializeField] Sprite playSprite;
	[SerializeField] Sprite pauseSprite;
	[SerializeField] Sprite soundSprite;
	[SerializeField] Sprite muteSprite;

	private float savedTimeScale;

	private void Start() {
		savedTimeScale = Time.timeScale;
	}

	// StartSimulation is called in ManageData 'cause it's easier

	// Called from UI, to pause/unpause the simulation
	public void PauseSimulation(){
		// We aren't paused
		if (Time.timeScale > 0) {
			savedTimeScale = Time.timeScale;
			Time.timeScale = 0f;
			pauseButton.image.sprite = playSprite;
		}
		// We are paused
		else {
			Time.timeScale = savedTimeScale;
			pauseButton.image.sprite = pauseSprite;
		}
	}

	// Called from UI, to reset the simulation
	public void ResetSimulation(){
		Time.timeScale = 1f;
		SceneManager.LoadScene(SceneManager.GetActiveScene().name);
	}

	// Called from UI, to change the play speed
	public void PlaySpeed(){
		// We are in fast mode
		if (Time.timeScale > 1f){
			Time.timeScale = 1f;
			playSpeedButton.image.sprite = fastSpeedSprite;
		}
		// We are in normal mode
		else if(Time.timeScale == 1f){
			Time.timeScale = 4f;
			playSpeedButton.image.sprite = normalSpeedSprite;
		}
		// We are in pause mode
		else if(Time.timeScale == 0){
			if (savedTimeScale > 1f){
				savedTimeScale = 1f;
				playSpeedButton.image.sprite = fastSpeedSprite;
			}
			// We are in normal mode
			else if(savedTimeScale == 1f){
				savedTimeScale = 4f;
				playSpeedButton.image.sprite = normalSpeedSprite;
			}
		}
	}

	public void Music(){	
		// Currently no music
		if(musicButton.image.sprite == muteSprite){
			musicButton.image.sprite = soundSprite;
		}	
		else{
			musicButton.image.sprite = muteSprite;
		}
	}
}
