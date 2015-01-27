using UnityEngine;
using System.Collections;

public class TileOnHand : MonoBehaviour {

	const float handHeight = 5f;
	const float boardHeight = 0f;

	Camera cam;
	Board board;
	bool isValid = false;
	Tile tile;

	int player;

	public GameObject ghost;

	static GameObject ghostGO;
	static Material[] ghostMaterials;

	void Awake()
	{
		cam = Camera.main;
		tile = GetComponent<Tile>();
		board = GameObject.FindObjectOfType<Board>();

		LoadResources();

		ghost = (GameObject)Instantiate(ghostGO);
		ghost.transform.parent = transform;
		ghost.renderer.enabled = false;
		ghost.renderer.material = ghostMaterials[player];
		((ScrabbleElement)tile).SetValue(ScrabbleElement.GetValidRandomValue());
	}

	static void LoadResources()
	{
		if(ghostGO != null) return;
		ghostGO = (GameObject)Resources.Load("Prefabs/Ghost");
		ghostMaterials = new Material[Hand.maxPlayers + 1];
		for(int a = 0; a <= Hand.maxPlayers; ++a)
		{
			ghostMaterials[a] = (Material) Resources.Load("Materials/PlayerGhost" + a);
		}

	}

	void OnMouseDrag()
	{
		if(HandRoot.Instance.turnState == TurnState.SetTile)
		{
			SetTilePosition();
			SetGhostPosition();
		}
	}

	void OnMouseUp()
	{
		if(!isValid) return;
		board.Spawn(tile, ghost.transform.position);
		Destroy(ghost);
		Destroy(this);
		isValid = false;
	}

	void SetTilePosition()
	{
		float heightDifference = handHeight - cam.transform.position.y;
		float distance = heightDifference / cam.transform.forward.y;
		transform.position = cam.ScreenToWorldPoint(new Vector3(
			Input.mousePosition.x,
			Input.mousePosition.y,
			distance
			));
	}

	void SetGhostPosition()
	{
		ghost.transform.position = transform.position;
		isValid = board.SnapGhost(ghost, tile);
		ghost.renderer.enabled = isValid;
	}

	public void MarkPlayer(int _player)
	{
		player = _player;
		if(ghost) ghost.renderer.material = ghostMaterials[player];
	}
}
