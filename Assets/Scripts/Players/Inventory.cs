using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{

    #region Attributes

    public static Inventory instance;
    public static int numSlots = 8;

    public struct Item
    {
        public Sprite sprite;
        public string info;
        public string name;
        public int id;

        public Item(PickUpItem pickUpItem)
        {
            sprite = pickUpItem.GetComponent<SpriteRenderer>().sprite;
            name = instance.CleanItemName(pickUpItem.name);
            info = pickUpItem.info;
            id = pickUpItem.id;
        }

    }

    private GameObject selectedItemPanel;
    private GameObject selectedItemSlot;
    private Text selectedItemInfo;
    private Item? selectedItem;
    private Item?[] items;

    #endregion

    #region Start

    public void Start()
    {
        instance = this;

        selectedItemInfo = GameObject.Find("DisplayItemInfo").GetComponent<Text>(); // TODO: Rename this in editor
        selectedItemPanel = GameObject.Find("DisplayPanel"); // TODO: Rename this in editor
        selectedItemSlot = GameObject.Find("BackgroundActualItem"); // TODO: Rename this in editor

        if (items == null)
        {
            items = new Item?[numSlots];
        }

        ToggleSelectedItemPanel(false);
    }

    #endregion

    #region Common

    public void UseItem(PlayerController player)
    {
        if (selectedItem == null)
        {
            LevelManager lManager = FindObjectOfType<LevelManager>();
            lManager.ActivateNPCFeedback("No hay un item en esta casilla");
            
            return;
        }

        ActivableSystem system = GetActivableSystem(player);

        if (system)
        {
            if (system.PlaceItem(selectedItem.Value.sprite))
            {
                RemoveItem();
            }
        }
    }

    public void AddItem(PickUpItem item)
    {
        int freeSlot = GetFreeSlot();

        if (freeSlot != -1)
        {
            items[freeSlot] = new Item(item);

            Image slotSprite = GameObject.Find("SlotSprite" + freeSlot).GetComponent<Image>();
            slotSprite.sprite = item.GetComponent<SpriteRenderer>().sprite;
            slotSprite.enabled = true;

            SendMessageToServer("InventoryUpdate/Add/" + freeSlot + "/" + items[freeSlot].Value.name, true);
        }

    }

    public void RemoveItem()
    {

        bool found = false;
        int index;

        for (index = 0; index < items.Length; index++)
        {
            if (items[index].Equals(selectedItem))
            {
                found = true;
                break;
            }
        }

        if (found)
        {
            items[index] = null;

            Image slotSprite = GameObject.Find("SlotSprite" + index).GetComponent<Image>();
            slotSprite.enabled = false;
            slotSprite.sprite = null;

            UnselectItem();
            SendMessageToServer("InventoryUpdate/Remove/" + index, true);
        }

    }

    public void SelectItem(int slot)
    {

        if (slot >= items.Length)
        {
            Debug.LogError("Invalid slot number");
            return;
        }

        Item? item = items[slot];

        if (item == null)
        {
            return;
        }

        selectedItem = item;

        selectedItemInfo.text = "";
        selectedItemInfo.text = "<color=#e67f84ff><b>" + "Usando '" + selectedItem.Value.name + "': </b></color>" + "\r\n";
        selectedItemInfo.text += "<color=#f9ca45ff>" + selectedItem.Value.info + "</color>";

        selectedItemSlot.GetComponent<Image>().sprite = selectedItem.Value.sprite;

        ToggleSelectedItemPanel(true);
        LevelManager lManager = FindObjectOfType<LevelManager>();
        PlayerController pController = lManager.GetLocalPlayerController();
        UseItem(pController);
    }

    public void UnselectItem()
    {
        selectedItem = null;
        selectedItemInfo.text = "";
        selectedItemSlot.GetComponent<Image>().sprite = null;

        ToggleSelectedItemPanel(false);
    }

    public void DropItem()
    {
        if (selectedItem == null)
        {
            Debug.LogError("No item to drop");
            return;
        }

        SendMessageToServer("CreateGameObject/" + selectedItem.Value.name, true);
        RemoveItem();
        ToggleSelectedItemPanel(false);
    }

    #endregion

    #region Utils

    protected ActivableSystem GetActivableSystem(PlayerController player)
    {
        ActivableSystem[] systems = FindObjectsOfType<ActivableSystem>();

        for (int i = 0; i < systems.Length; i++)
        {
            if (!systems[i].activated)
            {
                if (Vector2.Distance(systems[i].transform.position, player.transform.position) <= systems[i].activationDistance)
                {
                    return systems[i];
                }
            }
        }

        return null;
    }

    protected int GetFreeSlot()
    {
        for (int i = 0; i < items.Length; i++)
        {
            if (items[i] == null)
            {
                return i;
            }
        }

        return -1;
    }

    protected void ToggleSelectedItemPanel(bool active)
    {
        selectedItemSlot.SetActive(active);
        selectedItemPanel.SetActive(active);
    }

    public string CleanItemName(string name)
    {
        if (name.Contains("(Clone)"))
        {
            name = name.Replace("(Clone)", "");
        }

        return name;
    }

    #endregion

    #region Messaging

    private void SendMessageToServer(string message, bool secure)
    {
        if (Client.instance)
        {
            Client.instance.SendMessageToServer(message, secure);
        }
    }

    #endregion

}
