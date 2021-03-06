using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity;

public class GameManagerScript : MonoBehaviour
{

    private static GameManagerScript _instance;
    public static GameManagerScript Instance { get { return _instance; } }

    //Declaring new enum of type GameState to handle what state we're currently in
    public enum GameState
    {
        Idle,
        InitialiseDeath,
        CleanupLists,
        Dead,
        GameOver,
    }

    //Initiliase a new GameState variable called currentState to track and assign which state we are in.
    [SerializeField] private GameState currentState;

    [SerializeField] private List<GameObject> tempStringList;
    [SerializeField] private GameObject particleRed, particleBlue;

    [SerializeField] private bool moveRigidBodies;

    [SerializeField] private int stringPointIntersectedWith;
    [SerializeField] private  int count2ndHalf, count1stHalf;


    private StringMovement sM;

    public GameState CurrentState { get => currentState; set => currentState = value; }
    public int Count2ndHalf { get => count2ndHalf; set => count2ndHalf = value;  }
    public int Count1stHalf { get => count1stHalf; set => count1stHalf = value;  }
    public int StringPointIntersectedWith { get => stringPointIntersectedWith; set => stringPointIntersectedWith = value; }
    public bool MoveRigidBodies {  get => moveRigidBodies; set => moveRigidBodies = value;  }

    struct TimeThreshold 
    {
        public int Level;

        public int Bronze;
        public int Silver;
        public int Gold;

        public TimeThreshold(int level, int bronze, int silver, int gold)
        {
            this.Level = level;
            this.Bronze = bronze;
            this.Silver = silver;
            this.Gold = gold;
        }
    }


    /// <summary>
    /// List of variables on script per scene 
    /// get passed through to timethreshold struct which get passed into list or dictionary for player prefs
    /// </summary>
    List<TimeThreshold> timeThresholds;


    void Start()
    {
        //Default Game State should always be idle after string and other componenents have initialised
        currentState = GameState.Idle;

        //Grab reference to string movement script to handle death animation
        sM = FindObjectOfType<StringMovement>();
    }

    void Awake()
    {


        //Checks to see if any Game Managers are present in the scene and either delete or assign this script to them. Necessary for singleton pattern
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
            DontDestroyOnLoad(this.gameObject);
        }


    }

    void Update()
    {

        switch (currentState)
        {
            case GameState.Idle:
                break;
            case GameState.InitialiseDeath:
                InitiliaseDeath();
                break;
            case GameState.CleanupLists:
                CleanUpLists();
                break;
            case GameState.Dead:
                //See Fixed Update
                break;
            case GameState.GameOver:
                break;
        }
    }

    private void FixedUpdate()
    {
        if(currentState == GameState.Dead)
        {
            DeathAnimation();
        }
    }


    void DeathAnimation()
    {

        //Loop 1st Half of string if intersected
        if(count1stHalf < sM.StringPointsGO.Count)
        {
            //Instantiate(particleRed, sM.StringPointsData[((sM.StringPointsGO.Count - 1) - count1stHalf)], Quaternion.identity, sM.gameObject.transform);
            Destroy(sM.StringPointsGO[(sM.StringPointsGO.Count - 1) - count1stHalf]);
            count1stHalf++;
        }


        //Loop 2nd half of string if intersected
        if (count2ndHalf < tempStringList.Count)
        {
            //Instantiate(particleRed, tempStringList[count2ndHalf].transform.position, Quaternion.identity, sM.gameObject.transform);
            Destroy(tempStringList[count2ndHalf]);
            count2ndHalf++;
        }

        //Check if both animations are finished and change gamestate to game over
        if(count1stHalf == sM.StringPointsGO.Count && count2ndHalf == tempStringList.Count)
        {
            currentState = GameState.GameOver;
        }

    }

    void InitiliaseDeath()
    {
        tempStringList = new List<GameObject>();

        for (int i = stringPointIntersectedWith; i < sM.StringPointsGO.Count; i++)
        {
            tempStringList.Add(sM.StringPointsGO[i]);
        }

        count1stHalf = 0;
        Count2ndHalf = 0;
        currentState = GameState.CleanupLists;
    }

    void CleanUpLists()
    {
        int amountToRemove = sM.StringPointsData.Count - stringPointIntersectedWith;
        sM.StringPointsGO.RemoveRange(stringPointIntersectedWith, amountToRemove);
        currentState = GameState.Dead;
    }


}