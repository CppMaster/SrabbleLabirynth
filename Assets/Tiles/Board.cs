using UnityEngine;
using System.Collections;

public class Board : MonoBehaviour {

	public IntVec2 size;
	public Vector2 tileOffset;

	public GameObject tileGO;

	Tile[,] tiles;
	
	void Start () 
	{
		tiles = new Tile[size.x,size.y];

		Spawn(size.x/2, size.y/2);
	}

	public void Spawn(Tile tile, Vector3 pos)
	{
		IntVec2 gridPosition = new IntVec2();
		gridPosition.x = Mathf.RoundToInt(pos.x / tileOffset.x);
		gridPosition.y = Mathf.RoundToInt(pos.z / tileOffset.y);

		tile.transform.position = new Vector3(gridPosition.x * tileOffset.x,
		                                      0, gridPosition.y * tileOffset.y );
		tiles[gridPosition.x, gridPosition.y] = tile;
	}

	void Spawn(int x, int y)
	{
		GameObject newTile = (GameObject) Instantiate(tileGO, 
      		new Vector3(x * tileOffset.x,
		            0, y * tileOffset.y ),
      				Quaternion.identity);
		tiles[x,y] = newTile.GetComponent<Tile>();
	}

	public bool SnapGhost(GameObject ghost)
	{
		IntVec2 gridPosition = new IntVec2();
		gridPosition.x = Mathf.RoundToInt(ghost.transform.position.x / tileOffset.x);
		gridPosition.y = Mathf.RoundToInt(ghost.transform.position.z / tileOffset.y);

		if(!ValidateSpawn(gridPosition)) return false;

		ghost.transform.position = new Vector3(
			gridPosition.x * tileOffset.x,
			0,
			gridPosition.y * tileOffset.y
			);

		return true;
	}

	public bool ValidatePosition(IntVec2 pos)
	{
		if(pos.x < 0) return false;
		if(pos.x >= size.x) return false;
		if(pos.y < 0) return false;
		if(pos.y >= size.y) return false;
		return true;
	}

	public bool ValidateSpace(IntVec2 pos)
	{
		if(tiles[pos.x, pos.y] == null) return true;
		return false;
	}

	public bool HasNeighbour(IntVec2 pos)
	{
		IntVec2[] neighbours = new IntVec2[4];
		neighbours[0] = new IntVec2(pos.x, pos.y + 1);
		neighbours[1] = new IntVec2(pos.x, pos.y - 1);
		neighbours[2] = new IntVec2(pos.x + 1, pos.y);
		neighbours[3] = new IntVec2(pos.x - 1, pos.y);
		foreach(IntVec2 neighbour in neighbours)
		{
			if(ValidatePosition(neighbour) && !ValidateSpace(neighbour)) return true;
		}
		return false;
	}

	public bool ValidateSpawn(IntVec2 pos)
	{
		if(!ValidatePosition(pos) || !ValidateSpace(pos)) return false;
		return HasNeighbour(pos);
	}
}
