using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
	 
public class Path: IEnumerable<HexData>
{
	//code from http://goo.gl/oJ6Hd
	public HexData LastStep { get; private set; }
    public Path PreviousSteps { get; private set; }
    public double TotalCost { get; private set; }
	public int Count { get; private set;}
	
    private Path(HexData lastStep, Path previousSteps, double totalCost, int totalCount)
    {
		Count = totalCount;
        LastStep = lastStep;
        PreviousSteps = previousSteps;
        TotalCost = totalCost;
    }
	
	
    public Path(HexData start) : this(start, null, 0, 1) {}
	
    public Path AddStep(HexData step, double stepCost) 
    {
		Count++;
		step.traversal_cost = (int) stepCost;
//		HexD val = (HexData) step;
		
        return new Path (step, this, TotalCost + stepCost, Count);
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
            yield return p.LastStep;
		}
    }
	
    IEnumerator IEnumerable.GetEnumerator()
    {
        return this.GetEnumerator();
    }
}
