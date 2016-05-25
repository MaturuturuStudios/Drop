using System.Collections.Generic;

/// <summary>
/// Defines a reorderable list which will be drawn
/// different form the other lists on the editor.
/// </summary>
public class ReorderableListAttribute {
	// This class is empty. Only it's signature is needed since
	// custom property drawers won't work with generic classes
}

/// <summary>
/// Implements the funcionable of ReorderableListAttribute
/// and defines a generic reorderable list.
/// Particular lists will need to extend this class with the
/// right parameter in order for the drawer to work.
/// </summary>
/// <typeparam name="T">The type of the elements stored in the list</typeparam>
public class ReorderableList<T> : ReorderableListAttribute {

	/// <summary>
	/// Wrapped list. Used since the drawer won't allow the same
	/// object as a list.
	/// </summary>
	public List<T> list;

	public int Length { get { return list.Count; } }

	/// <summary>
	/// Returns this object as a list.
	/// </summary>
	/// <returns>This object as a list</returns>
	public List<T> AsList() {
		return this;
	}

	/// <summary>
	/// Implicit conversor to use this object as a list.
	/// </summary>
	/// <param name="item">The item to convert</param>
	public static implicit operator List<T>(ReorderableList<T> item) {
		return item.list;
	}

	/// <summary>
	/// Implicit operator brackets to directly access the list.
	/// </summary>
	/// <param name="i">The index to access</param>
	public T this[int i] {
		get { return list[i]; }
		set { list[i] = value; }
	}
}