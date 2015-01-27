using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Board : MonoBehaviour {

	public IntVec2 size;
	public Vector2 tileOffset;

	public HandRoot handRoot;
	public Transform tilesRoot;
	public Transform charactersRoot;
	GameObject displayedMoves = null;

	Tile[,] tiles;
	IntVec2 startPosition;

	PlayerCharacter[] players;

	public GameObject tileGO;
	public GameObject hintGO;
	
	void Start () 
	{
		tiles = new Tile[size.x,size.y];
		startPosition = new IntVec2(size.x/2, size.y/2);

		Spawn(startPosition.x, startPosition.y);
		((ScrabbleElement)tiles[startPosition.x, startPosition.y]).SetValue(ScrabbleElement.startElementValue);

		SpawnPlayers();
	}

	void SpawnPlayers()
	{
		players = new PlayerCharacter[handRoot.players];
		for(int a = 0; a < players.Length; ++a)
		{
			GameObject playerGO = (GameObject)Instantiate(PlayerCharacter.GetResourceObject());
			players[a] = playerGO.GetComponent<PlayerCharacter>();
			players[a].player = a + 1;
			playerGO.transform.parent = charactersRoot;
			MovePlayer(players[a], startPosition);
		}
	}

	public Tile GetTile(IntVec2 pos)
	{
		if(pos.x < 0 || pos.y < 0 || size.x <= pos.x || size.y <= pos.y) return null;
		return tiles[pos.x, pos.y];
	}

	public IntVec2 GetStarsPosition()
	{
		return startPosition;
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
		tile.transform.parent = tilesRoot;

		handRoot.RemoveFromHand(tile);
		handRoot.NextState();
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
		tile.transform.parent = tilesRoot;

	}

	public void MovePlayer(PlayerCharacter player, IntVec2 pos)
	{
		player.transform.position = new Vector3(
			pos.x * tileOffset.x,
			player.transform.position.y,
			pos.y * tileOffset.y
			) + player.BoardOffset;
		player.BoardPosition = pos;
	}

	public void MovePlayer(int player, IntVec2 pos)
	{
		MovePlayer(players[player], pos);
	}

	public Vector3 GridToWorldPosition(IntVec2 pos)
	{
		return new Vector3(
			pos.x * tileOffset.x,
			0,
			pos.y * tileOffset.y
			);
	}

	public IntVec2 WorldToGridPosition(Vector3 pos)
	{
		return new IntVec2(Mathf.RoundToInt(pos.x / tileOffset.x), Mathf.RoundToInt(pos.y / tileOffset.y));
	}

	public bool SnapGhost(GameObject ghost, Tile tile)
	{
		IntVec2 gridPosition = new IntVec2();
		gridPosition.x = Mathf.RoundToInt(ghost.transform.position.x / tileOffset.x);
		gridPosition.y = Mathf.RoundToInt(ghost.transform.position.z / tileOffset.y);

		if(!ValidateSpawn(gridPosition)) return false;
		if(!ValidateSides(gridPosition, tile)) return false;

		ghost.transform.position = GridToWorldPosition(gridPosition);

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

	public void ShowMovesForPlayer(int _player)
	{
		PlayerCharacter player = players[_player];
		ScrabbleElement tile = GetTile(player.BoardPosition) as ScrabbleElement;
		IntVec2[] neighbours = GetNeighbours(player.BoardPosition);
		List<IntVec2> possibilities = new List<IntVec2>();

		for(int a = 0; a < 4; ++a)
		{
			ScrabbleElement tileNeighbour = GetTile(neighbours[a]) as ScrabbleElement;
			if(tileNeighbour == null) continue;
			int sideValue = tile.GetSideValue(a);
			Debug.Log("SideValue: " + sideValue);
			if(sideValue < 2) continue;
			possibilities.Add(neighbours[a]);
		}

		SpawnHints(possibilities.ToArray());
	}

	public void SpawnHints(IntVec2[] positions)
	{
		DestroyHints();
		displayedMoves = new GameObject();
		displayedMoves.transform.parent = transform;

		foreach(IntVec2 pos in positions)
		{
			GameObject hint = (GameObject) Instantiate(hintGO);
			hint.transform.parent = displayedMoves.transform;
			hint.transform.position = GridToWorldPosition(pos) + Vector3.up * 0.6f;
			hint.GetComponent<HintTile>().position = pos;
		}
	}

	public void DestroyHints()
	{
		if(displayedMoves != null) Destroy(displayedMoves);
	}
}
