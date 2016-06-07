using UnityEngine;

/// <summary>
/// This class is for the canon plant shoot
/// </summary>
[ExecuteInEditMode]
public class CannonShoot : ActionPerformer {
    public LaunchCharacter launch;
	/// <summary>
	/// Unity's method called each frame.
	/// </summary>
	public void Update(){
		transform.eulerAngles = new Vector3(0, 0, launch.GetAngle()); //this is to face in the direction you are aming
	}

	protected override bool OnAction(GameObject character)	{
        CharacterControllerCustom ccc = character.GetComponent<CharacterControllerCustom>();

        ccc.transform.position = this.transform.position;
        ccc.Stop();
        ccc.SendFlying(launch.GetNeededVelocityVector());

		return true;
    }

    public void ChangeAngleCannon(float newAngle) {
        launch.SetAngle(newAngle);
    }

    public void OnDrawGizmos() {
        launch.OnDrawGizmos();
    }
}
