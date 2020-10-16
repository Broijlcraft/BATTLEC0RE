using UnityEngine.UI;
using UnityEngine;

public class MenuButton : MonoBehaviour {

    public Menu nextMenu;
    public GameObject[] additionalObjectsToDisableSeperateFromMenus;
    Menu menu;
    [HideInInspector] public Button button;

    private void Awake() {
        menu = GetComponentInParent<Menu>();
        button = GetComponent<Button>();
        button.onClick.AddListener(OpenSpecificMenu);
    }

    public void ResetButtonToDefaultOnclick() {
        button.onClick.AddListener(OpenSpecificMenu);
    }

    public void OpenSpecificMenu() {
        MenuManager.single_MM.OpenMenu(nextMenu);
        if (menu) {
            menu.gameObject.SetActive(false);
        }
        for (int i = 0; i < additionalObjectsToDisableSeperateFromMenus.Length; i++) {
            additionalObjectsToDisableSeperateFromMenus[i].SetActive(false);
        }
    }
}