using System;

[Serializable]
public class CustomDictionary : SerializableDictionary<string, InputRecordForAxis> { }

[Serializable]
public class InputRecord : InputProvider {
	
	public CustomDictionary records = new CustomDictionary();

	public InputRecord(InputProvider provider, string[] axisToRecord) {
		foreach (string axis in axisToRecord)
			records.Add(axis, new InputRecordForAxis(provider, axis));
	}

	public float GetAxis(string input) {
		if (records.ContainsKey(input))
			return records[input].axis;
		return 0;
	}

	public float GetAxisRaw(string input) {
		if (records.ContainsKey(input))
			return records[input].axisRaw;
		return 0;
	}

	public bool GetButtonDown(string input) {
		if (records.ContainsKey(input))
			return records[input].buttonDown;
		return false;
	}

	public bool GetButtonUp(string input) {
		if (records.ContainsKey(input))
			return records[input].buttonUp;
		return false;
	}
}
