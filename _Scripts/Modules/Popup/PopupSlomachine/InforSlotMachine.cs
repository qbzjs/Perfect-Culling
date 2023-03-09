using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InforSlotMachine : MonoBehaviour
{
    [SerializeField] private Button btArrowRight, btArrowLeft;
    [SerializeField] private RectTransform imageRoll;
    [SerializeField] private Image imageDotRight, imageDotLeft;
    // Start is called before the first frame update
    private void OnEnable()
    {
        if (imageRoll != null&& btArrowLeft != null && btArrowRight != null && imageDotRight!=null && imageDotLeft != null)
        {
            imageRoll.anchoredPosition = new Vector2(512, 0);
            btArrowLeft.gameObject.SetActive(false);
            btArrowRight.gameObject.SetActive(true);
            imageDotLeft.gameObject.SetActive(true);
            imageDotRight.gameObject.SetActive(false);
        }
    }
    void Start()
    {
        if (btArrowLeft != null && btArrowRight != null)
        {
            btArrowLeft.onClick.AddListener(ClickBtLeft);
            btArrowRight.onClick.AddListener(ClickBtRight);
        }
    }

    // Update is called once per frame
    private void ClickBtRight()
    {
        imageRoll.LeanMoveLocal(new Vector2(-512, 0), 0.25f).setEaseLinear().setOnComplete(() =>
        {
            btArrowRight.gameObject.SetActive(false);
            btArrowLeft.gameObject.SetActive(true);
            imageDotLeft.gameObject.SetActive(false);
            imageDotRight.gameObject.SetActive(true);
        });

    }
    private void ClickBtLeft()
    {
        imageRoll.LeanMoveLocal(new Vector2(512, 0), 0.25f).setEaseLinear().setOnComplete(() =>
        {
            btArrowLeft.gameObject.SetActive(false);
            btArrowRight.gameObject.SetActive(true);
            imageDotRight.gameObject.SetActive(false);
            imageDotLeft.gameObject.SetActive(true);
        });

    }
}
