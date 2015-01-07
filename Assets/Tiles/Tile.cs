using UnityEngine;
using System.Collections;

public class Tile : MonoBehaviour {

	public IntVec2 position;
	public Board board;

	public virtual bool ValidateSides()
	{
		return false;
	}

}
