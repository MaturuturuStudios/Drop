using UnityEngine;

public enum AnimationState {
    IDDLE,
    WALK,
    CHASE,
    DETECT,
    ATTACK,
    RUN_AWAY,
    HIDDE_RECOLECT
}

public interface AIStateListener{
    void OnStateAnimationChange(AnimationState previousState, AnimationState actualState);
}
