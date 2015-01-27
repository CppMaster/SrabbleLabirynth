using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Hand : MonoBehaviour {

	List<Tile> tiles;
	const int initialCount = 4;
	const float offset = 1.2f;

	public int player = 0;
	public const int maxPlayers = 4;

	static GameObject tileGO = null;

	void Awake()
	{
		if(tileGO == null) tileGO = (GameObject) Resources.Load("Prefabs/GameTile");
		tiles = new List<Tile>();
	}

	void Start () 
	{
		for(int a = 0; a < initialCount; ++a)
		{
			SpawnTile();
		}
		AdjustTiles();
	}

	void SpawnTile()
	{
		GameObject go = (GameObject) Instantiate(tileGO);
		ScrabbleElement element = go.GetComponent<ScrabbleElement>();
		tiles.Add(element);
		element.MarkPlayer(player);
		go.transform.parent = transform;
		go.AddComponent<TileOnHand>().MarkPlayer(player);
	}

	[ContextMenu("SpawnNextTile")]
	public void SpawnNextTile()
	{
		SpawnTile();
		AdjustTiles();
	}

	void AdjustTiles()
	{
		float halfWidth = (tiles.Count - 1) * offset / 2;
		for(int a = 0; a < tiles.Count; ++a)
		{
			tiles[a].transform.localPosition = new Vector3(a * offset - halfWidth, 0, 0);
		}
	}

	public void RemoveFromHand(Tile tile)
	{
		tiles.Remove(tile);
	}

}
