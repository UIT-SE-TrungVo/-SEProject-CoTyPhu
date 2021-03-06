using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIItemInPlayer : MonoBehaviour
{
    #region UI properties
    [SerializeField] Sprite baseItemImage;
    #endregion
    public BaseItem value;
    Button _clickField;

    public void Init(BaseItem initItem)
    {
        if (value == null)
        {
            value = initItem;
        }
        SetInfo();
    }

    public void SetInfo()
    {
        transform.GetComponent<Image>().sprite = value.gameObject.GetComponent<Image>().sprite;
    }

    public void SetNull()
    {
        value = null;
        transform.GetComponent<Image>().sprite = baseItemImage;
    }

    public void Detail()
    {
        Debug.Log("Show Info");
        if (value != null && value.CanActivate)
        {
            ItemManager.Ins.RequestUseItem(value.Owner.Id, value.Id);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        _clickField = transform.GetComponent<Button>();
        _clickField.onClick.AddListener(Detail);
        //_clickField.onClick.AddListener(() => SoundManager.Ins.Play(AudioClipEnum.Select));
    }

    // Update is called once per frame
    void Update()
    {
        if (value == null)
        {
            _clickField.enabled = false;
        }
        else
        {
            _clickField.enabled = true;
        }
    }
}
