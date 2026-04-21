using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class TutorialSystem : MonoBehaviour
{
    public Text instructionTextUI;
    public GameObject tutorialPanel; // napø. Panel s textem
    public float hideDelay = 2f;

    public List<TutorialStep> steps = new List<TutorialStep>();
    private int currentStepIndex = 0;
    private bool isHiding = false;

    void Start()
    {
        SetupSteps(); // napojení podmínek
        ShowCurrentStep();
    }

    void Update()
    {
        if (currentStepIndex >= steps.Count || isHiding) return;

        var step = steps[currentStepIndex];
        if (step.condition != null && step.condition.Invoke())
        {
            CompleteCurrentStep();
        }
    }

    public void CompleteCurrentStep()
    {
        if (currentStepIndex >= steps.Count) return;

        steps[currentStepIndex].onCompleted?.Invoke();
        currentStepIndex++;

        if (currentStepIndex < steps.Count)
            ShowCurrentStep();
        else
            StartCoroutine(HideTutorialAfterDelay());
    }
    public void CompleteStepByID(string id)
    {
        if (currentStepIndex >= steps.Count) return;

        var step = steps[currentStepIndex];
        if (step.stepID == id)
        {
            step.onCompleted?.Invoke();
            currentStepIndex++;

            if (currentStepIndex < steps.Count)
                ShowCurrentStep();
            else
                StartCoroutine(HideTutorialAfterDelay());
        }
    }

    void ShowCurrentStep()
    {
        tutorialPanel.SetActive(true);
        instructionTextUI.text = steps[currentStepIndex].instructionText;
    }

    IEnumerator HideTutorialAfterDelay()
    {
        isHiding = true;
        yield return new WaitForSeconds(hideDelay);
        tutorialPanel.SetActive(false);
        Debug.Log("Tutorial finished.");
    }

    // Napojení podmínek (mùžeš volat i zvenèí)
    void SetupSteps()
    {
        steps[0].condition = () => Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0;
        steps[1].condition = () => Input.GetKeyDown(KeyCode.Q) || Input.GetKeyDown(KeyCode.E);
        steps[2].condition = () => Input.GetKeyDown(KeyCode.I);
    }
}

[Serializable]
public class TutorialStep
{
    public string instructionText;
    public UnityEvent onCompleted;
    public string stepID;
    [HideInInspector] public Func<bool> condition;
}
