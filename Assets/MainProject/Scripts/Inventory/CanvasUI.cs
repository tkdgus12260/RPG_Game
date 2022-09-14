using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CanvasUI : MonoBehaviour
{
    Inventory inven;

    public GameObject inventoryPanel;
    public GameObject pausePanel;
    public GameObject dragonHpBar;
    public GameObject alteregoHpBar;

    public Text playerName;

    public bool activePause = false;
    private bool activeInventory = false;
    private bool cursorHide = false;
    private bool activeCursor = false;

    public bool isPause = false;

    //private bool activeDragonHP = false;

    public Slot[] slots;
    public Transform slotHolder;

    private void Start()
    {
        inven = Inventory.instance;
        slots = slotHolder.GetComponentsInChildren<Slot>();
        inven.onSlotCountChange += SlotChange;
        inven.onChangeItem += pickUpSlotUI;
        inventoryPanel.SetActive(activeInventory);
        pausePanel.SetActive(activePause);
        playerName.text = DataManager.Inst.Player.name;
    }

    private void SlotChange(int var)
    {
        for(int i =0; i < slots.Length; i++)
        {
            slots[i].slotnum = i;

            if (i < inven.SlotCount)
            {
                slots[i].GetComponent<Button>().interactable = true;
            }
            else
            {
                slots[i].GetComponent<Button>().interactable = false;
            }
        }
    }

    public void pickUpSlotUI()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            slots[i].RemoveSlot();
        }
        for (int i = 0; i < inven.items.Count; i++)
        {
            slots[i].item = inven.items[i];
            slots[i].UpdateSlotUI();
        }
    }

    public void InventoryOnOff()
    {
        activeInventory = !activeInventory;
        inventoryPanel.SetActive(activeInventory);

        activeCursor = !activeCursor;

        if (activeCursor)
        {
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
        }

        cursorHide = !cursorHide;
        Cursor.visible = cursorHide;
    }

    // 일시정지 창 온 오프
    public void PauseOnOff()
    {
        isPause = !isPause;

        if (isPause)
            Time.timeScale = 0.0f;
        else
            Time.timeScale = 1.0f;

        activePause = !activePause;
        pausePanel.SetActive(activePause);

        activeCursor = !activeCursor;

        if (activeCursor)
        {
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
        }

        cursorHide = !cursorHide;
        Cursor.visible = cursorHide;
    }

    // 드래곤 hp바 켜기
    public void DragonHpBarOn()
    {
        dragonHpBar.SetActive(true);
    }

    // 드래곤 hp바 끄기
    public void DragonHpBarOff()
    {
        dragonHpBar.SetActive(false);
    }

    // 분신 드래곤 hp바 켜기
    public void AlterDragonHpBarOn()
    {
        alteregoHpBar.SetActive(true);
    }

    // 분신 드래곤 hp바 끄기
    public void AlterDragonHpBarOff()
    {
        alteregoHpBar.SetActive(false);
    }

    public void Save()
    {
        DataManager.Inst.SaveData();
    }

    public void Exit()
    {
        //Time.timeScale = 0.0f;
        SceneManager.LoadScene(0);
    }

    // 인벤토리 칸 추가
    public void AddSlot()
    {
        inven.SlotCount++;
    }
}
