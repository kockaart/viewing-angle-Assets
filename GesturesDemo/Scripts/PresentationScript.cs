using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PresentationScript : MonoBehaviour
{
    public bool slideChangeWithGestures = true;
    public bool slideChangeWithKeys = true;
    public float spinSpeed = 5;

    public bool autoChangeAlfterDelay = false;
    public float slideChangeAfterDelay = 10;

    public List<GameObject> horizontalSides;

    [System.Serializable]
    public class TextureList
    {
        public Texture[] tex;

    }
    public TextureList[] texList;

    // if the presentation cube is behind the user (true) or in front of the user (false)
    public bool isBehindUser = false;

    private int maxSides = 0;
    private int side = 0;

    private int menuX = 0;
    private int menuY = 0;

    private bool isSpinning = false;
    private float slideWaitUntil;
    private Quaternion targetRotation;

    private GestureListener gestureListener;

    void Start()
    {
        // hide mouse cursor
        Screen.showCursor = false;

        // calculate max slides
        maxSides = horizontalSides.Count;

        // delay the first slide
        slideWaitUntil = Time.realtimeSinceStartup + slideChangeAfterDelay;

        targetRotation = transform.rotation;
        isSpinning = false;

        side = 0;
        menuX = 0;
        menuY = 0;

        if (horizontalSides[side] && horizontalSides[side].renderer)
        {
            horizontalSides[side].renderer.material.mainTexture = texList[menuX].tex[menuY];
        }

        // get the gestures listener
        gestureListener = Camera.main.GetComponent<GestureListener>();
    }

    void Update()
    {
        // dont run Update() if there is no user
        KinectManager kinectManager = KinectManager.Instance;
        if (autoChangeAlfterDelay && (!kinectManager || !kinectManager.IsInitialized() || !kinectManager.IsUserDetected()))
            return;

        if (!isSpinning)
        {
            if (slideChangeWithKeys)
            {
                if (Input.GetKeyDown(KeyCode.RightArrow))
                    RotateToNext();
                else if (Input.GetKeyDown(KeyCode.LeftArrow))
                    RotateToPrevious();
                else if (Input.GetKeyDown(KeyCode.UpArrow))
                    RotateUp();
                else if (Input.GetKeyDown(KeyCode.DownArrow))
                    RotateDown();
            }

            if (slideChangeWithGestures && gestureListener)
            {
                if (gestureListener.IsSwipeLeft())
                    RotateToNext();
                else if (gestureListener.IsSwipeRight())
                    RotateToPrevious();
                else if (gestureListener.IsSwipeUp())
                    RotateUp();
                else if (gestureListener.IsSwipeDown())
                    RotateDown();
            }

            // check for automatic slide-change after a given delay time
            if (autoChangeAlfterDelay && Time.realtimeSinceStartup >= slideWaitUntil)
            {
                RotateToNext();
            }
        }
        else
        {
            // spin the presentation
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, spinSpeed * Time.deltaTime);

            // check if transform reaches the target rotation. If yes - stop spinning
            float deltaTargetX = Mathf.Abs(targetRotation.eulerAngles.x - transform.rotation.eulerAngles.x);
            float deltaTargetY = Mathf.Abs(targetRotation.eulerAngles.y - transform.rotation.eulerAngles.y);

            if (deltaTargetX < 1f && deltaTargetY < 1f)
            {
                // delay the slide
                slideWaitUntil = Time.realtimeSinceStartup + slideChangeAfterDelay;
                isSpinning = false;
            }
        }
    }

    private void RotateToNext()
    {
        menuY = 0;
        // set the next texture slide
        menuX = (menuX + 1) % texList.Count;

        if (!isBehindUser)
        {
            side = (side + 1) % maxSides;
        }
        else
        {
            if (side <= 0)
                side = maxSides - 1;
            else
                side -= 1;
        }

        if (horizontalSides[side] && horizontalSides[side].renderer)
        {
            horizontalSides[side].renderer.material.mainTexture = texList[menuX].tex[menuY];
        }

        // rotate the presentation
        float yawRotation = !isBehindUser ? 360f / maxSides : -360f / maxSides;
        Vector3 rotateDegrees = new Vector3(0f, yawRotation, 0f);
        targetRotation *= Quaternion.Euler(rotateDegrees);
        isSpinning = true;
    }

    private void RotateToPrevious()
    {
        menuY = 0;
        // set the previous texture slide
        if (menuX <= 0)
            menuX = texList.Count - 1;
        else
            menuX -= 1;

        if (!isBehindUser)
        {
            if (side <= 0)
                side = maxSides - 1;
            else
                side -= 1;
        }
        else
        {
            side = (side + 1) % maxSides;
        }

        if (horizontalSides[side] && horizontalSides[side].renderer)
        {
            horizontalSides[side].renderer.material.mainTexture = texList[menuX].tex[menuY];
        }

        // rotate the presentation
        float yawRotation = !isBehindUser ? -360f / maxSides : 360f / maxSides;
        Vector3 rotateDegrees = new Vector3(0f, yawRotation, 0f);
        targetRotation *= Quaternion.Euler(rotateDegrees);
        isSpinning = true;
    }
    private void RotateUp()
    {
        // set the above texture slide
        if (menuY <= 0)
            menuY = texList[menuX].tex.Count - 1;
        else
            menuY -= 1;

        if (!isBehindUser)
        {
            if (side <= 0)
                side = maxSides - 1;
            else
                side -= 1;
        }
        else
        {
            side = (side + 1) % maxSides;
        }

        if (horizontalSides[side] && horizontalSides[side].renderer)
        {
            horizontalSides[side].renderer.material.mainTexture = texList[menuX].tex[menuY];
        }

        // rotate the presentation
        float yawRotation = !isBehindUser ? -360f / maxSides : 360f / maxSides;
        Vector3 rotateDegrees = new Vector3(yawRotation, 0f, 0f);
        targetRotation *= Quaternion.Euler(rotateDegrees);
        isSpinning = true;
    }
    private void RotateDown()
    {
        // set the next texture slide
        menuY = (menuY + 1) % texList[menuX].tex.Count;

        if (!isBehindUser)
        {
            side = (side + 1) % maxSides;
        }
        else
        {
            if (side <= 0)
                side = maxSides - 1;
            else
                side -= 1;
        }

        if (horizontalSides[side] && horizontalSides[side].renderer)
        {
            horizontalSides[side].renderer.material.mainTexture = texList[menuX].tex[menuY];
        }

        // rotate the presentation
        float yawRotation = !isBehindUser ? 360f / maxSides : -360f / maxSides;
        Vector3 rotateDegrees = new Vector3(yawRotation, 0f, 0f);
        targetRotation *= Quaternion.Euler(rotateDegrees);
        isSpinning = true;
    }
}
