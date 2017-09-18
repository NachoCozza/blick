using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    public Transform perspectiveTransform;
    public Transform topTransform;
    public Transform rightTransform;


    public float topProjectionSize;
    public float rightProjectionSize;

    public AnimationCurve movementCurve;
    public float movementTime = 1f;

    bool moving = false;
    Camera cam;
    Coroutine currentTransitionCoroutine;

    float transitionTime = 0f; //Todo delete
    Matrix4x4 perspective;
    Matrix4x4 orthoTop;
    Matrix4x4 orthoRight;

    private void Start() {
        cam = GetComponent<Camera>();

        float aspect = (float)Screen.width / (float)Screen.height;
        orthoTop = Matrix4x4.Ortho(-topProjectionSize * aspect, topProjectionSize * aspect, -topProjectionSize, topProjectionSize, 0.3f, 1000);
        orthoRight = Matrix4x4.Ortho(-rightProjectionSize * aspect, rightProjectionSize * aspect, -rightProjectionSize, rightProjectionSize, 0.3f, 1000);

        perspective = Matrix4x4.Perspective(60, aspect, 0.3f, 1000);

    }

    // Use this for initialization
    public void SetCurrentView(View view) {
        Transform moveTo = null;
        Matrix4x4 projectTo;
        switch (view) {
            case View.Top:
                moveTo = topTransform;
                projectTo = orthoTop;
                break;
            case View.Right:
                moveTo = rightTransform;
                projectTo = orthoRight;
                break;
            case View.Persp:
            default:
                moveTo = perspectiveTransform;
                projectTo = perspective;
                break;
        }
        if (moving) {
            StopCoroutine(currentTransitionCoroutine);
            moving = false;
        }
        currentTransitionCoroutine = StartCoroutine(MoveToPosition(moveTo, projectTo));
    }

    // Update is called once per frame
    IEnumerator MoveToPosition(Transform endTransform, Matrix4x4 endProjection) {
        Debug.Log("Start chance pos");
        Transform startTransform = transform;
        Matrix4x4 starttProjection = cam.projectionMatrix;
        WaitForEndOfFrame wait = new WaitForEndOfFrame();
        if (!moving) {
            moving = true;
            float i = 0.0f;
            float rate = 1 / movementTime;
            while (i < 1) {
                i += Time.deltaTime * rate;
                transitionTime += Time.deltaTime;
                transform.localPosition = Vector3.Lerp(startTransform.position, endTransform.position, movementCurve.Evaluate(i));
                transform.localRotation = Quaternion.Lerp(startTransform.rotation, endTransform.rotation, movementCurve.Evaluate(i));
                cam.projectionMatrix = MatrixLerp(starttProjection, endProjection, movementCurve.Evaluate(i));
               
                yield return wait;
            }
            transitionTime = 0f;
            moving = false;
        }
        yield return wait;
    }

    Matrix4x4 MatrixLerp(Matrix4x4 from, Matrix4x4 to, float time) {
        Matrix4x4 ret = new Matrix4x4();
        for (int i = 0; i < 16; i++)
            ret[i] = Mathf.Lerp(from[i], to[i], time);
        return ret;
    }


}
