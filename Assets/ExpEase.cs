// Example:
//
// currentPos = ExpEase.Out(currentPos, targetPos, -4.0);
//
//   or
//
// ExpEase.Out2(currentPos, targetPos, -4.0); // This modifies currentPos.
//

using UnityEngine;

public static class ExpEase {
    public static float Out(float current, float target, float coeff) {
        return target - (target - current) * Mathf.Exp(coeff * Time.deltaTime); 
    }

    public static float OutAngle(float current, float target, float coeff) {
        return target - 
               Mathf.DeltaAngle(current, target) * Mathf.Exp(coeff * Time.deltaTime); 
    }

    public static Vector3 Out(Vector3 current, Vector3 target, float coeff) {
        return Vector3.Lerp(target, current, Mathf.Exp(coeff * Time.deltaTime));
    }

    public static Quaternion Out(Quaternion current, Quaternion target, float coeff) {
        if (current == target) {
            return target;
        } else {
            return Quaternion.Lerp(target, current, Mathf.Exp(coeff * Time.deltaTime));
        }
    }

    public static void Out2(ref float current, float target, float coeff) {
        current = target - (target - current) * Mathf.Exp(coeff * Time.deltaTime); 
    }

    public static void OutAngle2(ref float current, float target, float coeff) {
        current = target - 
                  Mathf.DeltaAngle(current, target) * Mathf.Exp(coeff * Time.deltaTime);
    }

    public static void Out2(ref Vector3 current, Vector3 target, float coeff) {
        current = Vector3.Lerp(target, current, Mathf.Exp(coeff * Time.deltaTime));
    }

    public static void Out2(ref Quaternion current, Quaternion target, float coeff) {
        if (current == target) {
            current = target;
        } else {
            current = Quaternion.Lerp(target, current, Mathf.Exp(coeff * Time.deltaTime));
        }
    }

    public static void OutLocalTransform(Transform current, Vector3 targetPosition,
                                         Quaternion targetRotation, float coeff) {
        coeff = Mathf.Exp(coeff * Time.deltaTime);
        current.localPosition = Vector3.Lerp(targetPosition,
                                             current.localPosition, coeff);
        if (current.localRotation == targetRotation) {
            current.localRotation = targetRotation;
        } else {
            current.localRotation = Quaternion.Lerp(targetRotation,
                                                    current.localRotation, coeff);
        }
    }
}
