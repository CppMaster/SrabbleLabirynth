using UnityEngine;
using System.Collections;

public class TileOnHand : MonoBehaviour {

	const float handHeight = 5f;
	const float boardHeight = 0f;

	Camera cam;
	Board board;
	bool isValid = false;

	public GameObject ghost;

	void Awake()
	{
		cam = Camera.main;
		board = GameObject.FindObjectOfType<Board>();
	}

	void OnMouseDrag()
	{
		SetTilePosition();
		SetGhostPosition();
	}

	void OnMouseUp()
	{
		if(!isValid) return;
		board.Spawn(GetComponent<Tile>(), ghost.transform.position);
		Destroy(ghost);
		Destroy(this);
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
		isValid = board.SnapGhost(ghost);
		ghost.renderer.enabled = isValid;
	}
}
