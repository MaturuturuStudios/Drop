using UnityEngine;
using System.Collections.Generic;

public class InputSequence : ScriptableObject {

	public List<InputRecord> recordSequence = new List<InputRecord>();

	public float timeStep;

	public float beginAt;

	public float endAt;

	public InputRecord GetInputAtTime(float time) {
		if (IsOver(time))
			return null;
		
		return recordSequence[GetInputIndex(time)];
	}

	public void RecordCurrentInput(InputProvider provider, string[] axisToRecord) {
		InputRecord record = new InputRecord(provider, axisToRecord);
		recordSequence.Add(record);
		endAt += timeStep;
	}

	public bool IsOver(float time) {
		return time >= (endAt - beginAt);
	}

	private int GetInputIndex(float time) {
		return Mathf.FloorToInt((time + beginAt) / timeStep);
	}
}
