/// UnityUtils https://github.com/mortennobel/UnityUtils
/// By Morten Nobel-Jørgensen
/// License lgpl 3.0 (http://www.gnu.org/licenses/lgpl-3.0.txt)


using UnityEngine;
using System.Collections;
using System.Collections.Generic;


/// Interface for a shortest path problem
public interface IShortestPath<HexData,Action> {

/// Should return a estimate of shortest distance. The estimate must me admissible (never overestimate)
float Heuristic(HexData fromLocation, HexData toLocation);

/// Return the legal moves from a state
List<HexData> Expand(HexData position);

/// Return the actual cost between two adjecent locations
float ActualCost(HexData fromLocation, Action action);

/// Returns the new state after an action has been applied
HexData ApplyAction(HexData location, Action action);
}
