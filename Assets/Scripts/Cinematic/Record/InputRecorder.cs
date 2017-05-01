using UnityEngine;
using UnityEditor;

public class InputRecorder : MonoBehaviour {

	public string path = "Records";

	public string[] axisToRecord;

	private bool useFixedStep = true;

	private bool recording;
	private InputSequence recordingSequence;

	private GameControllerInput _gameControllerInput;

	void Awake() {
		_gameControllerInput = GameObject.FindGameObjectWithTag(Tags.GameController).GetComponent<GameControllerInput>();
	}

	void FixedUpdate() {
		if (recording && _gameControllerInput.IsInputEnabled())
			recordingSequence.RecordCurrentInput(_gameControllerInput.GetCurrentInputProvider(), axisToRecord);
	}

	public void Record() {
		if (!recording) {
			recording = true;
			recordingSequence = ScriptableObject.CreateInstance<InputSequence>();
			recordingSequence.timeStep = Time.fixedDeltaTime;

			_gameControllerInput.updateOnFixedStep = true;
		}
	}

	public InputSequence StopRecord() {
		InputSequence temp = recordingSequence;
		if (recording) {
			recording = false;
			recordingSequence = null;

			_gameControllerInput.updateOnFixedStep = false;
		}
		return temp;
	}

	public void SaveRecord(InputSequence sequence, string name) {
		string finalPath = "Assets/" + path + "/" + name + ".asset";

		AssetDatabase.CreateAsset(sequence, finalPath);

		AssetDatabase.SaveAssets();
		AssetDatabase.Refresh();
	}

	public bool IsRecording() {
		return recording;
	}

	public void SetUseFixedStep(bool value) {
		useFixedStep = value;
	}

	public void PlaySequence(InputSequence sequence) {
		_gameControllerInput.PlaySequence(sequence);
	}

	public void StopSequence() {
		_gameControllerInput.StopSequence();
	}

	public bool IsSequencePlaying() {
		return _gameControllerInput.IsSequencePlaying();
	}
}
