using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VideoScene : MonoBehaviour {

    public UnitClass TurnCoat1;
    public UnitClass TurnCoat2;
    public UnitClass TurnCoat3;
    public RuntimeAnimatorController ToChangeTo;
    public UnitClass Attacker;
    public UnitClass Defender;
    public UnitClass Attacker1;
    public UnitClass Defender1;
    private 
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            TurnCoat1.GetComponent<Animator>().runtimeAnimatorController = ToChangeTo;
            TurnCoat2.GetComponent<Animator>().runtimeAnimatorController = ToChangeTo;
            TurnCoat3.GetComponent<Animator>().runtimeAnimatorController = ToChangeTo;
            //print("GotHereasdfasdf");
        }
        if (Input.GetKeyDown(KeyCode.T))
        {
            Attacker.GetComponent<Animator>().Play("Attack");
            StartCoroutine(Example());
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            Attacker1.GetComponent<Animator>().Play("Attack");
            StartCoroutine(Example1());
        }
    }
    IEnumerator Example()
    {
        
        yield return new WaitForSeconds(1);
        
        Attacker.CurrentTile.attack(Attacker, Defender, Attacker.currentTile, Defender.currentTile);
    }
    IEnumerator Example1()
    {

        yield return new WaitForSeconds(1);

        Attacker1.CurrentTile.attack(Attacker1, Defender1, Attacker1.currentTile, Defender1.currentTile);
    }
}
