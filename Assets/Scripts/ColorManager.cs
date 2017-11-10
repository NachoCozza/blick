using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorManager : MonoBehaviour {

    public ColorGroup[] colors;

    int colorIdx = 0;

    public Material buildingMaterial;
    public Material firstLaneMaterial;
    public Material secondLaneMaterial;
    public Material obstacleMaterial;
    public Material signMaterial;


    // Use this for initialization
    void Start() {
        colorIdx = 0;
        ResetColor();
    }

    public void OneColorUp() {
        Debug.Log("one color p");
        bool apply = false;
        if (colorIdx + 1 < colors.Length) {
            colorIdx++;
            apply = true;
        }
        else {
            //ToDo image effect
        }
        if (apply) {
            StartCoroutine("ApplyColor", colors[colorIdx]);
        }
    }

    public void ResetColor() {
        InstantApplyColor(colors[0]);
    }

    IEnumerator ApplyColor(ColorGroup colors) {
        buildingMaterial.color = colors.buildingColor;
        firstLaneMaterial.color = colors.firstLaneColor;
        secondLaneMaterial.color = colors.secondLaneColor;
        obstacleMaterial.color = colors.obstacleColor;
        signMaterial.color = colors.firstLaneColor;
        yield return 0;
    }

    void InstantApplyColor(ColorGroup colors) {
        buildingMaterial.color = colors.buildingColor;
        firstLaneMaterial.color = colors.firstLaneColor;
        secondLaneMaterial.color = colors.secondLaneColor;
        obstacleMaterial.color = colors.obstacleColor;
        signMaterial.color = colors.firstLaneColor;
    }

    [System.Serializable]
    public class ColorGroup {
        public Color buildingColor; //Building
        public Color firstLaneColor; //MainMat
        public Color obstacleColor; //MainMatInverted
        public Color secondLaneColor; //MainMatInverted2
    }
}
