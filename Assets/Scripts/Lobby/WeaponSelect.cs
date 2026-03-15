using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class WeaponSelect : MonoBehaviour
{
    private InputSystem_Actions inputActions;

    private InputAction attackInputAction;
    private InputAction moveInputAction;

    private int weaponTypeInt;

    private Image[] weaponLists;

    private void Init()
    {
        inputActions = new();

        attackInputAction = inputActions.Player.Attack;
        moveInputAction = inputActions.Player.Move;

        Subscribe(moveInputAction, OnLeft);
        Subscribe(attackInputAction, OnRight);

        inputActions.Enable();

        weaponLists = GetComponentsInChildren<Image>();
    }

    private void Subscribe(InputAction inputAction, Action<InputAction.CallbackContext> action)
    {
        inputAction.started += action;
    }

    private void Describe(InputAction inputAction, Action<InputAction.CallbackContext> action)
    {
        inputAction.started -= action;
    }

    private void OnLeft(InputAction.CallbackContext context)
    {
        weaponLists[weaponTypeInt].canvasRenderer.SetAlpha(0.4f);
        weaponTypeInt = (weaponTypeInt == 0 ? 2 : weaponTypeInt - 1);
        Managers.PlayerStatus.NowWeaponType = (WeaponType)weaponTypeInt;
        weaponLists[weaponTypeInt].canvasRenderer.SetAlpha(1f);
    }
    private void OnRight(InputAction.CallbackContext context)
    {
        weaponLists[weaponTypeInt].canvasRenderer.SetAlpha(0.4f);
        weaponTypeInt = (weaponTypeInt == 2 ? 0 : weaponTypeInt + 1);
        Managers.PlayerStatus.NowWeaponType = (WeaponType)weaponTypeInt;
        weaponLists[weaponTypeInt].canvasRenderer.SetAlpha(1f);
    }

    private void Awake()
    {
        Init();
    }

    private void Start()
    {
        foreach (Image weaponList in weaponLists) weaponList.canvasRenderer.SetAlpha(0.4f);
        weaponTypeInt = (int)Managers.PlayerStatus.NowWeaponType;
        weaponLists[weaponTypeInt].canvasRenderer.SetAlpha(1f);
    }

    private void OnDestroy()
    {
        inputActions.Disable();

        Describe(moveInputAction, OnLeft);
        Describe(attackInputAction, OnRight);
    }
}
