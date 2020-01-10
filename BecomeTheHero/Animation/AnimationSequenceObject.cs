using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// An object that contains the text data for an <see cref="AnimationSequence"/>
/// </summary>
[CreateAssetMenu(fileName = "NewAnimationSequence", menuName = "Animation/Animation Sequence Object", order = 2)]
public class AnimationSequenceObject : ScriptableObject
{
    public string animationName = "";
    public TextAsset animationSequence;
}
