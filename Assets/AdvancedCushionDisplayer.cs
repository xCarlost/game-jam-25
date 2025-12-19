                                                                                                                                                                                                                                                                                                                                                                                                                                                                    using Cynteract.InputDevices;
using UnityEngine;
using UnityEngine.Rendering;
/// <summary>
/// Simple script to rotate a cube with cushion rotation
/// </summary>
public class AdvancedCushionDisplayer : MonoBehaviour
{
    public float playerheight = 2;
    public float sidestepLimit = 2;
    public float stepFactor = 0.1f;
    public float backstepLimit = 2;
    public float duckLimit = 2;
    public float duckfactor = 0.5f;

    [SerializeField]
    private Transform PlayerJoint;
    [SerializeField]
    public GameObject CameraPos;
    private CushionData cushionData;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //Make sure a CynteractDeviceManager script is present in the scene
        //Wait for the device to be ready
        CynteractDeviceManager.Instance.ListenOnReady(device =>
        {
            //Get the corresponding data
            //This assumes, the device is a cushion
            cushionData = new CushionData(device);
        });
    }

    // Update is called once per frame
    void Update()
    {
        if (cushionData == null)
        {
            return;
        }
        // Get the absolute (unmodified) rotation of the cushions main and only sensor (Palm Center bc it was on a glove first)
        var rotation = cushionData.GetAbsoluteRotationOfPartOrDefault(FingerPart.palmCenter).eulerAngles;
        //Set the rotation of the cube to the rotation of the cushion
        var position = Vector3.zero;

        rotation.y = 0;

        if (rotation.z < 180 && rotation.z > 0)
        {
            rotation.z = Mathf.Clamp(rotation.z, 0, 10);
            position.Set(Mathf.Clamp(-rotation.z*stepFactor,-sidestepLimit,0), 0, 0);
        }
        else
        {
            rotation.z = Mathf.Clamp(rotation.z, 350, 360);
            position.Set(Mathf.Clamp((360 -rotation.z)*stepFactor,0,sidestepLimit), 0, 0);
        }


        if (rotation.x < 180 && rotation.x > 0)
        {
            rotation.x = Mathf.Clamp(rotation.x, 0, 10);
            CameraPos.transform.SetLocalPositionAndRotation(new Vector3(0, Mathf.Clamp((playerheight-duckLimit)/(-rotation.x)+duckLimit, duckLimit, playerheight), 0), Quaternion.Euler(Vector3.zero));
            rotation.x = 0;
        }else if (rotation.x > 180 && rotation.x < 355)
        {
            rotation.x = Mathf.Clamp(rotation.x, 350, 360);
            CameraPos.transform.localPosition = new Vector3(0, playerheight, 0);
            CameraPos.transform.localRotation = Quaternion.Euler(new Vector3((360 - rotation.x) / 1.5f, 0, 0));

            position.z = Mathf.Clamp((rotation.x - 355) * stepFactor * 0.1f, -backstepLimit, 0);
        }
            
        PlayerJoint.SetLocalPositionAndRotation(position, Quaternion.Euler(rotation));
    }
}
