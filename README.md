# rgkunity22
 racing game kit

 Add two button in UI
 
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
        
