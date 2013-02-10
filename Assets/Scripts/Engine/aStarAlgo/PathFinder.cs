using System;
using System.Collections.Generic;
using System.Linq;
 
public static class PathFinder
{
	//distance f-ion should return distance between two adjacent nodes
    //estimate should return distance between any node and destination node
    public static Path<HexData> FindPath<HexData>(
    	HexData start,
    	HexData destination,
    	Func<HexData, HexData, double> distance,
    	Func<HexData, double> estimate)
		
    	where HexData : IHasNeighbours<HexData>
		{
   	 		var closed = new HashSet<HexData>();
   			var queue = new PriorityQueue<double, Path<HexData>>();
    		queue.Enqueue(0, new Path<HexData>(start));
   			while (!queue.IsEmpty)
    		{
        		var path = queue.Dequeue();
       			if (closed.Contains(path.LastStep))
            		continue;
        		if (path.LastStep.Equals(destination))
           			return path;
        			closed.Add(path.LastStep);
        	foreach(HexData n in path.LastStep.Neighbours)
        	{
            	double d = distance(path.LastStep, n);
            	var newPath = path.AddStep(n, d);
            	queue.Enqueue(newPath.TotalCost + estimate(n), newPath);
        	}
    	}
    	return null;
	}
}