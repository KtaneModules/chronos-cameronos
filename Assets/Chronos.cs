//Door Peep shall not enter dis yah Holy Land
//Where wise and true man stand Sipping from Cupful Cup of Peace
//Not one shall enter, not one
//Jah Rastafari

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using KModkit;
using Rnd = UnityEngine.Random;
using Math = ExMath;

public class Chronos : MonoBehaviour {

   public KMBombInfo Bomb;
   public KMAudio Audio;
   public KMSelectable[] Buttons;
   public TextMesh[] DisplayTexts;
   public Renderer watchBorder;
   public Material[] mats;

   static int ModuleIdCounter = 1;
   int ModuleId;
   private bool ModuleSolved;
   private const float _interactionPunchIntensity = .5f;

    string[] greekGods = {
    "Zeus",
    "Hera",
    "Poseidon",
    "Demeter",
    "Athena",
    "Apollo",
    "Artemis",
    "Ares",
    "Aphrodite",
    "Hephaestus",
    "Hermes",
    "Dionysus"
};

string[] greekGodsTraits = {
        "King",
        "Queen",
        "Sea",
        "Agriculture",
        "Wisdom",
        "Music",
        "Hunt",
        "War",
        "Love",
        "Fire",
        "Trade",
        "Wine"
    };
    
    
    string[] romanGods = {
    "Jupiter",
    "Juno",
    "Neptune",
    "Ceres",
    "Minerva",
    "Apollo",
    "Diana",
    "Mars",
    "Venus",
    "Vulcan",
    "Mercury",
    "Bacchus"
};



    //private ints for menus
    private int correctTrait = 0;
    private int correctRomanEquiv = 0;
    private int currentSelectedTrait = 0;
    private int currentSelectedRomanEquiv = 0;

   void Awake () {
      ModuleId = ModuleIdCounter++;
      GetComponent<KMBombModule>().OnActivate += Activate;
      /*
      foreach (KMSelectable object in keypad) {
          object.OnInteract += delegate () { keypadPress(object); return false; };
      }
      */

      //interaction punches
      Buttons[0].AddInteractionPunch(_interactionPunchIntensity);
      Buttons[1].AddInteractionPunch(_interactionPunchIntensity);
      Buttons[2].AddInteractionPunch(_interactionPunchIntensity);
      Buttons[3].AddInteractionPunch(_interactionPunchIntensity);
      Buttons[4].AddInteractionPunch(_interactionPunchIntensity);
      Buttons[5].AddInteractionPunch(_interactionPunchIntensity);

      Buttons[0].OnInteract += delegate () { buttonPress(1); return false; };
      Buttons[1].OnInteract += delegate () { buttonPress(2); return false; };
      Buttons[2].OnInteract += delegate () { buttonPress(3); return false; };
      Buttons[3].OnInteract += delegate () { buttonPress(4); return false; };
      
      Buttons[4].OnInteract += delegate () { submitModule(); return false; };
      Buttons[5].OnInteract += delegate () { startTimerForSubmit(); return false; };
   }

   void OnDestroy () { //Shit you need to do when the bomb ends
      
   }
   
   void startTimerForSubmit()
   {
       if(!ModuleSolved)
       {   
       Audio.PlaySoundAtTransform("ticking", Buttons[0].transform);
       StartCoroutine(SubmitTimer());
       }
   }
   
   private bool allowSubmission = false;
   
   void RedClock()
   {
    Material[] watchMats = watchBorder.materials;
    watchMats[0] = mats[2];
    watchBorder.materials = watchMats;
   }
   
   IEnumerator SubmitTimer()
    {
        RedClock();
        allowSubmission = true;
        // Base time off edgework
            if (Bomb.IsPortPresent(Port.Serial) || Bomb.GetSerialNumberLetters().Any(x => "JDGE".Contains(x)))
            {
                yield return new WaitForSeconds(2f);
            }
            else if (Bomb.IsPortPresent(Port.PS2) || Bomb.GetSerialNumberLetters().Any(x => "GODS".Contains(x)))
            {
                yield return new WaitForSeconds(3f);
            }
            else
            {
                yield return new WaitForSeconds(5f);
            }
        allowSubmission = false;
        DetermineWatchBorderWithoutLogging(godIndex);
        StartCoroutine(DetermineIfStrikeForTimeLoss());
    }
    
    IEnumerator DetermineIfStrikeForTimeLoss()
    {
        yield return new WaitForSeconds(0.5f);
        if(!ModuleSolved)
        {
            Strike();
        }
        else
        {
            //Pleased
        }
    }
    
   void buttonPress(int buttonIndex)
   {
       if(!ModuleSolved)
     {
        Audio.PlaySoundAtTransform("flip", Buttons[0].transform);
        switch (buttonIndex) 
        {
            case 1:
            currentSelectedTrait = (currentSelectedTrait - 1 + greekGodsTraits.Length) % greekGodsTraits.Length;
            DisplayTexts[1].text = greekGodsTraits[currentSelectedTrait];
            break;
            
            case 2:
            currentSelectedTrait = (currentSelectedTrait + 1) % greekGodsTraits.Length;
            DisplayTexts[1].text = greekGodsTraits[currentSelectedTrait];
            break;
            
            case 3:
            currentSelectedRomanEquiv = (currentSelectedRomanEquiv - 1 + romanGods.Length) % romanGods.Length;
            DisplayTexts[2].text = romanGods[currentSelectedRomanEquiv];
            break;
            
            case 4:
            currentSelectedRomanEquiv = (currentSelectedRomanEquiv + 1) % romanGods.Length;
            DisplayTexts[2].text = romanGods[currentSelectedRomanEquiv];
            break;
        }
     }
   }
   
   void submitModule(){
       if(!ModuleSolved)
       {
        if(currentSelectedTrait == correctTrait && currentSelectedRomanEquiv == correctRomanEquiv && allowSubmission)
        {
            Solve();
        }
        else
        {
            Strike();
        }
       }
   }
    
    private int godIndex = 0;
    private string equivalent;
    void SelectGod()
    {
        godIndex = UnityEngine.Random.Range(0, greekGods.Length);
        string randomGod = greekGods[godIndex];
        DisplayTexts[0].text = randomGod;
        correctRomanEquiv = godIndex;
        string trait = greekGodsTraits[correctTrait];
        equivalent = romanGods[godIndex];
    }
    
    void DetermineTraitAfterAll()
    {
        if(watchColor == 1)
        {
            correctTrait = Mathf.Max(godIndex - 3, 0);
        }
        else
        {
           correctTrait = Mathf.Min(godIndex + 5, 11);
        }
        string trait = greekGodsTraits[correctTrait];
    }
    
    void DetermineWatchTime()
    {
        int timeDefuserHas;
            if (Bomb.IsPortPresent(Port.Serial) || Bomb.GetSerialNumberLetters().Any(x => "JDGE".Contains(x)))
            {
                timeDefuserHas = 2;
            }
            else if (Bomb.IsPortPresent(Port.PS2) || Bomb.GetSerialNumberLetters().Any(x => "GODS".Contains(x)))
            {
                timeDefuserHas = 3;
            }
            else
            {
                timeDefuserHas = 5;
            }
        Debug.LogFormat("[Chronos " + "#" + ModuleId + "] Defuser will have " + timeDefuserHas + " seconds to submit after pressing the clock.", ModuleId);
    }
    
    private int watchColor = 0;
    //0 being silver
    //1 being gold    
    void DetermineWatchBorderWithoutLogging(int index)
    {
        if(godIndex > 6)
        {
            Material[] watchMats = watchBorder.materials;
            watchMats[0] = mats[1];
            watchBorder.materials = watchMats;
            watchColor = 1;
        }
        else
        {
            Material[] watchMats = watchBorder.materials;
            watchMats[0] = mats[0];
            watchBorder.materials = watchMats;
            watchColor = 0;
        }
    }
    
    void SetupMenus()
    {
        DisplayTexts[1].text = greekGodsTraits[currentSelectedTrait];
        DisplayTexts[2].text = romanGods[currentSelectedRomanEquiv];
    }
    
    void LogAll()
    {
        string traitLog = greekGodsTraits[correctTrait];
        Debug.LogFormat("[Chronos " + "#" + ModuleId + "] Greek God is: " + greekGods[godIndex] + ".", ModuleId);
        Debug.LogFormat("[Chronos " + "#" + ModuleId + "] Correct trait is: " + traitLog + ".", ModuleId);
        Debug.LogFormat("[Chronos " + "#" + ModuleId + "] Correct Roman equivalent is: " + equivalent + ".", ModuleId);
        if(watchColor == 1)
        {
            Debug.LogFormat("[Chronos " + "#" + ModuleId + "] Correct watch border is: Gold.", ModuleId);
        }
        else
        {
            Debug.LogFormat("[Chronos " + "#" + ModuleId + "] Correct watch border is: Silver.", ModuleId);
        }
    }
    
   void Activate () { //Shit that should happen when the bomb arrives (factory)/Lights turn on
    Audio.PlaySoundAtTransform("timewarp", Buttons[0].transform); //To be cool IG
    SelectGod();
    DetermineWatchBorderWithoutLogging(godIndex);
    DetermineTraitAfterAll();
    LogAll();
    SetupMenus();
    DetermineWatchTime();
   }

   void Start () { //Shit
      
   }

   void Update () { //Shit that happens at any point after initialization

   }

    void SolveDisplayTexts()
    {
       DisplayTexts[0].text = "THE";
       DisplayTexts[1].text = "GODS";
       DisplayTexts[2].text = "REJOICE!";
       DisplayTexts[0].color = Color.green;
       DisplayTexts[1].color = Color.green;
       DisplayTexts[2].color = Color.green;
    }
    
   void Solve () {
      ModuleSolved = true;
      Debug.LogFormat("[Chronos " + "#" + ModuleId + "] Solved! Defuser entered correct options: " + greekGods[godIndex] + ", " + greekGodsTraits[correctTrait] + ", " +romanGods[godIndex] + ".", ModuleId);
      GetComponent<KMBombModule>().HandlePass();
      Audio.PlaySoundAtTransform("solve", Buttons[0].transform);
      SolveDisplayTexts();
   }
   
      void SolveForceTP () {
      ModuleSolved = true;
      Debug.LogFormat("[Chronos " + "#" + ModuleId + "] Solved forcefully through Twitch Plays.", ModuleId);
      GetComponent<KMBombModule>().HandlePass();
      Audio.PlaySoundAtTransform("solve", Buttons[0].transform);
      SolveDisplayTexts();
   }

   void Strike () {
      Debug.LogFormat("[Chronos " + "#" + ModuleId + "] Strike! Defuser entered incorrect options: " + greekGods[godIndex] + ", " + greekGodsTraits[currentSelectedTrait] + ", " +romanGods[currentSelectedRomanEquiv] + ".", ModuleId);
      GetComponent<KMBombModule>().HandleStrike();
      Audio.PlaySoundAtTransform("strike", Buttons[0].transform);
   }

#pragma warning disable 414
   private readonly string TwitchHelpMessage = @"Use !{0} trait [left/right] to cycle through the trait menu. Use !{0} roman [left/right] to cycle through the Roman equivalents. Use !{0} clock to start the timer on the clock. Use !{0} submit to submit answers, while the clock is running.";
#pragma warning restore 414

   IEnumerator ProcessTwitchCommand (string Command) {
  Command = Command.ToUpper();
  yield return null;

  switch (Command) {

  case "TRAIT LEFT":
    Buttons[0].OnInteract();
    break;
    
   case "TRAIT RIGHT":
    Buttons[1].OnInteract();
    break;
    
   case "ROMAN LEFT":
    Buttons[2].OnInteract();
    break;
    
   case "ROMAN RIGHT":
    Buttons[3].OnInteract();
    break;
    
    case "SUBMIT":
    Buttons[4].OnInteract();
    break;
    
       case "CLOCK":
    Buttons[5].OnInteract();
    break;
   }
   }

   IEnumerator TwitchHandleForcedSolve () {
       SolveForceTP(); //lazy LOL
      yield return null;
   }
}