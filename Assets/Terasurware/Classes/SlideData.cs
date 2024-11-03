using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SlideData : ScriptableObject
{	
	public List<Sheet> sheets = new List<Sheet> ();

	[System.SerializableAttribute]
	public class Sheet
	{
		public string name = string.Empty;
		public List<Param> list = new List<Param>();
	}

	[System.SerializableAttribute]
	public class Param
	{
		
		public double Frame;
		public string TexturePath;
		public string Message;
		public double PositionX;
		public double PositionY;
		public double MoveSpeedX;
		public double MoveSpeedY;
		public double Scale;
		public double ShowDuration;
		public double FadeInDuration;
		public double FadeOutDuration;
	}
}

