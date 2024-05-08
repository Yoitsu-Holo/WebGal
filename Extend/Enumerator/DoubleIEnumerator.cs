using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace WebGal.Extend.Collections;

public interface IExtendEnumerator<T> : IEnumerator<T>
{
	bool MovePrevious();
	bool TryGetNext([MaybeNullWhen(false)] out T? result);
	bool TryGetPrevious([MaybeNullWhen(false)] out T? result);
}

public class DoubleEnumerator<T>(List<T> list) : IExtendEnumerator<T>
{
	private readonly List<T> list = list;
	private int position = -1;


	public bool MoveNext()
	{
		position++;
		return position < list.Count;
	}

	public bool MovePrevious()
	{
		position--;
		return position >= 0;
	}

	public bool TryGetNext([MaybeNullWhen(false)] out T? result)
	{
		if (position < list.Count - 1)
		{
			result = list[position + 1];
			return true;
		}
		result = default;
		return false;
	}

	public bool TryGetPrevious([MaybeNullWhen(false)] out T? result)
	{
		if (position > 0)
		{
			result = list[position + 1];
			return true;
		}
		result = default;
		return false;
	}

	public T Current
	{
		get
		{
			if (position >= 0 && position < list.Count)
				return list[position];
			else
				throw new IndexOutOfRangeException();
		}
	}

	// 下面是实现 IEnumerator<T> 接口所必需的成员

#pragma warning disable CS8603 // 可能返回 null 引用。
	object IEnumerator.Current => Current; // 已经经过检测
#pragma warning restore CS8603 // 可能返回 null 引用。

	public void Reset() { position = -1; }

	public void Dispose() { }
}