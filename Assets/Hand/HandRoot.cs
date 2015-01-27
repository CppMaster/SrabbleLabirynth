using UnityEngine;
using System.Collections;

public class HandRoot : MonoBehaviour {

	static HandRoot instance;
	public static HandRoot Instance
	{
		get
		{
			if(instance == null) 
			{
				instance = GameObject.FindObjectOfType<HandRoot>();
			}
			return instance;
		}
	}
	
	void Awake()
	{
		instance = this;
	}


	public int players = 2;
	Hand[] hands;
	public Board board;

	public int activePlayer = 0;
	
	public GameObject handGO;
	public GameObject playerCharacterGO;

	public TurnState turnState = TurnState.SetTile;

	void Start () 
	{
		hands = new Hand[players];
		for(int a = 0; a < players; ++a)
		{
			GameObject hand = (GameObject) Instantiate(handGO);
			hands[a] = hand.GetComponent<Hand>();
			hands[a].player = a + 1;
			hand.transform.parent = transform;
			hand.transform.localPosition = Vector3.zero;
		}

		SetActivePlayer(0);
	}

	public void SetActivePlayer(int player)
	{
		activePlayer = player;
		for(int a = 0; a < hands.Length; ++a)
		{
			hands[a].gameObject.SetActive(a == activePlayer);
		}
	}

	public void NextState()
	{
		switch(turnState)
		{
		case TurnState.SetTile:
			turnState = TurnState.MoveCharacter;
			board.ShowMovesForPlayer(activePlayer);
			break;
		case TurnState.MoveCharacter:
			turnState = TurnState.SetTile;
			board.DestroyHints();
			NextActivePlayer();
			SpawnNextTile();
			break;
		}
	}

	[ContextMenu("NextActivePlayer")]
	public void NextActivePlayer()
	{
		SetActivePlayer((activePlayer + 1) % players);
	}

	public void RemoveFromHand(Tile tile)
	{
		hands[activePlayer].RemoveFromHand(tile);
	}

	[ContextMenu("SpawnNextTile")]
	public void SpawnNextTile()
	{
		hands[activePlayer].SpawnNextTile();
	}

	public void MoveCurrentPlayer(IntVec2 pos)
	{
		board.MovePlayer(activePlayer, pos);
	}

}
