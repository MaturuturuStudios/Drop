using System;

[Serializable]
public class InputRecordForAxis {

	public float axis;
	public float axisRaw;
	public bool buttonDown;
	public bool buttonUp;

	public InputRecordForAxis(InputProvider provider, string axisName) {
		axis = provider.GetAxis(axisName);
		axisRaw = provider.GetAxisRaw(axisName);
		buttonDown = provider.GetButtonDown(axisName);
		buttonUp = provider.GetButtonUp(axisName);
	}
}
