# rgkunity22
 racing game kit
 
Bug:

 Scene of the game will be always set on the pause on the start of the game. 
 This bug exist only in editor. Need every time manualy Run the game by the "Play" button. 
 In the build this bug does  not exist.

Fixing second bug with manual transmition:

(When manual transmition setup, You can change transmition by the keys F,V. 
Its enable to increase or decrease speed by this keys )

 Add two button in UI:
 
![15](https://github.com/oneru230/rgkunity22/assets/171971032/9216983c-4017-4835-afb7-e306a96f4004)

second
![16](https://github.com/oneru230/rgkunity22/assets/171971032/b9f1f8f4-55e1-4793-93fb-89b60881f233)

Correct script RGKUI_PauseMenu.cs:

line 60:

//my correction

        public GameObject oRacemanager;
        
line 242: paste method

 //my correction
 
        public void startManager() {
        
        oRacemanager.SetActive(true);
        
        }

Correct script RGKUI:

line 62:

//my correction

        public Race_Manager m_RaceManager;
        
        public RGKUI_PauseMenu pausemenu;
        
line 152: paste new method

    //my correction
    
        public void StartManager() 
        {

            pausemenu.enabled = true;
            m_RaceManager.enabled = true;
        }

Next Steps:

1. Turn of script "RaceManager" located on the game object "_RaceManager"

2. Turn of game object "_RaceManager" in the scene

3. Turn of script "RGKUI_PauseMenu.cs" or PauseMenuUI (in Editor) located on the object "UI_INGAME".

4. Script "RGKUI_PauseMenu.cs":

   In the new variable gameObject "O RaceManager" set game object "_RaceManager"

5. Script "RGKUI.cs": 

   In the new variable "m_RaceManager" set script "Race_Manager.cs" (from the same game object)

   In the new variable "pausemenu" set script "RGKUI_PauseMenu.cs" from the game object "UI_INGAME"





