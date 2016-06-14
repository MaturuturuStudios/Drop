using UnityEngine;

/// <summary>
/// Interface for the observers listening for the
/// character shoot's events.
/// </summary>
public interface CharacterShootListener {

    /// <summary>
    /// Event called when a character enters shoot mode.
    /// </summary>
    /// <param name="character">The character entering shoot mode</param>
    void OnEnterShootMode(CharacterShoot character);

    /// <summary>
    /// Event called when a character exits shoot mode.
    /// </summary>
    /// <param name="character">The character exiting shoot mode</param>
    void OnExitShootMode(CharacterShoot character);

    /// <summary>
    /// Event called when the character shoots.
    /// </summary>
    /// <param name="shootingCharacter">The character who is shooting</param>
    /// <param name="shotCharacter">The character who is shoot</param>
    /// <param name="velocity">The velocity the character is shot with</param>
    void OnShoot(CharacterShoot shootingCharacter, GameObject shotCharacter, Vector3 velocity);
}

/// <summary>
/// Adapter for the CharacterShootListener interface used to
/// avoid forcing each class to implement all it's methods.
/// </summary>
public class CharacterShootAdapter : MonoBehaviour, CharacterShootListener {

    public virtual void OnEnterShootMode(CharacterShoot character) {
        // Do nothing
    }

    public virtual void OnExitShootMode(CharacterShoot character) {
        // Do nothing
    }

    public virtual void OnShoot(CharacterShoot shootingCharacter, GameObject shotCharacter, Vector3 velocity) {
        // Do nothing
    }
}
