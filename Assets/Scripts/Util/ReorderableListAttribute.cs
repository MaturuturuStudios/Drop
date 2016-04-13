using System.Collections.Generic;

public class ReorderableListAttribute {
	
}

public class ReorderableList<T> : ReorderableListAttribute {
	public List<T> list;

	public List<T> AsList() {
		return this;
	}

	public static implicit operator List<T>(ReorderableList<T> item) {
		return item.list;
	}
}