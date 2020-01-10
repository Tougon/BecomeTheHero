using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Hero.Core;

/// <summary>
/// Represents a <see cref="Sequence"/> made up of an Entity's motion.
/// </summary>
public class AnimationSequence : Hero.Core.Sequence
{

    /// <summary>
    /// Represents an action that should be taken on a frame of animation
    /// </summary>
    public class AnimationSequenceAction
    {
        public enum Action { ChangeUserAnimation, ChangeTargetAnimation, TerminateAnimation, GenerateEffect, TerminateEffect,
           Move, Rotate, Scale, Color, Vibrate, ChangeAnimationSpeed, ChangeAnimationState, PlaySound, BeginLoop, EndLoop, ApplyDamage }

        public int frame;
        public Action action;
        public string param;
    }

    private bool initialized = false;
    private bool running;
    private bool looping;
    private int currentFrame = 0;
    private int loop = 1;
    private float directionX = 1;
    private float directionY = 1;

    public string sequenceName { get; private set; }

    private EntityController user;
    private EntityController target;
    private SpellCast spell;

    #region Initial Values
    /// <summary>
    /// Initial position of the user and target
    /// </summary>
    private Vector3 userPosition;
    private Vector3 targetPosition;
    private Vector3 userRotation;
    private Vector3 targetRotation;
    private Vector3 userScale;
    private Vector3 targetScale;
    private Color userColor;
    private Color targetColor;
    #endregion

    private SpriteRenderer userSprite;
    private SpriteRenderer targetSprite;

    private List<EntityController> effects = new List<EntityController>();

    private List<AnimationSequenceAction> sequenceActions = new List<AnimationSequenceAction>();
    private List<AnimationSequenceLoop> loops = new List<AnimationSequenceLoop>();


    /// <summary>
    /// Creates a sequence cast from the given spell with all loops matching the number of hits
    /// </summary>
    public AnimationSequence(AnimationSequenceObject obj, EntityController u, EntityController t, SpellCast s)
    {
        InitSequence(obj, u, t);
        spell = s;
        loop = s.GetNumHits();
    }


    /// <summary>
    /// Creates a sequence
    /// </summary>
    public AnimationSequence(AnimationSequenceObject obj, EntityController u, EntityController t)
    {
        InitSequence(obj, u, t);
    }


    /// <summary>
    /// Initializes a sequence
    /// </summary>
    public void InitSequence(AnimationSequenceObject obj, EntityController u, EntityController t)
    {
        user = u;
        target = t;

        // Iinitialize position of user and target
        userPosition = user.transform.position;
        userRotation = user.transform.eulerAngles;
        userScale = user.transform.localScale;
        userSprite = user.GetSpriteRenderer();
        userColor = userSprite.color;

        // Calculate direction of motion. This allows all animations to be uniform regardless of positioning.
        directionX = user.transform.localScale.x / Mathf.Abs(user.transform.localScale.x);
        directionY = user.transform.localScale.y / Mathf.Abs(user.transform.localScale.y);

        if (target!= null)
        {
            targetPosition = target.transform.position;
            targetRotation = target.transform.eulerAngles;
            targetScale = target.transform.localScale;
            targetSprite = target.GetSpriteRenderer();
            targetColor = targetSprite.color;
        }

        // Split the animation script.
        string[] sequence = obj.animationSequence.text.Split('\n');

        for(int i=0; i<sequence.Length; i++)
        {
            // Remove dividing characters
            string[] line = sequence[i].Split('|');

            // If line does not have enough values, show an error
            if(line.Length > 3 || line.Length < 2)
            {
                Debug.LogError("Invalid format on line " + (i+1) + "!");
                return;
            }

            // Create the action for the given line
            AnimationSequenceAction seq = new AnimationSequenceAction();

            seq.frame = int.Parse(line[0]);
            seq.action = (AnimationSequenceAction.Action)Enum.Parse(typeof(AnimationSequenceAction.Action), line[1]);
            
            // If the line length is greater than 2, add the param
            if (line.Length > 2)
                seq.param = line[2];

            // Add the action to our list
            sequenceActions.Add(seq);
        }

        initialized = true;
    }


    /// <summary>
    /// Start the sequence
    /// </summary>
    public override void SequenceStart()
    {
        if (!initialized)
            Debug.LogError("Sequence has not been initialized!");

        active = true;
        running = true;
    }


    public override IEnumerator SequenceLoop()
    {
        while (running)
        {
            // Increment frame
            currentFrame++;

            // If the current frame has an action associated, call it.
            for(int i=0; i<sequenceActions.Count; i++)
            {
                if (sequenceActions[i].frame != currentFrame)
                    continue;
                else
                    CallSequenceFunction(sequenceActions[i].action, sequenceActions[i].param);
            }

            yield return null;
        }

        SequenceEnd();
        active = false;
    }


    public override void SequenceEnd()
    {
        while(effects.Count > 0)
            effects.RemoveAt(0);

        user.transform.position = userPosition;
        user.transform.eulerAngles = userRotation;
        user.transform.localScale = userScale;
        userSprite.color = userColor;

        if(target != null)
        {
            target.transform.position = targetPosition;
            target.transform.eulerAngles = targetRotation;
            target.transform.localScale = targetScale;
            targetSprite.color = targetColor;
        }
    }



    #region Animation Events

    /// <summary>
    /// Runs specific behaviors based on the given action and its param.
    /// </summary>
    private void CallSequenceFunction(AnimationSequenceAction.Action a, string param)
    {
        switch (a)
        {
            case AnimationSequenceAction.Action.ChangeUserAnimation:
                ChangeUserAnimation(param);
                break;

            case AnimationSequenceAction.Action.ChangeTargetAnimation:
                ChangeTargetAnimation(param);
                break;

            case AnimationSequenceAction.Action.GenerateEffect:
                // Split the param
                string[] effectVals = param.Split(',');

                if (effectVals.Length != 12)
                    Debug.LogError("Invalid param count for Effect Generation!");

                // Generate an effect with the given values
                GenerateEffect(effectVals[0], effectVals[1], 
                    float.Parse(effectVals[2].Trim()), float.Parse(effectVals[3].Trim()), float.Parse(effectVals[4].Trim()),
                    float.Parse(effectVals[5].Trim()), float.Parse(effectVals[6].Trim()), float.Parse(effectVals[7].Trim()),
                    bool.Parse(effectVals[8].Trim()), float.Parse(effectVals[9].Trim()), float.Parse(effectVals[10].Trim()), 
                    float.Parse(effectVals[11].Trim()));
                break;

            case AnimationSequenceAction.Action.TerminateEffect:
                int id = int.Parse(param);
                TerminateEffect(id);
                break;

            case AnimationSequenceAction.Action.Move:
                // Split the param
                string[] moveVals = param.Split(',');

                if(moveVals.Length > 6 && moveVals.Length < 5)
                    Debug.LogError("Invalid param count for Movement!");

                Transform tM;
                string sM = moveVals[0].Trim();

                if (sM.Equals("User"))
                    tM = user.transform;
                else if (sM.Equals("Target"))
                    tM = target.transform;
                else
                    tM = effects[int.Parse(moveVals[5].Trim())].transform;

                float durationM = ((float.Parse(moveVals[1].Trim())) / 60.0f);

                float xM = (float.Parse(moveVals[2].Trim()) * directionX) + tM.position.x;
                float yM = (float.Parse(moveVals[3].Trim()) * directionY) + tM.position.y;
                float zM = float.Parse(moveVals[4].Trim()) + tM.position.z;

                // Move the target
                TweenPosition(tM, xM, yM, zM, durationM);
                break;

            case AnimationSequenceAction.Action.Rotate:
                // Split the param
                string[] rotateVals = param.Split(',');

                if (rotateVals.Length > 6 && rotateVals.Length < 5)
                    Debug.LogError("Invalid param count for Movement!");

                Transform tR;
                string sR = rotateVals[0].Trim();

                if (sR.Equals("User"))
                    tR = user.transform;
                else if (sR.Equals("Target"))
                    tR = target.transform;
                else
                    tR = effects[int.Parse(rotateVals[5].Trim())].transform;

                float durationR = ((float.Parse(rotateVals[1].Trim())) / 60.0f);

                float xR = float.Parse(rotateVals[2].Trim());
                float yR = float.Parse(rotateVals[3].Trim());
                float zR = (float.Parse(rotateVals[4].Trim()) * directionX);

                // Rotate the target
                TweenRotation(tR, xR, yR, zR, durationR);
                break;

            case AnimationSequenceAction.Action.Scale:
                // Split the param
                string[] scaleVals = param.Split(',');

                if (scaleVals.Length > 6 && scaleVals.Length < 5)
                    Debug.LogError("Invalid param count for Movement!");

                Transform tS;
                string sS = scaleVals[0].Trim();

                if (sS.Equals("User"))
                    tS = user.transform;
                else if (sS.Equals("Target"))
                    tS = target.transform;
                else
                    tS = effects[int.Parse(scaleVals[5].Trim())].transform;

                float durationS = ((float.Parse(scaleVals[1].Trim())) / 60.0f); ;

                // Increase the target scale
                float xS = !(sS.Equals("User") || sS.Equals("Target")) ? (float.Parse(scaleVals[2].Trim())) :
                    (float.Parse(scaleVals[2].Trim()) * directionX) * Mathf.Abs(tS.localScale.x);
                float yS = !(sS.Equals("User") || sS.Equals("Target")) ? (float.Parse(scaleVals[3].Trim())) :
                    (float.Parse(scaleVals[3].Trim()) * directionY) * Mathf.Abs(tS.localScale.y);
                float zS = float.Parse(scaleVals[4].Trim()) + tS.position.z;

                TweenScale(tS, xS, yS, zS, durationS);
                break;

            case AnimationSequenceAction.Action.Color:
                // Split the param
                string[] colorVals = param.Split(',');

                if (colorVals.Length > 7 && colorVals.Length < 6)
                    Debug.LogError("Invalid param count for Color!");

                EntityController eC;
                string sC = colorVals[0].Trim();

                if (sC.Equals("User"))
                    eC = user;
                else if (sC.Equals("Target"))
                    eC = target;
                else
                    eC = effects[int.Parse(colorVals[6].Trim())];

                float durationC = ((float.Parse(colorVals[1].Trim())) / 60.0f);

                float xC = float.Parse(colorVals[2].Trim());
                float yC = float.Parse(colorVals[3].Trim());
                float zC = float.Parse(colorVals[4].Trim());
                float wC = float.Parse(colorVals[5].Trim());

                // Change the target's color
                TweenColor(eC, new Color(xC, yC, zC, wC), durationC);
                break;

            case AnimationSequenceAction.Action.Vibrate:
                // Split the param
                string[] vibrateVals = param.Split(',');

                if (vibrateVals.Length > 6 && vibrateVals.Length < 5)
                    Debug.LogError("Invalid param count for Vibration!");

                Transform tV;
                string sV = vibrateVals[0].Trim();

                if (sV.Equals("User"))
                    tV = user.transform;
                else if (sV.Equals("Target"))
                    tV = target.transform;
                else
                    tV= effects[int.Parse(vibrateVals[5].Trim())].transform;

                float durationV = ((float.Parse(vibrateVals[1].Trim())) / 60.0f);
                Debug.Log(durationV);
                Vector3 strengthV = new Vector3(float.Parse(vibrateVals[2]), float.Parse(vibrateVals[3]), 0.0f);
                int vibratoV = int.Parse(vibrateVals[4]);

                // Vibrae the target
                Vibrate(tV, durationV, strengthV, vibratoV);
                break;

            case AnimationSequenceAction.Action.ChangeAnimationSpeed:
                // Split the param
                string[] speedVals = param.Split(',');

                if (speedVals.Length > 3 && speedVals.Length < 2)
                    Debug.LogError("Invalid param count for Speed!");
                
                string sSp = speedVals[0].Trim();
                float sSpeed = float.Parse(speedVals[1].Trim());

                // Modifies the speed of the target animator

                if (sSp.Equals("User"))
                    user.FrameSpeedModify(sSpeed);
                else if (sSp.Equals("Target"))
                    target.FrameSpeedModify(sSpeed);
                else
                    effects[int.Parse(speedVals[2].Trim())].FrameSpeedModify(sSpeed);
                break;

            case AnimationSequenceAction.Action.ChangeAnimationState:
                // Split the param
                string[] stateVals = param.Split(',');

                if (stateVals.Length > 4 && stateVals.Length < 3)
                    Debug.LogError("Invalid param count for State Change!");

                EntityController eAS;
                string sAS = stateVals[0].Trim();

                if (sAS.Equals("User"))
                    eAS = user;
                else if (sAS.Equals("Target"))
                    eAS = target;
                else
                    eAS = effects[int.Parse(stateVals[3].Trim())];

                eAS.SetAnimationState(stateVals[1].Trim(), bool.Parse(stateVals[2].Trim()));
                break;

            case AnimationSequenceAction.Action.BeginLoop:
                // Begins a loop
                int numLoops = int.Parse(param);

                if (numLoops < 0)
                    numLoops = loop;

                loops.Add(new AnimationSequenceLoop(currentFrame, numLoops));
                looping = true;
                break;

            case AnimationSequenceAction.Action.EndLoop:
                // Checks if the loop should repeat or terminate
                AnimationSequenceLoop currLoop = loops[loops.Count - 1];
                currLoop.numIterations++;

                if (currLoop.numIterations < currLoop.numLoops)
                    currentFrame = currLoop.startFrame;
                else
                    loops.Remove(currLoop);

                looping = loops.Count <= 0;
                break;

            case AnimationSequenceAction.Action.ApplyDamage:
                // Applies damage
                target.ApplyDamage(spell.GetDamageOfCurrentHit());
                break;

            case AnimationSequenceAction.Action.PlaySound:
                // Plays a sound effect
                PlaySound(param);
                break;

            case AnimationSequenceAction.Action.TerminateAnimation:
                // End the animation
                running = false;
                break;
        }
    }

    /// <summary>
    /// Change animator animation
    /// </summary>
    private void ChangeUserAnimation(string t) { user.SetAnimation(t.Trim()); }
    private void ChangeTargetAnimation(string t) { target.SetAnimation(t.Trim()); }
    

    /// <summary>
    /// Create an effect
    /// </summary>
    private void GenerateEffect(string path, string relative, float x, float y, float z, float scaleX, float scaleY, float scaleZ, bool match,
        float varX, float varY, float varZ)
    {
        path = path.Trim();
        relative = relative.Trim();
        Transform par = null;

        // Determines where the effect should spawn
        if(relative == "User")
        {
            Vector3 rel = user.transform.position;

            x = (rel.x) + (x * directionX);
            y = (rel.y) + (y * directionY);
            z += rel.z;

            if(match)
                par = user.transform;
        }
        else if(relative == "Target")
        {
            Vector3 rel = target.transform.position;

            x = (rel.x) + (x * directionX);
            y = (rel.y) + (y * directionY);
            z += rel.z;

            if(match)
                par = target.transform;
        }

        // Loads the effect
        GameObject effect = GameObject.Instantiate(Resources.Load(path, typeof(GameObject))) as GameObject;

        if (target != null)
            effect.transform.SetParent(par);

        varX /= 2;
        varY /= 2;
        varZ /= 2;

        // Sets positioning
        Vector3 offset = new Vector3(UnityEngine.Random.Range(-varX, varX), UnityEngine.Random.Range(-varY, varY), 
            UnityEngine.Random.Range(-varZ, varZ));

        effect.transform.position = new Vector3(x, y, z) + offset;
        effect.transform.localScale = match ? new Vector3(scaleX * directionX, scaleY * directionY, scaleZ) : 
            new Vector3(scaleX, scaleY, scaleZ);
        effects.Add(effect.GetComponent<EntityController>());
    }


    /// <summary>
    /// Disabled the given effect
    /// </summary>
    private void TerminateEffect(int id)
    {
        effects[id].gameObject.SetActive(false);
    }


    /// <summary>
    /// Tweens position to the target over the given duration
    /// </summary>
    private void TweenPosition(Transform t, float x, float y, float z, float duration)
    {
        t.DOMove(new Vector3(x, y, z), duration);
    }


    /// <summary>
    /// Tweens rotation to the target over the given duration
    /// </summary>
    private void TweenRotation(Transform t, float x, float y, float z, float duration)
    {
        t.DORotate(new Vector3(x, y, z), duration, RotateMode.Fast);
    }


    /// <summary>
    /// Tweens scale to the target over the given duration
    /// </summary>
    private void TweenScale(Transform t, float x, float y, float z, float duration)
    {
        t.DOScale(new Vector3(x, y, z), duration);
    }


    /// <summary>
    /// Tweens color to the target over the given duration
    /// </summary>
    private void TweenColor(EntityController e, Color c, float duration)
    {
        e.SetColorTween(c, duration);
    }


    /// <summary>
    /// Vibrates the target over the given duration
    /// </summary>
    private void Vibrate(Transform t, float duration, Vector3 strength, int vibrato)
    {
        t.transform.DOShakePosition(duration, strength, vibrato);
    }

    private void PlaySound(string s)
    {
        //SoundManager.Instance.PlaySound(s.Trim());
    }

    #endregion
}


/// <summary>
/// Data for an animation loop
/// </summary>
public class AnimationSequenceLoop
{
    public int startFrame;
    public int numLoops;
    public int numIterations;

    public AnimationSequenceLoop(int start, int loops)
    {
        startFrame = start;
        numLoops = loops;
        numIterations = 0;
    }
}
