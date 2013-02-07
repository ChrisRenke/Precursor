using UnityEngine; 
using System.Collections; 
using System.Collections.Generic;
using System;
using System.IO; 


public class hexManagerS : MonoBehaviour {

	
		/*
		  		north 
      northwest  ___   northeast
                /    \
      southwest \___/  southeast
				south
		*/
 
	public static string 	  	level_name;
	public static int   		level_editor_format_version;
	
	private static HexData[,] 	hexes; 
	private static int 			x_max, z_max = 0; //size of hex array, used for out of bounds checking
	
	
	//used for parsing level files
	private	string[] stringSeparators = new string[] {" = "};  
	

	void Start(){

		if(!Load())
			throw new MissingComponentException("Level file malformed! : (");

	}
	
	
	private bool Load(){	
		
		
		
	//INITIALIZE FILE TO READ\
		StreamReader reader;
		FileInfo filer = new FileInfo(Application.dataPath + "/Level_Files/" + level_name + ".pcl");
		if(filer != null && filer.Exists)
		{
		   reader = filer.OpenText();  // returns StreamReader
		} 
		else
		{
			print ("FILE DOES NOT EXIST!");
			return false;
		}
		  	
	//BEGIN PARSING DATA
		//PARSE HEADER INFO
		if(!reader.ReadLine().Equals("LEVEL{"))
		{
			print("l1 ILL FORMATED!");
			return false;
		} 
		if(getIntR(reader) != level_editor_format_version) //EDITOR VER
		{
			print ("EDITOR VERSION MISMATCH!");
			return false;
		}
		level_name 		= getStringR(reader); //NAME
		int version    		= getIntR(reader);    //VERSION
		
		int total_count, game_count, border_count;
		x_max = getIntR(reader);
		z_max = getIntR(reader);
		
		hexes = new HexData[x_max, z_max];
		
		--x_max;
		--z_max;
		
		total_count 	= getIntR(reader);  		//total count
		game_count 		= getIntR(reader);		//game count
		border_count 	= getIntR(reader);  		//border count
		
		
		//BEGIN PARSING HEXES
		if(!reader.ReadLine().Contains("HEXES{"))
		{
			print("HEXES ILL FORMATED");
			return false;
		}
		while(getHexR(reader)) //next line is a HEX{
		{
			int x = getIntR(reader);
			int z = getIntR(reader);
            Hex hex_type = (Hex) Enum.Parse(typeof(Hex), getStringR(reader));
			hexes[x, z] = new HexData(x, z, hex_type);
			
			if(!getCBR(reader))
			{
				print("MALFORMED HEX!");
				return false;
			}
		}
		 
//		print (reader.ReadLine());
		//BEGIN PARSING ENTITES
//		if(!reader.ReadLine().Contains("ENTITIES{"))
//		{
//			print("ENTITIES ILL FORMATED");
//			return false;
//		}
//		while(getEntR(reader)) //next line is a HEX{
//		{
//			int x = getIntR(reader);
//			int z = getIntR(reader);
//            editorEntityManagerS.Entity ent_type = (editorEntityManagerS.Entity) Enum.Parse(typeof(editorEntityManagerS.Entity), getStringR(reader));
//			
//			editorUserS.ems.base_starting_health_percentage = 100;
//			editorUserS.ems.mech_starting_health_percentage = 100;
//			editorUserS.ems.enemy_knows_base_loc 			= false;
//			editorUserS.ems.enemy_knows_mech_loc 			= false;
//			editorUserS.ems.node_starting_level				= 2;
//			
//			switch(ent_type)
//			{
//				case editorEntityManagerS.Entity.Base:
//					editorUserS.ems.base_starting_health_percentage = getIntR(reader);
//					break;
//				case editorEntityManagerS.Entity.Player:
//					editorUserS.ems.mech_starting_health_percentage = getIntR(reader);
//					break;
//				case editorEntityManagerS.Entity.Enemy:
//					editorUserS.ems.enemy_knows_base_loc = getBoolR(reader);
//					editorUserS.ems.enemy_knows_mech_loc = getBoolR(reader);
//					break;
//				case editorEntityManagerS.Entity.Factory:
//				case editorEntityManagerS.Entity.Junkyard:
//				case editorEntityManagerS.Entity.Outpost:
//					editorUserS.ems.node_starting_level	 = getIntR(reader);
//					break;
//			}
//			 
//			editorUserS.ems.LoadEntity(ent_type, x, z);
//			if(!getCBR(reader))
//			{
//				print("MALFORMED HEX!");
//				return false;
//			}
//		}
		 
		return true;
	}
	
	public string getStringR(StreamReader reader)
	{   
    	string[] items = reader.ReadLine().Split(stringSeparators, StringSplitOptions.None);
		return items[1];
	}
	
	public int getIntR(StreamReader reader)
	{  
    	string[] items = reader.ReadLine().Split(stringSeparators, StringSplitOptions.None);
		return int.Parse(items[1]);
	}
	
	public bool getBoolR(StreamReader reader)
	{  
    	string[] items = reader.ReadLine().Split(stringSeparators, StringSplitOptions.None);
		return bool.Parse(items[1]);
	}
	
	public bool getCBR(StreamReader reader) //close bracket Reader
	{  
    	return reader.ReadLine().Contains("}"); 
	}
	
	public bool getOBR(StreamReader reader) //close bracket Reader
	{  
    	return reader.ReadLine().Contains("{"); 
	}
	
	public bool getHexR(StreamReader reader) //close bracket Reader
	{  
    	return reader.ReadLine().Contains("HEX{"); 
	}
	
	public bool getEntR(StreamReader reader) //close bracket Reader
	{  
    	return reader.ReadLine().Contains("ENTITY{"); 
	} 

	//Return adjacent hexes for the given entity position
	public static HexData[] getAdjacentHexes(int x, int z){
		
		
		//if we're out of bounds or if we're trying to get adjacency from a perimeter hex, return false
		if(x < 0 || x > x_max || z < 0 || z > z_max || getHex(x, z).hex_type == Hex.Settlement)
		{
			throw new KeyNotFoundException("Accessing out of bounds!"); 
		}
		
		//ALWAYS BUILD THIS ARRAY "CLOCKWISE" STARTING AT NORTH
		HexData[] output = new HexData[6];

		//Get North						//Get Northeast
		output[0] = hexes[x, z +1];		output[1] = hexes[x+1, z];

		//Get Southeast                 //Get South
		output[2] = hexes[x+1, z-1];	output[3] = hexes[x, z-1]; 

		//Get Southwest					//Get Northwest
		output[4] = hexes[x-1, z];		output[5] = hexes[x-1, z+1];
		
		return output;
	}
	
	
	
	//Get hex at given position in the map
	public static HexData getHex(int x, int z){
		
		if(x < 0 || x > x_max || z < 0 || z > z_max || getHex(x, z).hex_type == Hex.Settlement)
		{
			throw new KeyNotFoundException("Accessing out of bounds!"); 
		}
		else
		{ 
			return hexes[x, z];
		}
	} 
}


 
public class HexData{ 
	
	public readonly int 	x;   	  //level x coord (NE / SW)
	public readonly int 	z;  	  //level z coord  (N / S)
	public readonly Hex 	hex_type; //enviroment type of this hex
	
	public HexData(int _x, int _z, Hex _type){
		x = _x;
		z = _z; 
		hex_type = _type;
	}
} 

