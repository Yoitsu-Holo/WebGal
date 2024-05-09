using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace WebGal.Extend.Collections;

public interface IExtendEnumerator<T> : IEnumerator<T>
{
	bool MovePrevious();
	bool TryGetNext([MaybeNullWhen(false)] out T? result);
	bool TryGetPrevious([MaybeNullWhen(false)] out T? result);
	bool IsEnd { get; }
	int Size { get; }
	int Position { get; }
}

public class DoubleEnumerator<T>(List<T> list) : IExtendEnumerator<T>
{
	private readonly List<T> _list = list;
	private int _position = -1;


	public bool MoveNext()
	{
		_position++;
		return _position < _list.Count;
	}

	public bool MovePrevious()
	{
		_position--;
		return _position >= 0;
	}

	public bool TryGetNext([MaybeNullWhen(false)] out T? result)
	{
		if (_position < _list.Count - 1)
		{
			result = _list[_position + 1];
			return true;
		}
		result = default;
		return false;
	}

	public bool TryGetPrevious([MaybeNullWhen(false)] out T? result)
	{
		if (_position > 0)
		{
			result = _list[_position + 1];
			return true;
		}
		result = default;
		return false;
	}

	public T Current
	{
		get
		{
			if (_position >= 0 && _position < _list.Count)
				return _list[_position];
			else
				throw new IndexOutOfRangeException();
		}
	}

	public bool IsEnd => _position == _list.Count;

	// 下面是实现 IEnumerator<T> 接口所必需的成员
	public int Size => _list.Count;
	public int Position => _position;

#pragma warning disable CS8603 // 可能返回 null 引用。
	object IEnumerator.Current => Current; // 已经经过检测



#pragma warning restore CS8603 // 可能返回 null 引用。

	public void Reset() { _position = -1; }

	public void Dispose() { }
}