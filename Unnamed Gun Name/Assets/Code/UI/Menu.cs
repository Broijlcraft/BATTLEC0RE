using UnityEngine;

public class Menu : MonoBehaviour {

    public MenuState menuPosition;
    public bool canNotGoBackWithEsc, dontOpenPreviousMenuOnJustMenuClose;
    public Menu previousMenu;

    public virtual void ExtraFunctionalityOnCLose() {}
    public virtual void ExtraFunctionalityOnOpen() {}
}