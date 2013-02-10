using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
	 
public class Path<HexData>: IEnumerable<HexData>
{
	//code from http://goo.gl/oJ6Hd
	public HexData LastStep { get; private set; }
    public Path<HexData> PreviousSteps { get; private set; }
    public double TotalCost { get; private set; }
	
    private Path(HexData lastStep, Path<HexData> previousSteps, double totalCost)
    {
        LastStep = lastStep;
        PreviousSteps = previousSteps;
        TotalCost = totalCost;
    }
	
    public Path(HexData start) : this(start, null, 0) {}
	
    public Path<HexData> AddStep(HexData step, double stepCost)
    {
        return new Path<HexData>(step, this, TotalCost + stepCost);
    }
	
    public IEnumerator<HexData> GetEnumerator()
    {
        for (Path<HexData> p = this; p != null; p = p.PreviousSteps)
            yield return p.LastStep;
    }
	
    IEnumerator IEnumerable.GetEnumerator()
    {
        return this.GetEnumerator();
    }
}
