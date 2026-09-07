using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UIInputBox : MonoBehaviour
{
    public Text title;
    public Text message;
    public Text tips;
    public Button btnYes;
    public Button btnNo;
    public Text btnYesTitle;
    public Text btnNoTitle;
    public InputField inputField;

    public delegate bool SubmitHandler(string inputText, out string tips);
    public event SubmitHandler onSubmit;
    public UnityAction OnCancel;

    public string emptyTips;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Init(string title,string message,string btnOK = "",string btnCancel = "",string emptyTips = "")
    {
        if(!string.IsNullOrEmpty(title)) this.title.text = title;
        this.message.text = message;
        this.tips.text = null;
        this.onSubmit = null;
        this.emptyTips = emptyTips;

        if (!string.IsNullOrEmpty(btnOK)) this.btnYesTitle.text = title;
        if (!string.IsNullOrEmpty(btnCancel)) this.btnNoTitle.text = title;

        this.btnYes.onClick.AddListener(OnClickYes);
        this.btnNo.onClick.AddListener(OnClickNo);

    }
    private void OnClickYes()
    {
        this.tips.text = "";
        if(string.IsNullOrEmpty(inputField.text))
        {
            this.tips.text = this.emptyTips;
            return;
        }
        if(onSubmit != null)
        {
            string tips;
            if(!onSubmit(this.inputField.text,out tips))
            {
                this.tips.text = tips;
                return;
            }
        }
        Destroy(this.gameObject);
    }
    private void OnClickNo()
    {
        Destroy(this.gameObject);
        if(this.OnCancel != null)
            this.OnCancel();
    }


}
