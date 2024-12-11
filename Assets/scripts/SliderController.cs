using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class SliderController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI sliderText = null;
    [SerializeField] private Slider _slider;

    private void Start()
    {
        _slider.onValueChanged.AddListener((v) =>
        {
            sliderText.text = v.ToString("0.0");
        });
    }
}
