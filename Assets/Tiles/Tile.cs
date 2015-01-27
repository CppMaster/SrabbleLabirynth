using UnityEngine;
using System.Collections;

public class Tile : MonoBehaviour {

	public IntVec2 position;
	public Board board;

	protected static Material[] tileMaterials = null;

	public virtual bool ValidateSides()
	{
		return false;
	}

	static void LoadResources()
	{
		if(tileMaterials != null) return;
		tileMaterials = new Material[Hand.maxPlayers + 1];
		for(int a = 0; a <= Hand.maxPlayers; ++a)
		{
			tileMaterials[a] = (Material) Resources.Load("Materials/PlayerCube" + a);
		}
		
	}

	public static Material GetCubeMaterial(int player)
	{
		LoadResources();
		return tileMaterials[player];
	}

}
