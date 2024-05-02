using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SliderClickDetection : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    bool isSliderClicked = false;

    /// <summary>
    /// 獲取Slider點擊狀態
    /// </summary>
    public bool GetSkiderClicked
    {
        get
        {
            return isSliderClicked;
        }
    }

    //Slider點擊
    public void OnPointerDown(PointerEventData eventData)
    {
        isSliderClicked = true;
    }

    //Slider被釋放
    public void OnPointerUp(PointerEventData eventData)
    {
        isSliderClicked = false;
    }
}