using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Suburb {

public class SimpleOpenClose : MonoBehaviour
{
    private Animator myAnimator;
    private Animator additionalAnimator;
    public bool objectOpen;
    public bool objectOpenAdditional;
    public GameObject animateAdditional;
    private bool hasAdditional = false;
    float myNormalizedTime;


    // Open or close animator state in start depending on selection.
    // Additional object with animator. For example another door when double doors. 
    void Start()
    {
       
        // If there is no animator in the gameobject itself, get the parent animator.
        myAnimator = GetComponent<Animator>();
        if (myAnimator == null)
        {
            myAnimator = GetComponentInParent<Animator>();
        }
        

        if (objectOpen == true)
        {
            myAnimator.Play("Open", 0, 1.0f);
        }
        if (animateAdditional != null)
            if (animateAdditional.GetComponent<SimpleOpenClose>())
            {
                additionalAnimator = animateAdditional.GetComponent<Animator>();
                hasAdditional = true;
                objectOpenAdditional = animateAdditional.GetComponent<SimpleOpenClose>().objectOpen;
            }
        else
            {
                hasAdditional = false;
            }
    }

    // Player clicks object. Method called from SimplePlayerUse script.

    void ObjectClicked()
    {

        myNormalizedTime = myAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime;

        if (hasAdditional == false)
        {
            if (myNormalizedTime >= 1.0)
            {
                if (objectOpen == true)
                {
                    myAnimator.Play("Close", 0, 0.0f);
                    objectOpen = false;
                }

                else
                {
                    myAnimator.Play("Open", 0, 0.0f);
                    objectOpen = true;
                }
            }
        }

        if (hasAdditional == true && myNormalizedTime >= 1.0)
        {
            if (objectOpen == true)
            {
                myAnimator.Play("Close", 0, 0.0f);
                objectOpen = false;
                animateAdditional.GetComponent<SimpleOpenClose>().objectOpenAdditional = false;

                if (objectOpenAdditional == true)
                {
                    additionalAnimator.Play("Close", 0, 0.0f);
                    objectOpenAdditional = false;
                    animateAdditional.GetComponent<SimpleOpenClose>().objectOpen = false;
                }

            }

            else
            {
                myAnimator.Play("Open", 0, 0.0f);
                objectOpen = true;
                animateAdditional.GetComponent<SimpleOpenClose>().objectOpenAdditional = true;

                if (objectOpenAdditional == false)
                {
                    additionalAnimator.Play("Open", 0, 0.0f);
                    objectOpenAdditional = true;
                    animateAdditional.GetComponent<SimpleOpenClose>().objectOpen = true;

                }

            }

        }

    }

}
}
