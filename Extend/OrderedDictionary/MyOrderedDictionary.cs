using System.Collections;

namespace WebGal.Extend.Collections;

public class NamedDictionary<TValue> : IEnumerator<TValue>
{
	private readonly SortedDictionary<int, TValue> _data = [];
	private readonly Dictionary<string, int> _smap = [];
	private readonly Dictionary<int, string> _imap = [];

	private SortedDictionary<int, TValue>.Enumerator _enum;


	#region IEnumerator
	public TValue Current
	{
		get
		{
			if (_data.GetEnumerator().Current.Value is null)
				throw new NullReferenceException();
			return _data.GetEnumerator().Current.Value;
		}
	}

#pragma warning disable CS8603 // 可能返回 null 引用。
	object IEnumerator.Current => Current; // 已经经过检测

#pragma warning restore CS8603 // 可能返回 null 引用。

	public void Dispose() { }

	public bool MoveNext() => _enum.MoveNext();

	public void Reset() => _enum = _data.GetEnumerator();
	#endregion
}