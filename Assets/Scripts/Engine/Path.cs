using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
	 using UnityEngine;

public class Path: IEnumerable<HexData>
{
	//code from http://goo.gl/oJ6Hd
	public HexData ThisStep { get; private set; }
	public Path FirstStep { get; private set; }
    public Path PreviousSteps { get; private set; }
    public Path NextStep { get; private set; }
    public double TotalCost { get; private set; }
	public int Count { get; private set;}
	
    private Path(HexData thisStep, Path previousSteps, double totalCost, int totalCount)
    {
		Count = totalCount;
        ThisStep = thisStep;
        PreviousSteps = previousSteps;
        TotalCost = totalCost;
		NextStep = null;
    }
	
    public Path(HexData start) : this(start, null, 0, 1) { 
	}
	
    public Path AddStep(HexData step, double stepCost) 
    {
		if(FirstStep == null)
		{ 
			FirstStep = this;
		}
		Count++;
		step.traversal_cost = (int) stepCost;
		NextStep = new Path (step, this, TotalCost + stepCost, Count);
//		HexD val = (HexData) step;
		
        return NextStep;
    }
	
	public int getPathLength()
	{
		int i = 0;
        for (Path p = this; p != null; p = p.PreviousSteps)
            i++;
		return i;
	}
	
    public IEnumerator<HexData> GetEnumerator()
    {
        for (Path p = this; p != null; p = p.PreviousSteps)
		{	
            yield return p.ThisStep;
		}
    }
//    public IEnumerator<HexData> GetStartEnumerator()
//    {
//			Debug.LogWarning("GetStartEnumerator");
//			Debug.LogWarning("first " + FirstStep.);
//        for (Path p = this.FirstStep; p != null; p = p.NextStep)
//		{	
//			Debug.LogWarning("hrm");
//            yield return p.ThisStep;
//		}
//    }
	
    IEnumerator IEnumerable.GetEnumerator()
    {
        return this.GetEnumerator();
    }
	
	public List<HexData> getTraverseOrderList()
	{
		List<HexData> list = new List<HexData>();
		foreach (HexData hex in this)
			list.Add(hex);
		
		list.Reverse();	//path comes in backwards
		return list;
	}
}
