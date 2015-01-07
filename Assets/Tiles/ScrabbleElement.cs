using UnityEngine;
using System.Collections;

public class ScrabbleElement : Tile {

	public int value = 0;
	const int states = 4;
	public const int possibilities = states * states * states * states;
	public const int startElementValue = possibilities - 1;

	[ContextMenu ("Start")]
	void Start () 
	{
		DecodeValue();
	}

	void DecodeValue()
	{
		value %= possibilities;
		int tempValue = value;

		Transform el = transform.FindChild("element");
		for(int a = 0; a < 4; ++a)
		{
			Transform side = el.FindChild("side" + a);
			if(side == null) continue;
			switch(tempValue % states)
			{
			case 0:
				side.FindChild("in").gameObject.SetActive(false);
				side.FindChild("out").gameObject.SetActive(false);
				side.FindChild("inout").gameObject.SetActive(false);
				break;
			case 1:
				side.FindChild("in").gameObject.SetActive(true);
				side.FindChild("out").gameObject.SetActive(false);
				side.FindChild("inout").gameObject.SetActive(false);
				break;
			case 2:
				side.FindChild("in").gameObject.SetActive(false);
				side.FindChild("out").gameObject.SetActive(true);
				side.FindChild("inout").gameObject.SetActive(false);
				break;
			case 3:
				side.FindChild("in").gameObject.SetActive(false);
				side.FindChild("out").gameObject.SetActive(false);
				side.FindChild("inout").gameObject.SetActive(true);
				break;
			}
			tempValue /= states;
		}
	}

	public void SetValue(int val)
	{
		value = val;
		DecodeValue();
	}

	public override bool ValidateSides ()
	{
		return true;
	}

	static public bool IsValid(int val)
	{
		bool isIn = false;
		bool isOut = false;
		val %= possibilities;
		int tempValue = val;
		for(int a = 0; a < 4; ++a)
		{
			switch(tempValue % states)
			{
			case 1:
				isIn = true;
				break;
			case 2:
				isOut = true;
				break;
			case 3:
				isIn = true;
				isOut = true;
				break;
			}
			tempValue /= states;
		}
		return isIn && isOut;
	}

	static public int GetValidRandomValue()
	{
		int val = 255;
		while(!IsValid(val = Random.Range(0, possibilities)));
		return val;
	}

	static public int GetSideValue(int value, int side)
	{
		side /= 4;
		value /= possibilities;
		while(side-- > 0)
		{
			value /= states;
		}
		return value % states;
	}

	public int GetSideValue(int side)
	{
		side %= 4;
		int val = value;
		while(side-- > 0)
		{
			val /= states;
		}
		return val % states;
	}
	
}
