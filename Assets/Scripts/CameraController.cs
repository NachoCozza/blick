using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{

    public Transform perspectiveTransform;
    public Transform topTransform;
    public Transform rightTransform;

    public float topProjectionSize;
    public float rightProjectionSize;

    public AnimationCurve movementCurve;
    public AnimationCurve fovCurve;
    public float movementTime = 0.4f;
    public float fovTime = 0.3f;

    float cameraShakeDuration = 0f;
    public float shakeAmount = 0.7f;
    public float decreaseFactor = 1.0f;

    private Vector3 originalPos;

    bool moving = false;
    Camera cam;
    Coroutine currentTransitionCoroutine;

    Matrix4x4 perspective;
    Matrix4x4 orthoTop;
    Matrix4x4 orthoRight;

    PerspectiveController perspectiveController;

    private void Awake()
    {
        //Time.timeScale = 0.5f;
        cam = GetComponent<Camera>();
        moving = false;
        float aspect = (float)Screen.width / (float)Screen.height;

        orthoTop = Matrix4x4.Ortho(-topProjectionSize * aspect, topProjectionSize * aspect, -topProjectionSize, topProjectionSize, 0.3f, 1000);
        orthoRight = Matrix4x4.Ortho(-rightProjectionSize * aspect, rightProjectionSize * aspect, -rightProjectionSize, rightProjectionSize, 0.3f, 1000);
        perspective = Matrix4x4.Perspective(60, aspect, 0.3f, 1000);

        perspectiveController = GameObject.FindGameObjectWithTag("GameController").GetComponent<PerspectiveController>();

        CalculatePlayerPosition();
    }

    private void Start() {
        originalPos = perspectiveTransform.position;
    }

    private void CalculatePlayerPosition()
    {
        GameObject auxGO = new GameObject();
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        auxGO.transform.parent = gameObject.transform;
        Camera aux = auxGO.AddComponent<Camera>();
        aux.enabled = false;

        aux.projectionMatrix = orthoRight;
        aux.transform.position = rightTransform.position;
        aux.transform.rotation = rightTransform.rotation;

        float newZ = aux.ViewportToWorldPoint(new Vector3(0.15f, 0, 0)).z;
        float maxZForPlayer = aux.ViewportToWorldPoint(new Vector3(0f, 0, 0)).z;
        Vector3 newPos = player.transform.position;
        newPos.z = newZ;
        player.transform.position = newPos;
        player.GetComponent<PlayerController>().SetMaxZ(maxZForPlayer);
        Destroy(auxGO);
    }

    // Use this for initialization
    public void SetCurrentView(View oldView, View newView)
    {
        Transform moveTo = null;
        Matrix4x4 projectTo;
        switch (newView)
        {
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
        if (moving)
        {
            StopCoroutine(currentTransitionCoroutine);
            moving = false;
        }
        currentTransitionCoroutine = StartCoroutine(MoveToPosition(moveTo, projectTo, oldView, newView));
    }


    // Update is called once per frame
    IEnumerator MoveToPosition(Transform endTransform, Matrix4x4 endProjection, View oldView, View newView)
    {
        Transform startTransform = transform;
        Matrix4x4 startProjection = cam.projectionMatrix;
        if (!moving)
        {
            moving = true;
            originalPos = endTransform.position;
            perspectiveController.NotifyCameraStart(oldView, newView);

            float i = 0f;
            float rate = 1 / movementTime;
            while (i < 1)
            {
                i += Time.deltaTime * rate;
                transform.localPosition = Vector3.Lerp(startTransform.position, endTransform.position, movementCurve.Evaluate(i));
                transform.localRotation = Quaternion.Lerp(startTransform.rotation, endTransform.rotation, movementCurve.Evaluate(i));
                cam.projectionMatrix = MatrixLerp(startProjection, endProjection, fovCurve.Evaluate(i));
                yield return 0;
            }

            perspectiveController.NotifyCameraFinish(oldView, newView);
            moving = false;
        }
        yield return 0;
    }

    Matrix4x4 MatrixLerp(Matrix4x4 from, Matrix4x4 to, float time)
    {
        Matrix4x4 ret = new Matrix4x4();
        for (int i = 0; i < 16; i++)
            ret[i] = Mathf.Lerp(from[i], to[i], time);
        return ret;
    }

    void Update() {
        if (cameraShakeDuration > 0) {
            transform.localPosition = originalPos + Random.insideUnitSphere * shakeAmount;
            cameraShakeDuration -= Time.deltaTime * decreaseFactor;
        }
    }

    public void DoShake(float duration) {
        cameraShakeDuration = duration;
    }

}
