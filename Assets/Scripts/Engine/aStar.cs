using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class aStar {

	//Find path from start node to destination node
	//Distance function returns distance between two adjacent nodes
    //Estimate function returns distance between any node and destination node
	//Neighbors function returns adjacent traversible hexes for given hex input (Very hefty method)
    public static Path<HexData> FindPath(HexData start,
		HexData destination,
		Func<HexData, HexData, double> distance,
		Func<HexData, HexData, double> estimate,
		Func<HexData, HexData, List<HexData>> neighbours)
		{
			//set of already checked HexData
   	 		var closed = new HashSet<HexData>();
			//queued HexData in open set
   			var queue = new PriorityQueue<double, Path<HexData>>();
			//start by adding enemy's hex to queue
    		queue.Enqueue(0, new Path<HexData> (start));
		
   			while (!queue.IsEmpty)
    		{
				//get first element on list
        		var path = queue.Dequeue();
				//check to see if this element is in the set of already checked hexes, i.o the closed set 
       			if (closed.Contains(path.LastStep))
            		continue;
				//check to see if this element is our destination hex
        		if (path.LastStep.Equals(destination))
           			return path;  //return full path to destination
			
				//if element isn't the destination hex and isn't in closed set, add it to closed set
        		closed.Add(path.LastStep);
        	
				//Go through neighbors (adjacent hexes) of current element
				foreach(HexData n in neighbours(path.LastStep, destination))
        		{
					//compute distance between current element and it's neighbor
            		double d = distance(path.LastStep, n);
					//New step added without modifying current path
            		var newPath = path.AddStep(n, d);
					//add new path to queue
            		queue.Enqueue(newPath.TotalCost + estimate(n, destination), newPath);
        		}
    	}
    	return null;
	}	
}
