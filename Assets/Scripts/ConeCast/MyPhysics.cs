using System.Collections.Generic;
using UnityEngine;

// Source: https://github.com/walterellisfun/ConeCast
public static class MyPhysics {
    public static RaycastHit[] ConeCastAll(Vector3 origin, float maxRadius, Vector3 direction, float maxDistance, float coneAngle) {
        RaycastHit[] sphereCastHits = Physics.SphereCastAll(origin - new Vector3(0, 0, maxRadius), maxRadius, direction, maxDistance);
        List<RaycastHit> coneCastHitList = new List<RaycastHit>();

        if (sphereCastHits.Length > 0) {
            for (int i = 0; i < sphereCastHits.Length; i++) {
                sphereCastHits[i].collider.gameObject.GetComponent<Renderer>().material.color = new Color(1f, 1f, 1f);
                Vector3 hitPoint = sphereCastHits[i].point;
                // Changed that 'cause it was also looking backwards...
                /*
                Vector3 directionToHit = hitPoint - origin;
                float angleToHit = Vector3.Angle(direction, directionToHit);
                if (angleToHit < coneAngle) {
                    coneCastHitList.Add(sphereCastHits[i]);
                }
                */
                float angleToPosition = Vector3.Angle(direction, sphereCastHits[i].collider.gameObject.transform.position - origin);
                if (angleToPosition < coneAngle) {
                    coneCastHitList.Add(sphereCastHits[i]);
                }
            }
        }

        RaycastHit[] coneCastHits = new RaycastHit[coneCastHitList.Count];
        coneCastHits = coneCastHitList.ToArray();

        return coneCastHits;
    }
}