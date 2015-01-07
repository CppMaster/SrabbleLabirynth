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
		((ScrabbleElement)tiles[size.x/2, size.y/2]).SetValue(ScrabbleElement.startElementValue);
	}

	public void Spawn(Tile tile, Vector3 pos)
	{
		IntVec2 gridPosition = new IntVec2();
		gridPosition.x = Mathf.RoundToInt(pos.x / tileOffset.x);
		gridPosition.y = Mathf.RoundToInt(pos.z / tileOffset.y);

		tile.transform.position = new Vector3(gridPosition.x * tileOffset.x,
		                                      0, gridPosition.y * tileOffset.y );
		tile.position = gridPosition;
		tile.board = this;
		tiles[gridPosition.x, gridPosition.y] = tile;
	}

	void Spawn(int x, int y)
	{
		GameObject newTile = (GameObject) Instantiate(tileGO, 
      		new Vector3(x * tileOffset.x,
		            0, y * tileOffset.y ),
      				Quaternion.identity);
		Tile tile = newTile.GetComponent<Tile>();
		tile.position = new IntVec2(x,y);
		tile.board = this;
		tiles[x,y] = tile;
	}

	public bool SnapGhost(GameObject ghost, Tile tile)
	{
		IntVec2 gridPosition = new IntVec2();
		gridPosition.x = Mathf.RoundToInt(ghost.transform.position.x / tileOffset.x);
		gridPosition.y = Mathf.RoundToInt(ghost.transform.position.z / tileOffset.y);

		if(!ValidateSpawn(gridPosition)) return false;
		if(!ValidateSides(gridPosition, tile)) return false;

		ghost.transform.position = new Vector3(
			gridPosition.x * tileOffset.x,
			0,
			gridPosition.y * tileOffset.y
			);

		return true;
	}

	public bool ValidateSides(IntVec2 pos, Tile tile)
	{
		if(!tile.ValidateSides()) return true;
		ScrabbleElement scrabble = tile.GetComponent<ScrabbleElement>();
		if(scrabble == null) return true;

		IntVec2[] neighbours = GetNeighbours(pos);
		for(int side = 0; side < 4; ++side)
		{
			Tile neighbour = tiles[neighbours[side].x, neighbours[side].y];
			if(neighbour == null) continue;
			ScrabbleElement neighbourScrabble = neighbour.GetComponent<ScrabbleElement>();
			if(neighbourScrabble == null) continue;

			int oppositeSide = (side + 2) % 4;

			int sideValue = scrabble.GetSideValue(side);
			int neighbourSideValue = neighbourScrabble.GetSideValue(oppositeSide);
			if(sideValue == 0 && neighbourSideValue != 0) return false;
			if(sideValue == 3 && neighbourSideValue != 3) return false;
			if(sideValue == 1 && neighbourSideValue != 2) return false;
			if(sideValue == 2 && neighbourSideValue != 1) return false;
		}

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
		IntVec2[] neighbours = GetNeighbours(pos);
		foreach(IntVec2 neighbour in neighbours)
		{
			if(ValidatePosition(neighbour) && !ValidateSpace(neighbour)) return true;
		}
		return false;
	}

	public IntVec2[] GetNeighbours(IntVec2 pos)
	{
		IntVec2[] neighbours = new IntVec2[4];
		neighbours[0] = new IntVec2(pos.x - 1, pos.y);
		neighbours[1] = new IntVec2(pos.x, pos.y + 1);
		neighbours[2] = new IntVec2(pos.x + 1, pos.y);
		neighbours[3] = new IntVec2(pos.x, pos.y - 1);
		return neighbours;
	}

	public bool ValidateSpawn(IntVec2 pos)
	{
		if(!ValidatePosition(pos) || !ValidateSpace(pos)) return false;
		return HasNeighbour(pos);
	}
}
