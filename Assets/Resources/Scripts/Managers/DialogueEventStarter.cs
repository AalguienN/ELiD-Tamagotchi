using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PixelCrushers.DialogueSystem;
using UnityEngine.UI;

public class DialogueEventStarter : MonoBehaviour
{
    public static DialogueEventStarter instance;
    private void Awake()
    {
        instance = this;
    }

    public string[] conversations = new string[7];
    public Button continueBtn;
    public GameObject caputxa;
    public GameObject pinochio;
    public GameObject[] caputxas;
    public GameObject burntCaputxa;
    public Material woodTexture;
    Animator caputxaAnim, pinochioAnim;

    public bool forceLaugh = false;

    //Para el sonido
    public GameObject CaputxaSound;
    public GameObject PinochoSound;
    

    private void Start()
    {
        pinochioAnim = pinochio.GetComponentInChildren<Animator>();
        caputxaAnim = caputxa.GetComponentInChildren<Animator>();
        disableCaputxas();
        burntCaputxa.SetActive(false);
        if (SaveManager.getCurrentDay() == 0)
        {
            startCurrentDayDialogue();
            enableCaputxa();
        }
        if (SaveManager.getCurrentDay() == 1)
        {
            disableCaputxaInteraction();
        }
        if (SaveManager.getCurrentDay() != 4 || SaveManager.getCurrentDay() == 4 && SaveManager.hasBeenDialoguePlayed)
        {
            disablePinochio();
        }
        if (SaveManager.getCurrentDay() == 5 && !SaveManager.hasBeenDialoguePlayed || SaveManager.getCurrentDay() == 6 && !SaveManager.hasBeenDialoguePlayed)
        {
            enableCaputxas();
        }
        else if (SaveManager.getCurrentDay() == 7)
        {
            disableCaputxa();
            disableCaputxas();
            burntCaputxa.SetActive(true);
            //Change forest texture ??
            woodTexture.SetColor("_Color", Color.black);
            //
            cameraAnimationHandler.instance.ChangeAnimation(cameraAnimationHandler.FINALE_ANIM);
        }
    }
    private void Update()
    {
        if (Input.touchCount == 0) { return; }
        Touch fingertouch = Input.GetTouch(0);
        if (fingertouch.phase == TouchPhase.Began)
        {
            Vector3 mousePos = fingertouch.position;
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(mousePos);
            if (Physics.Raycast(ray, out hit, 100.0f))
            {
                if (hit.collider.gameObject.CompareTag("Caputxa") && SaveManager.canCaputxaBeInteracted)
                {
                    startCurrentDayDialogue();
                    SaveManager.hasBeenCaputxaInteracted = true;
                }
                else if (hit.collider.gameObject.CompareTag("Pinochio") && SaveManager.getCurrentDay() == 4)
                {
                    //corre pinochio corre
                    runPinochio();
                }
            }
        }
    }

   

    public void startCurrentDayDialogue()
    {
        print(SaveManager.getCurrentDay());
        startConversation(SaveManager.getCurrentDay());
    }

    public void endOfDialogue()
    {
        SaveManager.hasBeenDialoguePlayed = true;
    }
    
    public void disableCaputxaInteraction()
    {
        SaveManager.canCaputxaBeInteracted = false;
    }

    public void enableCaputxaInteraction()
    {
        SaveManager.canCaputxaBeInteracted = true;
    }

    public void enableCaputxa()
    {
        caputxa.SetActive(true);
    }

    public void disableCaputxa()
    {
        caputxa.SetActive(false);
    }

    public void enableCaputxas()
    {
        foreach(GameObject cap in caputxas)
        {
            cap.SetActive(true);
        }
    }

    public void disableCaputxas()
    {
        foreach (GameObject cap in caputxas)
        {
            cap.SetActive(false);
        }
    }

    public bool isCaputxaEnabled()
    {
        return caputxa.activeSelf;
    }

    public void enablePinochio() {
        print("enablePinochio");
        pinochio.SetActive(true);
    }

    public void disablePinochio()
    {
        pinochio.SetActive(false);
    }

    public void lockCamera(float seconds)
    {
        StartCoroutine(AnimationManager.cameraLockAnimation(seconds));
    }

    public void killCaputxa()
    {
        caputxa.GetComponentInChildren<Animator>().Play("CaputxaKillAnimation");
    }

    public void runPinochio()
    {
        //Play sound
        var emitterPinocho = PinochoSound.GetComponent<FMODUnity.StudioEventEmitter>();

        pinochioAnim.Play("Running_Pin");
        emitterPinocho.Play();
        StartCoroutine(pinochioFinished());
        StartCoroutine(pinochioMovement());
    }
    public IEnumerator pinochioFinished()
    {
        yield return new WaitForSeconds(1.5f);
        SaveManager.hasPinochioRun = true;
        //SaveManager.saveAll();
        disablePinochio();
    }
    public IEnumerator pinochioMovement()
    {
        float x = 0;
        float seconds = 1.5f;
        while(x < seconds)
        {
            pinochio.transform.position += new Vector3(0, 0, -10 * Time.deltaTime);
            x += Time.deltaTime / seconds;
            yield return null;
        }
    }

    public void lockCameraToCenter()
    {
        SaveManager.setCanOnlyTurn(0); //Just center
    }
    public void lockCameraToLeft()
    {
        print("lockCameraToLeft");
        SaveManager.setCanOnlyTurn(-1); //Just left
        StartCoroutine(lockCameraToLeft_());
    }
    IEnumerator lockCameraToLeft_()
    {
        yield return new WaitForSeconds(5);
        print("Ienumerator");
        if (!SaveManager.hasTurnedLeft)
        {
            cameraAnimationHandler.instance.ChangeAnimation(cameraAnimationHandler.LEFT_ARROW);
            lockCameraToLeft();
        }
    }

    public void lockCameraToRight()
    {
        print("lockCameraToRight");
        SaveManager.setCanOnlyTurn(1); //Just right
        StartCoroutine(lockCameraToRight_());
    }
    IEnumerator lockCameraToRight_()
    {
        yield return new WaitForSeconds(5);
        if (!SaveManager.hasTurnedRight)
        {
            cameraAnimationHandler.instance.ChangeAnimation(cameraAnimationHandler.RIGHT_ARROW);
            lockCameraToRight();
        }
    }

    public void unlockCamera()
    {
        SaveManager.setCanOnlyTurn(2); //Unlocks
    }


    public void disableContinue() {
        continueBtn.interactable = false;
    }

    public void enableContinue()
    {
        continueBtn.interactable = true;
    }

    public void startConversation(int day)
    {
        DialogueManager.StartConversation(conversations[day]);
    }

    IEnumerator waitForFirstBurn() {
        while (!SaveManager.hasBurntFirstStick)
        {
            yield return null;
        }
        (DialogueManager.dialogueUI as StandardDialogueUI).OnContinue();
    }

    IEnumerator waitForFirstBlueBurn()
    {
        while (!SaveManager.hasBurntFirstBlueStick)
        {
            yield return null;
        }
        (DialogueManager.dialogueUI as StandardDialogueUI).OnContinue();

    }

    public void firstBurn()
    {
        SaveManager.setStickNum(SaveManager.getStickNum() + 1);
        StartCoroutine(waitForFirstBurn());
    }
    
    public void firstBlueBurn()
    {
        SaveManager.setBlueStickNum(SaveManager.getBlueStickNum() + 1);
        SaveManager.setStickNum(SaveManager.getStickNum() + 1);
        print("Blue: " + SaveManager.getBlueStickNum() + " Normal: " + SaveManager.getStickNum());
        StartCoroutine(waitForFirstBlueBurn());
    }

    public void lastBlueBurn()
    {
        SaveManager.setBlueStickNum(SaveManager.getBlueStickNum() + 999);
        SaveManager.setStickNum(SaveManager.getStickNum() + 999);
        StartCoroutine(waitForLastBlueBurn());
    }

    IEnumerator waitForLastBlueBurn()
    {
        while (!SaveManager.hasBurntLastBlueStick)
        {
            yield return null;
        }
        (DialogueManager.dialogueUI as StandardDialogueUI).OnContinue();
    }
    
    public void zoomCameraSequence() {
        StartCoroutine(zoomCameraSequenceCoroutine());
    }
    public IEnumerator zoomCameraSequenceCoroutine() {
        MonthlyCalendarManager.Instance.BlockManualZoom(true);
        MonthlyCalendarManager.Instance.ZoomIn();
        float t = 0;
        while (t <= 5f)
        {
            t += Time.deltaTime;
            yield return null;
        }
        MonthlyCalendarManager.Instance.ZoomOut();
        (DialogueManager.dialogueUI as StandardDialogueUI).OnContinue();
    }

    public void unlockCameraZoom()
    {
        MonthlyCalendarManager.Instance.BlockManualZoom(false);
    }



    //Creo que este metodo ya no es necesario, no lo quito pq realmente no hace nada malo
    public void playCaputxaLaugh()
    {
        SoundManager.instance.PlaySound("voiceCapuchaLaugh");
    }

    public void ForceLaugh()
    {
        forceLaugh = true;
    }
 
    public void playCaputxaSound()
    {
        
        var emitterCaputxa = CaputxaSound.GetComponent<FMODUnity.StudioEventEmitter>();
        emitterCaputxa.SetParameter("Laugh",0);

        if (forceLaugh) { 
            emitterCaputxa.SetParameter("Laugh",1);
            forceLaugh = false;
        }
        emitterCaputxa.Play();     
    }

    public void killPlayerCheat()
    {
        cameraAnimationHandler.instance.ChangeAnimation(cameraAnimationHandler.FINALE_ANIM);
        killCaputxa();
        StartCoroutine(killCheater());
    }

    public IEnumerator killCheater()
    {
        yield return new WaitForSeconds(5f);
        SaveManager.hasBeenDialoguePlayed = false;
        SaveManager.hasBeenCaputxaInteracted = true;
        Application.Quit();
    }
}
