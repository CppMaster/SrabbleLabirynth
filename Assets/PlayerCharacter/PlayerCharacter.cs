using UnityEngine;
using System.Collections;

public class PlayerCharacter : MonoBehaviour {

	public int player = 0;


	static Material[] playerMaterials;
	static GameObject playerGO;

	Vector3 boardOffset = Vector3.zero;

	IntVec2 boardPosition = new IntVec2();
	public IntVec2 BoardPosition
	{
		get{return boardPosition;}
		set{boardPosition = value;}
	}

	void Start () 
	{
		if(player > 0)
		{
			LoadResources();
			renderer.material = playerMaterials[player - 1];
		}
	}

	const float boardOffsetFactor = 0.1f;
	bool boardOffsetSetted = false;
	void SetBoardOffset()
	{
		switch(player)
		{
		case 1:
			boardOffset = new Vector3(-boardOffsetFactor, 0f, -boardOffsetFactor);
			break;
		case 2:
			boardOffset = new Vector3(boardOffsetFactor, 0f, boardOffsetFactor);
			break;
		case 3:
			boardOffset = new Vector3(-boardOffsetFactor, 0f, boardOffsetFactor);
			break;
		case 4:
			boardOffset = new Vector3(boardOffsetFactor, 0f, -boardOffsetFactor);
			break;
		}
		boardOffsetSetted = true;
	}


	static void LoadResources()
	{
		if(playerMaterials != null) return;
		playerMaterials = new Material[Hand.maxPlayers];
		for(int a = 0; a < Hand.maxPlayers; ++a)
		{
			playerMaterials[a] = (Material) Resources.Load("Materials/PlayerCharacter" + (a + 1));
		}
		
	}

	public static GameObject GetResourceObject()
	{
		if(playerGO == null) playerGO = (GameObject) Resources.Load("Prefabs/PlayerCharacter");
		return playerGO;
	}

	public Vector3 BoardOffset
	{
		get
		{
			if(!boardOffsetSetted) SetBoardOffset();
			return boardOffset;
		}
	}
}
