using UnityEngine;
using System.Collections;

public class HintTile : MonoBehaviour {

	public IntVec2 position;

	void OnMouseDown()
	{
		HandRoot.Instance.MoveCurrentPlayer(position);
		HandRoot.Instance.NextState();
	}
}
