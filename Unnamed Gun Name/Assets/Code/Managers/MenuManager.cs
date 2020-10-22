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

    public void Init() {
        isMainMenu = false;
    }

    private void Start() {
        Debug.LogWarning("Implement controller canmove");
    }

    private void Update() {
        if (Input.GetButtonDown("Cancel")) {
            if(currentMenuState != MenuState.Closed) {
                MoveUpOrCloseMenu();
            } else {
                OpenMenu(firstMenu);
            }
        }
    }

    public void MoveUpOrCloseMenu() {
        if (currentMenuState == MenuState.Closed) {
            if (menuHolder && firstMenu) {
                menuHolder.SetActive(true);
                OpenMenu(firstMenu);
            }
        } else {
            if (currentMenu && !currentMenu.canNotGoBackWithEsc) {
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
            } else {
                OpenMenu(currentMenu.previousMenu);
            }
        }
    }

    public void OpenMenu(Menu menu) {
        if (menu) {
            menu.ExtraFunctionalityOnOpen();
            currentMenu = menu;
            currentMenu.gameObject.SetActive(true);
            currentMenuState = menu.menuPosition;
            if (!isMainMenu) {
                Controller.single_CLocal.canMove = false;
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