using UnityEngine;

public class MenuManager : MonoBehaviour {

    public static MenuManager single_MM;
    public bool isMainMenu;

    [HideInInspector] public Menu currentMenu;
    /*[HideInInspector]*/ public MenuState currentMenuState;

    [Space, Header("EscapeMenu")]
    public GameObject menuHolder;
    public Menu firstMenu;

    private void Awake() {
        if (!MenuManager.single_MM) {
            MenuManager.single_MM = this;
        }
    }

    public void MainMenuInit() {
        isMainMenu = false;
    }

    public void InGameInit() {
        isMainMenu = false;
        currentMenuState = MenuState.Closed;
        CanvasComponents cc = CanvasComponents.single_CC;
        menuHolder = cc.menuHolder;
        firstMenu = cc.firstMenu;
        OptionsManager.single_OM.Init();
        for (int i = 0; i < cc.moveUpButtons.Length; i++) {
            cc.moveUpButtons[i].onClick.AddListener(MoveUpOrCloseMenu);
        }
    }

    private void Update() {
        if (Input.GetButtonDown("Cancel") && PlayerCheck()) {
            MoveUpOrCloseMenu();
        }
    }

    bool PlayerCheck() {
        bool menuIA = true;
        if (Controller.single_CLocal) {
            menuIA = Controller.single_CLocal.isActive;
        }
        return menuIA;
    }

    public void MoveUpOrCloseMenu() {
        if (currentMenuState == MenuState.Closed) {
            menuHolder.SetActive(true);
            OpenMenu(firstMenu);
        } else {
            if (!currentMenu.canNotGoBackWithEsc) {
                CloseMenu();
            }
        }
    }

    public void CloseMenu() {
        if (currentMenu) {
            currentMenu.ExtraFunctionalityOnCLose();
            currentMenu.gameObject.SetActive(false);
            if (currentMenu.menuPosition == MenuState.FirstPanel || currentMenu.dontOpenPreviousMenuOnJustMenuClose) {
                menuHolder.SetActive(false);
                currentMenu = null;
                currentMenuState = MenuState.Closed;
                Controller.single_CLocal.canMove = true;
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            } else {
                OpenMenu(currentMenu.previousMenu);
            }
        }
    }

    public void OpenMenu(Menu menu) {
        if (menu) {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            menu.ExtraFunctionalityOnOpen();
            currentMenu = menu;
            currentMenu.gameObject.SetActive(true);
            currentMenuState = menu.menuPosition;
            if (!isMainMenu) {
                Controller.single_CLocal.canMove = false;
                menuHolder.SetActive(true);
            }
        }
    }

    public void BackToMainMenu() {

    }

    public void QuitGame() {
        Application.Quit();
    }
}

public enum MenuState {
    Closed,
    FirstPanel,
    DeeperPanel
}