using UnityEngine;
using UnityEngine.UI;

public class InputRecorderUI : MonoBehaviour {

	[Header("References")]
	public InputRecorder recorder;

	public GameObject namePrompt;
	public InputField namePromptText;

	public Text playButton;
	public Text recordButton;

	public Text countdownText;

	public GameObject recordingLogo;
	public GameObject playingLogo;

	[Header("Sound clips")]
	public AudioClip countdownClip;
	public AudioClip startRecordClip;
	public AudioClip promptForNameClip;
	public AudioClip saveRecordClip;
	public AudioClip abortRecordClip;
	public AudioClip playSequenceClip;
	public AudioClip stopSequenceClip;
	public AudioClip noSequenceClip;

	[Header("Configuration")]
	public float timeToCountDown = 3;

	[Header("Sequence to play")]
	public InputSequence sequence;

	private bool countingDown;
	private float countdownTime;
	private int lastIntegerCountdown;

	private bool sequencePlaying = false;

	private AudioSource _audioSource;

	void Awake() {
		_audioSource = GetComponent<AudioSource>();
	}

	void Update () {
		if (Input.GetKeyDown(KeyCode.Home)) {
			ToggleRecord();
		}
		else if (recorder.IsRecording() && Input.GetKeyDown(KeyCode.End)) {
			_audioSource.clip = abortRecordClip;
			_audioSource.Play();
			sequence = recorder.StopRecord();
			Destroy(sequence);
			sequence = null;
		}
		else if (namePrompt.activeInHierarchy && (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)))
			SaveSequence();

		if (countingDown) {
			if (countdownTime <= 0) {
				_audioSource.clip = startRecordClip;
				_audioSource.Play();
				countdownText.gameObject.SetActive(false);
				countingDown = false;
				recorder.Record();
			}
			else {
				countdownTime -= Time.deltaTime;
				int integerCountdown = Mathf.CeilToInt(countdownTime);
				if (lastIntegerCountdown != integerCountdown) {
					_audioSource.clip = countdownClip;
					_audioSource.Play();
					countdownText.text = integerCountdown.ToString();
					lastIntegerCountdown = integerCountdown;
				}
			}
		}

		if (countingDown) {
			recordButton.text = "Cancelar (Inicio)";
			recordingLogo.SetActive(false);
		}
		else if (recorder.IsRecording()) {
			recordButton.text = "Finalizar / Abortar (Inicio / Fin)";
			recordingLogo.SetActive(true);
		}
		else {
			recordButton.text = "Grabar Input (Inicio)";
			recordingLogo.SetActive(false);
		}

		if (recorder.IsSequencePlaying()) {
			playButton.text = "Detener secuencia";
			playingLogo.SetActive(true);
		}
		else {
			playButton.text = "Reproducir secuencia";
			playingLogo.SetActive(false);

			if (sequencePlaying) {
				_audioSource.clip = stopSequenceClip;
				_audioSource.Play();
				sequencePlaying = false;
			}
		}
	}

	public void ToggleRecord() {
		if (countingDown) {
			countingDown = false;
			countdownText.gameObject.SetActive(false);
		}
		else if (recorder.IsRecording()) {
			sequence = recorder.StopRecord();
			PromptForName();
		}
		else {
			countingDown = true;
			countdownTime = timeToCountDown;
			countdownText.gameObject.SetActive(true);
		}
	}

	private void PromptForName() {
		namePrompt.SetActive(true);
		namePromptText.Select();
		namePromptText.ActivateInputField();
		recorder.StopInput();
		_audioSource.clip = promptForNameClip;
		_audioSource.Play();
	}

	public void SaveSequence() {
		recorder.SaveRecord(sequence, namePromptText.text);
		_audioSource.clip = saveRecordClip;
		_audioSource.Play();
		ExitPrompt();
	}

	public void ExitPrompt() {
		namePrompt.SetActive(false);
		recorder.ResumeInput();
	}

	public void PlayLastSequence() {
		if (!recorder.IsSequencePlaying()) {
			if (sequence != null) {
				recorder.PlaySequence(sequence);
				_audioSource.clip = playSequenceClip;
				_audioSource.Play();
				sequencePlaying = true;
			}
			else {
				_audioSource.clip = noSequenceClip;
				_audioSource.Play();
			}
		}
		else {
			recorder.StopSequence();
			_audioSource.clip = stopSequenceClip;
			_audioSource.Play();
			sequencePlaying = false;
		}
	}
}
