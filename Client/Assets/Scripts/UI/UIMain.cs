using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//using VSCodeEditor;
using Models;
//using UnityEditor.SceneManagement;
public class UIMain : MonoSingleton<UIMain>
{
    public Text AvatarName;
    public Text Avatarlevel;
    public UITeam TeamWindow;



    void UpdateAvatar()
	{
		AvatarName.text = string.Format("{0}[{1}]", User.Instance.CurrentCharacter.Name, User.Instance.CurrentCharacter.Id);
		Avatarlevel.text = User.Instance.CurrentCharacter.Level.ToString();
	}
	protected override void OnStart() {
		this.UpdateAvatar();
	}

	//返回选角面板 发送角色离开
	public void BackToCharSelect()
	{
		SceneManager.Instance.LoadScene("CharSelect");
		Services.UserService.Instance.SendGameLeave();
	}

	//测试面板
	public void OnClickTest()
	{
		UITest test = UIManager.Instance.Show<UITest>();
		test.title.text = "Test UI  111";
	}
	//背包面板
	public void OnClickBag()
	{
		UIManager.Instance.Show<UIBag>();
    }
	//角色装备面板
	public void OnClickEquip()
	{
		UIManager.Instance.Show<UICharEquip>();
	}
	//任务面板
	public void OnClickQuest()
	{
		UIManager.Instance.Show<UIQuestSystem>();
	}
	//角色面板
	public void OnClickFriend()
	{
		UIManager.Instance.Show<UIFriends>();
	}
	//组队信息
	public void ShowTeamUI(bool show)
	{
		TeamWindow.ShowTeam(show);
	}
	//工会面板
	public void OnClickGuild()
	{
		GuildManager.Instance.ShowGuild();
	}
	//坐骑面板
	public void OnClickRide()
	{

	}

	public void OnClickSetting()
	{
		UIManager.Instance.Show<UISetting>();
    }

	public void OnClickSkill()
	{

	}
}
