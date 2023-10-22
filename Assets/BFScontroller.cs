using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class State
{
    public int MissionariesOnLeft { get; set; }
    public int CannibalsOnLeft { get; set; }
    public int BoatPosition { get; set; }

    public State(int missionariesOnLeft, int cannibalsOnLeft, int boatPosition)
    {
        MissionariesOnLeft = missionariesOnLeft;
        CannibalsOnLeft = cannibalsOnLeft;
        BoatPosition = boatPosition;

    }

    public bool OnBoat(int characterIndex)
    {
        // Kiểm tra xem nhân vật có trên thuyền hay không bằng cách so sánh vị trí của thuyền với vị trí của nhân vật.
        if (BoatPosition == 1)
        {
            // Thuyền ở bờ phải, kiểm tra xem nhân vật có ở trên thuyền ở bờ phải không.
            return characterIndex >= 3; // Nhân vật từ index 3 trở đi là nhân vật ở bờ phải.
        }
        else
        {
            // Thuyền ở bờ trái, kiểm tra xem nhân vật có ở trên thuyền ở bờ trái không.
            return characterIndex < 3; // Nhân vật từ index 0 đến 2 là nhân vật ở bờ trái.
        }
    }

    public bool OnLeftBank(int characterIndex)
    {
        // Kiểm tra xem nhân vật có ở bên trái bờ sông hay không bằng cách so sánh index của nhân vật.
        return characterIndex < 3;
    }

    public bool OnRightBank(int characterIndex)
    {
        // Kiểm tra xem nhân vật có ở bên phải bờ sông hay không bằng cách so sánh index của nhân vật.
        return characterIndex >= 3;
    }
    // Định nghĩa hàm Equals để so sánh trạng thái với nhau.
    public bool Equals(State otherState)
    {
        return MissionariesOnLeft == otherState.MissionariesOnLeft &&
               CannibalsOnLeft == otherState.CannibalsOnLeft &&
               BoatPosition == otherState.BoatPosition;
    }

    // Định nghĩa hàm GetHashCode để sử dụng cho việc xây dựng HashSet hoặc Dictionary.
    public override int GetHashCode()
    {
        int hash = 17;
        hash = hash * 23 + MissionariesOnLeft.GetHashCode();
        hash = hash * 23 + CannibalsOnLeft.GetHashCode();
        hash = hash * 23 + BoatPosition.GetHashCode();
        return hash;
    }
}


public class BFSController : MonoBehaviour
{
    public Transform startPosition;
  //  public CharacterController characterController; // Tham chiếu đến script CharacterController
    public BoatController boatController;
    public List<Character> characters; // Danh sách các nhân vật (quỷ và sư).

    private List<Vector3> boatPositions; // Danh sách các vị trí cố định của thuyền (A và B).
    private List<Vector3> characterPositions; // Danh sách các vị trí cố định của nhân vật (ban đầu, bờ phải, bờ trái).

    private int currentStep = 0;
    public List<State> solution;

    private void Start()
    {
        InitializeCharacterPositions();
        InitializeBoatPositions();
        solution = BFS();

        if (solution != null)
        {
            Debug.Log("Found a solution with " + solution.Count + " steps.");
        }
        else
        {
            Debug.Log("No solution found.");
        }
    }

    private void InitializeCharacterPositions()
    {
        characterPositions = new List<Vector3>
        {
            new Vector3(4.27f, -0.69f, 0f),   // Quỷ 1
            new Vector3(6.84f, -0.64f, 0f),   // Quỷ 2
            new Vector3(10.6272f, -1.68762f, 0f), // Quỷ 3
            new Vector3(5.45f, -1.62f, 0f),   // Sư 1
            new Vector3(7.73f, -1.85f, 0f),   // Sư 2
            new Vector3(9.17f, -0.64f, 0f),   // Sư 3
        };
    }

    private void InitializeBoatPositions()
    {
        boatPositions = new List<Vector3>
        {
            new Vector3(1.77f, -1.65f, 0f),   // Vị trí thuyền A
            new Vector3(-2.27f, -1.65f, 0f),  // Vị trí thuyền B
        };
    }



    private void MoveCharacters(State nextState)
    {
        boatController.MoveBoat(); // Di chuyển thuyền đến vị trí tiếp theo (A hoặc B).
        Vector3 boatPosition = boatPositions[boatController.CurrentBoatPositionIndex];

        for (int i = 0; i < characters.Count; i++)
        {
            CharacterController characterController = characters[i].GetComponent<CharacterController>();
            if (nextState.OnBoat(i))
            {
                characterController.MoveCharacterToPosition(boatPosition);
            }
            else if (nextState.OnLeftBank(i))
            {
                characterController.MoveCharacterToPosition(characterPositions[2]);
            }
            else if (nextState.OnRightBank(i))
            {
                characterController.MoveCharacterToPosition(characterPositions[1]);
            }
        }
    }





    private void Update()
    {
        if (currentStep < solution.Count)
        {
            MoveCharacters(solution[currentStep]);
            currentStep++;
        }
    }

    // BFS Algorithm
    private List<State> BFS()
    {
        State initialState = new State(3, 3, 1);
        State goalState = new State(0, 0, 0);

        Queue<List<State>> queue = new Queue<List<State>>();
        List<State> initialPath = new List<State> { initialState };
        queue.Enqueue(initialPath);

        while (queue.Count > 0)
        {
            List<State> currentPath = queue.Dequeue();
            State currentState = currentPath[currentPath.Count - 1];

            if (currentState.Equals(goalState))
            {
                return currentPath; // Trả về lời giải nếu đạt được trạng thái kết thúc.
            }

            // Tìm tất cả các trạng thái kế tiếp hợp lệ và thêm vào hàng đợi.
            List<State> nextStates = FindNextStates(currentState);
            foreach (State nextState in nextStates)
            {
                if (!currentPath.Contains(nextState))
                {
                    List<State> newPath = new List<State>(currentPath);
                    newPath.Add(nextState);
                    queue.Enqueue(newPath);
                }
            }
        }

        return null; // Trả về null nếu không tìm thấy lời giải.
    }

    private List<State> FindNextStates(State currentState)
    {
        List<State> nextStates = new List<State>();

        int maxMove = 2; // Số lượng người hoặc quỷ tối đa có thể di chuyển trong một lần.

        for (int missionaries = 0; missionaries <= maxMove; missionaries++)
        {
            for (int canibales = 0; canibales <= maxMove; canibales++)
            {
                int direction = -1; // Mặc định qua trái.
                if (currentState.BoatPosition == 1)
                {
                    direction = 1; // Nếu thuyền ở bờ phải, qua phải.
                }

                int newMissionariesOnLeft = currentState.MissionariesOnLeft - (missionaries * direction);
                int newCannibalesOnLeft = currentState.CannibalsOnLeft - (canibales * direction);
                int newBoatPosition = 1 - currentState.BoatPosition; // Thay đổi vị trí thuyền.

                if (newMissionariesOnLeft >= 0 && newCannibalesOnLeft >= 0 &&
                    (3 - newMissionariesOnLeft) >= 0 && (3 - newCannibalesOnLeft) >= 0 &&
                    (newMissionariesOnLeft == 0 || newMissionariesOnLeft >= newCannibalesOnLeft) &&
                    ((3 - newMissionariesOnLeft) == 0 || (3 - newMissionariesOnLeft) >= (3 - newCannibalesOnLeft)))
                {
                    nextStates.Add(new State(newMissionariesOnLeft, newCannibalesOnLeft, newBoatPosition));
                }
            }
        }

        return nextStates;
    }
}
