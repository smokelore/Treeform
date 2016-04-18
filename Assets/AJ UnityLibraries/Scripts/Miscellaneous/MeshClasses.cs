using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public struct Vertices : System.IEquatable<Vertices> {
	// Vertex index, owner id
	public Tuple<int, int>[] vertices;

	public int Length {
		get { return vertices.Length; }
	}
	
	public Vertices(Tuple<int, int> a, Tuple<int, int> b, Tuple<int, int> c){
		vertices = new Tuple<int, int>[3];
		vertices[0] = a;
		vertices[1] = b;
		vertices[2] = c;
	}
	
	public Vertices(int a, int b, int c, int owner){
		vertices = new Tuple<int, int>[3];
		vertices[0] = new Tuple<int, int>(a,owner);
		vertices[1] = new Tuple<int, int>(b,owner);
		vertices[2] = new Tuple<int, int>(c,owner);
	}
	
	public Tuple<int, int> this[int index]{
		get { return vertices[index]; }
		set { vertices[index] = value;}
	}
	
	public int[] ToArray(){
		int[] verts = new int[3];
		for (int i = 0; i < 3; i++){
			verts[i] = vertices[i].First;
		}
		return verts;
	}
	
	public bool Equals (Vertices other){
		int common = 0;
		for (int i = 0; i < vertices.Length; i++){
			for (int j = 0; j < other.vertices.Length; j++){
				if (vertices[i].First == other.vertices[j].First && vertices[i].Second == other.vertices[j].Second){
					common++;
					break;
				}
			}
		}
		return common == 3;
	}
	
	public override int GetHashCode (){
		return hashcode();
	}

	int hashcode(){
		int hash = 23;
		foreach (Tuple<int, int> i in vertices){
			hash = hash * 31 + i.First;// + i.Second.ToString().GetHashCode();
		}
		return hash;
	}

	public void SetOwner(int owner){
		for (int i = 0; i < vertices.Length; i++){
			vertices[i].Second = owner;
		}
	}
}

public class Side : System.IEquatable<Side> {
	public Tuple<int,int>[] vertices;
	public Triangle[] triangles;
	int triangleCount;

	public Side(int a, int b, int owner){
		vertices = new Tuple<int, int>[2];
		vertices[0] = new Tuple<int, int>(a, owner);
		vertices[1] = new Tuple<int, int>(b, owner);
		triangles = new Triangle[2];
		triangleCount = 0;
	}
	
	public Side(Tuple<int, int> a, Tuple<int, int> b){
		vertices = new Tuple<int, int>[2];
		vertices[0] = a;
		vertices[1] = b;
		triangles = new Triangle[2];
		triangleCount = 0;
	}

	public int[] ToArray(){
		int[] verts = new int[2];
		int i = 0;
		foreach (Tuple<int,int> vert in vertices){
			verts[i] = vert.First;
			i++;
		}
		return verts;
	}

	public void AddTriangle(Triangle t){
		triangles[triangleCount] = t;
		triangleCount++;
	}
	
	public bool HasOpenTriangle(){
		return triangleCount < 2;
	}

	public bool Equals (Side other){
		int common = 0;
		for (int i = 0; i < 2; i++){
			for (int j = 0; j < 2; j++){
				if (vertices[i].First == other.vertices[j].First){
					common++;
					break;
				}
			}
		}
		return common == 2;
	}

	public Tuple<int, int> CommonVertex(Side other){
		Tuple<int, int> common = new Tuple<int, int>(-1, -1);
		foreach (Tuple<int,int> vert in vertices){
			foreach (Tuple<int, int> otherVert in other.vertices){
				if (vert.First == otherVert.First){
					common = vert;
					break;
				}
			}
			if (common.First >= 0){
				break;
			}
		}
		return common;
	}
	
	public Tuple<int, int> UncommonVertex(Side other){
		Tuple<int, int> common = new Tuple<int, int>(-1, -1);
		foreach (Tuple<int,int> vert in vertices){
			bool found = true;
			foreach (Tuple<int, int> otherVert in other.vertices){
				found = found && !vert.Equals(otherVert);
			}
			if (found){
				common = vert;
				break;
			}
		}
		return common;
	}
	
	public Tuple<int, int> OtherVertex(Tuple<int, int> vertex){
		foreach (Tuple<int, int> vert in vertices){
			if (!vert.Equals(vertex)){
				return vertex;
			}
		}
		return new Tuple<int, int>(-1,-1);
	}
	
	public override int GetHashCode (){
		return hashcode();
	}

	int hashcode(){
		int hash = 23;
		for (int i = 0; i < 2; i++){
			hash = 31 * hash + vertices[i].First;// + i.Second.ToString().GetHashCode();
		}

		int hash2 = 23;
		for (int i = 1; i >= 0; i--){
			hash2 = 31 * hash2 + vertices[i].First;// + i.Second.ToString().GetHashCode();
		}
		return hash + hash2;
	}
}

public class Triangle {
	public HashSet<Triangle> Adjacent;
	public Vertices vertices;
	public Side[] sides;
	
	public Triangle(int a, int b, int c, int owner){
		this.vertices = new Vertices(a, b, c, owner);
		this.sides = new Side[3];
		Adjacent = new HashSet<Triangle>();
	}
	
	public Triangle(Side s, Tuple<int, int> vert) : this(s, vert.First, vert.Second){}

	public Triangle(Side s, int vert, int owner){
		Vertices v = new Vertices(0,0,vert,owner);
		int i = 1;
		foreach (Tuple<int,int> vertex in s.vertices){
			v.vertices[i] = vertex;
			i--;
		}

		this.vertices = v;
		this.sides = new Side[3];
		Adjacent = new HashSet<Triangle>();
	}

	public Triangle(Vertices v){
		this.vertices = v;
		this.sides = new Side[3];
		Adjacent = new HashSet<Triangle>();
	}

	public void FindOrCreateSides(Dictionary<Side, Side> sides){
		Side s = new Side(vertices.vertices[0], vertices.vertices[1]);
		if (sides.ContainsKey(s)){
			s = sides[s];
		}
		else {
			sides.Add(s,s);
		}
		this.sides[0] = s;
		this.sides[0].AddTriangle(this);

		s = new Side(vertices.vertices[1], vertices.vertices[2]);
		if (sides.ContainsKey(s)){
			s = sides[s];
		}
		else {
			sides.Add(s,s);
		}
		this.sides[1] = s;
		this.sides[1].AddTriangle(this);
		
		s = new Side(vertices.vertices[2], vertices.vertices[0]);
		if (sides.ContainsKey(s)){
			s = sides[s];
		}
		else {
			sides.Add(s,s);
		}
		this.sides[2] = s;
		this.sides[2].AddTriangle(this);

		for (int i = 0; i < 3; i++){
			Side current = this.sides[i];
			for (int j = 0; j < current.triangles.Length; j++){
				Triangle t = current.triangles[j];
				if (t != null && t != this){
					t.Adjacent.Add(this);
					Adjacent.Add(t);
				}
			}
		}
	}
	
	public Tuple<int, int> OtherVertex(Side side){
		Tuple<int, int> other = new Tuple<int, int>(-1,-1);
		for (int i = 0; i < 3; i++){
			bool found = true;
			foreach (Tuple<int,int> vert in side.vertices){
				found = found && !vert.Equals(vertices.vertices[i]);
			}
			if (found){
				other = vertices.vertices[i];
				break;
			}
		}
		return other;
	}
	
	public bool HasAdjacent(Side side){
		return !side.HasOpenTriangle();
	}
	
	public Triangle GetAdjacent(Side side){
		foreach (Triangle t in side.triangles){
			if (t != this){
				return t;
			}
		}
		return null;
	}
}
