using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{

    Text text;
    Color notHoverColor;
    void Start() {
        text = transform.GetChild(0).gameObject.GetComponent<Text>();
        notHoverColor = text.color;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        text.color = Color.white;
    }

	public void OnPointerExit(PointerEventData eventData)
	{
        text.color = notHoverColor;
	}
}