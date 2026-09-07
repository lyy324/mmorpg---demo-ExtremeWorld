using Services;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIGuildPopCreate : UIWindow
{
    public InputField inputName;
    public InputField inputNotice;
    // Start is called before the first frame update
    void Start()
    {
        GuildService.Instance.OnGuildCreateResult = OnGuildCreated;
    }
    private void OnDestroy()
    {
        GuildService.Instance.OnGuildCreateResult = null;
    }

    public override void OnYesClick()
    {
        if (string.IsNullOrEmpty(inputName.text))
        {
            MessageBox.Show("请输入工会名称", "错误", MessageBoxType.Error);
            return;
        }
        if (inputName.text.Length < 4 || inputName.text.Length > 10)
        {
            MessageBox.Show("工会名称应该为4-10个字符", "错误", MessageBoxType.Error);
            return;
        }
        if (string.IsNullOrEmpty(inputNotice.text))
        {
            MessageBox.Show("请输入工会宗旨", "错误", MessageBoxType.Error);
            return;
        }
        if (inputNotice.text.Length < 3 || inputNotice.text.Length > 50)
        {
            MessageBox.Show("工会宗旨需要3-50个字符", "错误", MessageBoxType.Error);
            return;
        }

        GuildService.Instance.SendGuildCreate(inputName.text,inputNotice.text);
    }

    // Update is called once per frame
    void OnGuildCreated(bool result)
    {
        if (result)
            this.Close(WindowResult.Yes);
    }
}
