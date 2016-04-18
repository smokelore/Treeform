using System.Collections;

[System.Serializable]
public struct Tuple<T,V> {
	public T First;
	public V Second;

	public Tuple(T first, V second){
		this.First = first;
		this.Second = second;
	}

	public override bool Equals (object obj)
	{
		Tuple<T,V> other = (Tuple<T,V>) obj;
		return other.First.Equals(First) && other.Second.Equals(Second);
	}

	public static bool operator ==(Tuple<T,V> a, Tuple<T,V> b){
		return a.Equals(b);
	}

	public static bool operator !=(Tuple<T,V> a, Tuple<T,V> b){
		return !a.Equals(b);
	}

	public override int GetHashCode (){
		return 31 * (23 * 31 + First.GetHashCode()) + Second.GetHashCode();
	}
}
