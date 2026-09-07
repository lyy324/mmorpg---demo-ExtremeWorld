using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Entities;
using Services;
using SkillBridge.Message;
using Models;
using Managers;

public class GameObjectManager : MonoSingleton<GameObjectManager>
{

    Dictionary<int, GameObject> Characters = new Dictionary<int, GameObject>();
    // Use this for initialization
    protected override void OnStart()
    {
        EnsureUIWorldElementManager();
        StartCoroutine(InitGameObjects());
        CharacterManager.Instance.OnCharacterEnter += OnCharacterEnter;
        CharacterManager.Instance.OnCharacterLeave += OnCharacterLeave;
    }

    private void EnsureUIWorldElementManager()
    {
        UIWorldElementManager manager;
        if (UIWorldElementManager.TryGetInstance(out manager))
            return;

        GameObject prefab = Resloader.Load<GameObject>("UI/UIWorldElementManager");
        if (prefab == null)
        {
            Debug.LogError("UIWorldElementManager prefab was not found at Resources/UI/UIWorldElementManager.");
            return;
        }

        GameObject go = Instantiate(prefab);
        go.name = "UIWorldElementManager";
    }

    private void OnDestroy()
    {
        CharacterManager.Instance.OnCharacterEnter -= OnCharacterEnter;
        CharacterManager.Instance.OnCharacterLeave -= OnCharacterLeave;
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnCharacterEnter(Character cha)
    {
        CreateCharacterObject(cha);
    }

    void OnCharacterLeave(Character character)
    {
        if (!Characters.ContainsKey(character.entityId))
            return;

        if (Characters[character.entityId] != null)
        {
            Destroy(Characters[character.entityId]);
            this.Characters.Remove(character.entityId);
        }
    }

    IEnumerator InitGameObjects()
    {
        foreach (var cha in CharacterManager.Instance.Characters.Values)
        {
            CreateCharacterObject(cha);
            yield return null;
        }
    }

    //创建角色对象 存到Characters字典 
    private void CreateCharacterObject(Character character)
    {
        if (!Characters.ContainsKey(character.entityId) || Characters[character.entityId] == null)
        {
            Object obj = Resloader.Load<Object>(character.Define.Resource);
            if(obj == null)
            {
                Debug.LogErrorFormat("Character[{0}] Resource[{1}] not existed.",character.Define.TID, character.Define.Resource);
                return;
            }
            GameObject go = (GameObject)Instantiate(obj, this.transform);
            go.name = "Character_" + character.Id + "_" + character.Name;
            Characters[character.entityId] = go;

            UIWorldElementManager worldElementManager;
            if (UIWorldElementManager.TryGetInstance(out worldElementManager))
            {
                worldElementManager.AddCharacterNameBar(go.transform, character);
            }
            else
            {
                Debug.LogError("UIWorldElementManager is not initialized; character name bar was not created.");
            }
        }
        this.InitGameObject(Characters[character.entityId], character);
    }

    //初始化角色对象
    private void InitGameObject(GameObject go, Character character)
    {
        go.transform.position = GameObjectTool.LogicToWorld(character.position);
        go.transform.forward = GameObjectTool.LogicToWorld(character.direction);
        EntityController ec = go.GetComponent<EntityController>();
        if (ec != null)
        {
            ec.entity = character;
            ec.isPlayer = character.IsCurrentPlayer;
            ec.Ride(character.Info.Ride);
        }
        PlayerInputController pc = go.GetComponent<PlayerInputController>();
        // A character prefab without this component can still be rendered, but the
        // current player cannot be bound to input/camera. Recover at runtime so a
        // malformed class prefab does not leave the player apparently missing.
        if (pc == null)
        {
            pc = go.AddComponent<PlayerInputController>();
            pc.rb = go.GetComponent<Rigidbody>();
            Debug.LogWarningFormat("Character[{0}] prefab[{1}] had no PlayerInputController; added one at runtime.",
                character.Id, character.Define.Resource);
        }
        if (pc != null)
        {

            if (character.IsCurrentPlayer)
            {
                User.Instance.CurrentCharacterObject = pc;
                MainPlayerCamera.Instance.player = go;
                pc.enabled = true;
                pc.character = character;
                pc.entityController = ec;
            }
            else
            {
                pc.enabled = false;
            }
        }
    }


    public RideController LoadRide(int rideId, Transform parent)
    {
        var rideDefine = DataManager.Instance.Rides[rideId];
        Object obj = Resloader.Load<Object>(rideDefine.Resource);
        if (obj == null)
        {
            Debug.LogErrorFormat("Ride[{0}] Resource[{1}] not existed.", rideDefine.ID, rideDefine.Resource);
            return null;
        }
        GameObject go = (GameObject)Instantiate(obj, parent);
        go.name = "Ride_" + rideDefine.ID + "_" + rideDefine.Name;
        return go.GetComponent<RideController>();
    }
}

