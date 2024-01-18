using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using Unity.MLAgents.Sensors;
using UnityEngine.InputSystem;

[Serializable]
public class ProductionRuleCondition
{
    public CONDITION condition;
    public ProductionRuleIdentifier subjectIdentifier;
    public ProductionRuleIdentifier objectIdentifier;

    public ProductionRuleCondition(CONDITION condition, ProductionRuleIdentifier subjectIdentifier, ProductionRuleIdentifier objectIdentifier = null)
    {
        this.condition = condition;
        this.subjectIdentifier = subjectIdentifier;
        this.objectIdentifier = objectIdentifier;
    }

    public bool IsSatisfied(ProductionRuleObject subject, ProductionRuleObject obj, EnvironmentEngine env)
    {

        if (!CheckCondition(condition, subject, obj))
        {
            return false;
        }
        return true;
    }

    private bool CheckCondition(CONDITION condition, ProductionRuleObject subject, ProductionRuleObject obj)
    {
        // Access the state of the environment and check if the condition is satisfied
        // For example, if the condition is "near", check if the subject and object are near each other by checking their positions
        if (subject == null)
        {
            return false;
        }
        if (!subjectIdentifier.CompareTo(subject.identifier) || !objectIdentifier.CompareTo(obj.identifier))
        {
            return false;
        }
        switch (condition)
        {
            case CONDITION.NEAR:
                return CheckNear(subject, obj);

            // case CONDITION.USE:
            //     return CheckUse(subject, obj);

            // case CONDITION.DROP:
            //     return CheckDrop(subject, obj);

            // case CONDITION.PICKUP:
            //     return CheckPickup(subject, obj);

            case CONDITION.HOLD:
                return CheckHold(subject);

            case CONDITION.SEE:
                return CheckSee(subject);

            default:
                return false;
        }
    }

    private bool CheckNear(ProductionRuleObject subject, ProductionRuleObject obj)
    {
        // Logic to check if the subject and object are near each other
        // Example: return Vector3.Distance(subject.transform.position, obj.transform.position) < 1.0f;

        Vector3 subjectPosition = subject.transform.position;
        Vector3 objPosition = obj.transform.position;
        float distance = Vector3.Distance(subjectPosition, objPosition);
        return distance < subject.GetProductionRuleManager().GetNearDistance();

    }

    private bool CheckHold(ProductionRuleObject subject)
    {
        if (subject == null)
            return false;

        GravityGun gravityGun = subject.GetProductionRuleManager().GetGravityGun();
        ProductionRuleObject heldObject = null;
        if (gravityGun.GetHeldObject() != null)
        {
            heldObject = gravityGun.GetHeldObject().GetComponentInParent<ProductionRuleObject>();
        }
        return heldObject != null && heldObject == subject;
    }

    public bool CheckSee(ProductionRuleObject subject)
    {

        U3DPlayer player = (U3DPlayer)subject.GetEngine().GetEnvironmentComponent<U3DPlayer>();
        Camera camera = player.Camera;
        Vector3 viewPos = camera.WorldToViewportPoint(subject.transform.position);
        if (viewPos.x > 0 && viewPos.x < 1 && viewPos.y > 0 && viewPos.y < 1 && viewPos.z > 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public string Encode()
    {
        string conditionString = "";
        if (conditionString.Length > 0)
        {
            conditionString += " and ";
        }
        if (condition == CONDITION.NEAR)
        {
            conditionString += $"{subjectIdentifier.Encode()} is near {objectIdentifier.Encode()}";
        }
        else if (condition == CONDITION.CONTACT)
        {
            conditionString += $"{subjectIdentifier.Encode()} contacts {objectIdentifier.Encode()}";
        }
        else if (condition == CONDITION.USE)
        {
            conditionString += $"{subjectIdentifier.Encode()} is used";
        }
        else if (condition == CONDITION.DROP)
        {
            conditionString += $"{subjectIdentifier.Encode()} is dropped";
        }
        else if (condition == CONDITION.PICKUP)
        {
            conditionString += $"{subjectIdentifier.Encode()} is picked up";
        }
        else if (condition == CONDITION.HOLD)
        {
            conditionString += $"{subjectIdentifier.Encode()} is held";
        }
        else if (condition == CONDITION.USE)
        {
            conditionString += $"{subjectIdentifier.Encode()} is seen";
        }
        return conditionString;
    }

    //     try
    //     {
    //         string conditionString = "";
    //         foreach (var condition in conditions)
    //         {
    //             if (conditionString.Length > 0)
    //             {
    //                 conditionString += " and ";
    //             }
    //             conditionString += $"{condition.Encode()}";
    //         }

    //         return conditionString;
    //     }
    //     catch (System.NullReferenceException)
    //     {
    //         if (conditions.Contains(CONDITION.NEAR))
    //         {
    //             return $"{subject.Encode()} is near {obj.Encode()}";
    //         }
    //         else if (conditions.Contains(CONDITION.CONTACT))
    //         {
    //             return $"{subject.Encode()} contacts {obj.Encode()}";
    //         }
    //         else if (conditions.Contains(CONDITION.USE))
    //         {
    //             return $"{subject.Encode()} is used";
    //         }
    //         else if (conditions.Contains(CONDITION.DROP))
    //         {
    //             return $"{subject.Encode()} is dropped";
    //         }
    //         else if (conditions.Contains(CONDITION.PICKUP))
    //         {
    //             return $"{subject.Encode()} is picked up";
    //         }
    //         else if (conditions.Contains(CONDITION.HOLD))
    //         {
    //             return $"{subject.Encode()} is held";
    //         }
    //         else if (conditions.Contains(CONDITION.USE))
    //         {
    //             return $"{subject.Encode()} is seen";
    //         }
    //     }

    //     return "INVALID CONDITION";
    // }
}