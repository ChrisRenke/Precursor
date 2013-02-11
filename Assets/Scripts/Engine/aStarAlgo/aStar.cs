using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class aStar : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	//A star
	public static Path<HexData> search(HexData start, HexData end){
		//TODO: for readablility , will combine methods later, calling pathfinder method
		
		/*
		//We assume that the distance between any two adjacent tiles is 1
        //If you want to have some mountains, rivers, dirt roads or something else which might slow down the player you should replace the function with something that suits better your needs
        Func<Tile, Tile, double> distance = (node1, node2) => 1;

        var path = PathFinder.FindPath(start, end, 
            distance, estimate);
		*/
		
		return FindPath(start,end,);
	}
	
	//distance f-ion should return distance between two adjacent nodes
    //estimate should return distance between any node and destination node
    public static Path<HexData> FindPath<HexData>(
    	HexData start,
    	HexData destination,
    	Func<HexData, HexData, double> distance,
    	Func<HexData, double> estimate)
		
    	where HexData : IHasNeighbours<HexData>
		
		{
			//set of already checked HexData
   	 		var closed = new HashSet<HexData>();
			//queued HexData in open set
   			var queue = new PriorityQueue<double, Path<HexData>>();
			//start by adding enemy's hex to queue
    		queue.Enqueue(0, new Path<HexData>(start));
		
   			while (!queue.IsEmpty)
    		{
				//get first element on list
        		var path = queue.Dequeue();
				//check to see if this element is in the set of already checked hexes, i.o the closed set 
       			if (closed.Contains(path.LastStep))
            		continue;
				//check to see if this element is our destination hex
        		if (path.LastStep.Equals(destination))
           			return path;
				//if element isn't the destination hex and isn't in closed set, add it to closed set
        		closed.Add(path.LastStep);
        	
				//Go through neighbors (adjacent hexes) of current element
				foreach(HexData n in path.LastStep.Neighbours)
        		{
					//compute distance between current element and it's neighbor
            		double d = distance(path.LastStep, n);
					//New step added without modifying current path
            		var newPath = path.AddStep(n, d);
					//add new path to queue
            		queue.Enqueue(newPath.TotalCost + estimate(n), newPath);
        		}
    	}
    	return null;
	}
	
	//TODO: SEE ALL BELOW*********************************************
	
	//Change neighbor interface to account for adjacent hexes or
	//call a method that gets the adjacent hex for calling entity (enemy.getTraversable hexes(hex))
	//neighbor method()
	
	
	//Return distance between two adjacent nodes
	double calcDistance(HexData hex, HexData hex2)
    {
        return Mathf.Max(deltaX, deltaY, deltaZ);
    }
	
	//Need a estimate function
    //Return distance between any node and destination node
    double calcEstimate(HexData hex)
    {
		//TODO: this is dummy code, will change later
        HexData destTile = destTileTB.hex;
        //Formula used here can be found in Chris Schetter's article
        float deltaX = Mathf.Abs(destTile.X - hex.X);
        float deltaY = Mathf.Abs(destTile.Y - hex.Y);
        int z1 = -(hex.X + hex.Y);
        int z2 = -(destTile.X + destTile.Y);
        float deltaZ = Mathf.Abs(z2 - z1);

        return Mathf.Max(deltaX, deltaY, deltaZ);
    }
	
	//TODO: method for extracting hexes from path
	/*
	 // if (this.path == null){I THINK THIS mean there isn't a traversable path to base, so account for this in case it happens}
	 //use IEnumerable<Tile> path, that is returned by FindPath
	 foreach (HexData hex in path)
        {
            var line = (GameObject)Instantiate(Line);
            //calcWorldCoord method uses squiggly axis coordinates so we add y / 2 to convert x coordinate from straight axis coordinate system
            Vector2 gridPos = new Vector2(tile.X + tile.Y / 2, tile.Y);
            line.transform.position = calcWorldCoord(gridPos);
            this.path.Add(line);
            line.transform.parent = lines.transform;
        }
	 */
}
