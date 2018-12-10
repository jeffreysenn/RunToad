using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ConstructType
{
    Undecided,
    Floor,
    Hole,
    Block,
    FloatBlock
}

public class LevelContructor : MonoBehaviour {

    public float constructDistance = 50;
    public float constructAcceptance = 10;
    public float constructEveryDistance = 3;

    public GameObject floor, block, hole;

    public float floorChance = .6f;
    public float holeChance = .2f;
    public float blockChance = .1f;
    public float floatBlockChance = .1f;

    public float blockHeight = .5f;
    public float floatBlockHeight = 1;

    
    private ConstructType constructType = ConstructType.Undecided;
    private GameObject playerCharacterObj;
    private float preConstructZ = 0;

	void Start ()
    {
        playerCharacterObj = GameObject.FindGameObjectWithTag("Player");
        preConstructZ = playerCharacterObj.transform.position.z + constructDistance;
    }

    void Update ()
    {
        float targetZ = playerCharacterObj.transform.position.z + constructDistance;

        MoveZTo(targetZ);

        if (ReachNextConstructionPosition(targetZ))
        {
            if (constructType == ConstructType.Undecided) { DecideContructionType(); }
            else { Construct(); }
        }

    }

    private void DecideContructionType()
    {
        float dice = Random.Range(0.0f, 1.0f);
        if (0 <= dice && dice < floorChance) { constructType = ConstructType.Floor; }
        else if (dice < floorChance + holeChance) { constructType = ConstructType.Hole; }
        else if (dice < floorChance + holeChance + blockChance) { constructType = ConstructType.Block; }
        else if (dice < floorChance + holeChance + floatBlockChance) { constructType = ConstructType.FloatBlock; }
        // Debug.Log(constructType);
    }

    private void Construct()
    {
        switch (constructType)
        {
            case (ConstructType.Floor):
                if (floor == null) { break; }
                ConstructFloor();
                break;
            case (ConstructType.Block):
                if(floor == null) { break; }
                ConstructFloor();
                // ConstructBlock();
                break;
            case (ConstructType.Hole):
                if (hole == null) { break; }
                ConstructHole();
                break;
            case (ConstructType.FloatBlock):
                if (floor == null) { break; }
                ConstructFloor();
                break;
            default:
                break;
        }

        preConstructZ = transform.position.z;
        constructType = ConstructType.Undecided;
    }

    private void MoveZTo(float targetZ)
    {
        transform.position = new Vector3(transform.position.x, transform.position.y, targetZ);
    }

    private bool ReachNextConstructionPosition(float targetZ)
    {
        float constructZ = preConstructZ + constructEveryDistance;
        if(Mathf.Abs(targetZ - constructZ) < constructAcceptance)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, constructZ);
            return true;
        }
        return false;
    }

    private void ConstructFloor() { GameObject.Instantiate(floor, transform.position, transform.rotation); }
    private void ConstructBlock() { GameObject.Instantiate(block, transform.position + transform.up * blockHeight, transform.rotation); }
    private void ConstructHole() { GameObject.Instantiate(hole, transform.position, transform.rotation); }
}
