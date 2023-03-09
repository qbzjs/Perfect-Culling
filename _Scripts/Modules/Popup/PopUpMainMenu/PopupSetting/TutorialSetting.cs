using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialSetting : MonoBehaviour
{
    [SerializeField] private List<GameObject> list_Step;
    [SerializeField] private List<Image> list_ImageNumber;
    [SerializeField] private Sprite image_White;
    [SerializeField] private Sprite image_Black;


    [SerializeField] private Button bt_Next;
    [SerializeField] private Button bt_Previous;

    private int current_Index = 0;

    private void OnEnable()
    {
        current_Index = 0;
        if (list_Step != null && list_Step.Count != 0 && list_ImageNumber != null && list_ImageNumber.Count != 0)
        {
            SetActiveStep(list_Step[0], true);
            SetActiveNumber(list_ImageNumber[0], image_White);
            for (int i = 1; i < list_Step.Count; i++)
            {
                SetActiveStep(list_Step[i], false);
                SetActiveNumber(list_ImageNumber[i], image_Black);
            }
        }
    }

    private void SetActiveNumber(Image current_image, Sprite sp_next)
    {
        if (current_image != null && sp_next != null)
        {
            current_image.sprite = sp_next;
        }
    }
    private void SetActiveStep(GameObject game_object, bool active)
    {
        if (game_object != null)
            game_object.SetActive(active);
    }
    // Start is called before the first frame update
    void Start()
    {
        if (bt_Next != null)
            bt_Next.onClick.AddListener(
            () =>
            {
                int pre_index = current_Index;
                current_Index++;
                SetStepActive(current_Index, pre_index);
            });
        if (bt_Previous != null)
            bt_Previous.onClick.AddListener(
            () =>
            {
                int pre_index = current_Index;
                current_Index--;
                SetStepActive(current_Index, pre_index);
            });
    }

    private void SetStepActive(int index, int pre_index)
    {
        if (list_Step != null && list_Step.Count != 0 && list_ImageNumber != null && list_ImageNumber.Count != 0)
        {
            if (index < 0)
            {
                index = list_Step.Count - 1;
                current_Index = index;
            }
            else if (index == list_Step.Count)
            {
                index = 0;
                current_Index = index;
            }
            SetActiveStep(list_Step[index], true);
            SetActiveNumber(list_ImageNumber[index], image_White);
            SetActiveStep(list_Step[pre_index], false);
            SetActiveNumber(list_ImageNumber[pre_index], image_Black);
        }
    }
}
