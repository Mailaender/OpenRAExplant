using UnityEngine;

	// Use this for initialization
        MyTransform = transform;
        OffsetTransform.position = OffsetTransform.TransformPoint(DefaultOffset);
        return OffsetTransform.position * 0.9f + MyTransform.position;
            List<Vector3> pos = new List<Vector3>();
                if (cameraVolume.Time == 0)
                {
                    OffsetTransform.position = cameraVolume.CameraOffset.localPosition;
                    //CameraBehaviour.Instance.Reset();
                }
                else
                {
                    pos.Add(cameraVolume.CameraOffset.localPosition);
                    iTween.moveToBezier(Offset, cameraVolume.Time, 0, pos);
                }
     //   Debug.Log("activate " + t.position);
        OffsetTransform.position = t.TransformDirection(DefaultOffset);
        CameraBehaviour.Instance.Reset();