using UnityEngine;
using System.Collections;
using System;
using System.Text;

public class UniqueIdGenerator{

	public static string GenerateId(){
		Guid guid = Guid.NewGuid();
		String[] parts = guid.ToString().Split('-');
		string timeNow = DateTime.Now.Ticks.ToString();
		
		StringBuilder sb = new StringBuilder();
		sb.Append(parts[0]);
		sb.Append(timeNow);
		string uniqueId = sb.ToString();

		return uniqueId;
	}
}
