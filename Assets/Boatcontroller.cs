using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoatController : MonoBehaviour
{
    public Transform startPosition;
    public BFSController bfsController;

    private List<Vector3> boatPositions; // Danh sách các vị trí cố định của thuyền (A và B).
    private int currentBoatPositionIndex = 0;

    public int MissionariesOnBoat { get; set; }
    public int CanibalesOnBoat { get; set; }

    public int CurrentBoatPositionIndex => currentBoatPositionIndex;

    private void Start()
    {
        InitializeBoatPositions();
    }

    private void InitializeBoatPositions()
    {
        boatPositions = new List<Vector3>
        {
            new Vector3(1.77f, -1.65f, 0f),   // Vị trí thuyền A
            new Vector3(-2.27f, -1.65f, 0f),  // Vị trí thuyền B
        };
    }

    public void MoveBoat()
    {
        // Kiểm tra điều kiện số lượng người và quỷ trên thuyền
        if (MissionariesOnBoat + CanibalesOnBoat >= 1 && MissionariesOnBoat + CanibalesOnBoat <= 2)
        {
            if (currentBoatPositionIndex == 0)
            {
                currentBoatPositionIndex = 1;
            }
            else
            {
                currentBoatPositionIndex = 0;
            }
        }
    }

}
