using UnityEngine;

/// <summary>
/// This class is for the canon plant shoot
/// </summary>
[ExecuteInEditMode]
public class CannonShoot : LaunchCharacter {

	/// <summary>
	/// Unity's method called each frame.
	/// </summary>
	public void Update(){
		transform.eulerAngles = new Vector3(0, 0, GetAngle()); //this is to face in the direction you are aming
	}

	protected override bool OnAction(GameObject character)	{
        CharacterControllerCustom ccc = character.GetComponent<CharacterControllerCustom>();

        ccc.transform.position = this.transform.position;
        ccc.Stop();
        ccc.SendFlying(GetNeededVelocityVector());

		return true;
    }

    public void ChangeAngleCannon(float newAngle) {
        SetAngle(newAngle);
    }
}
