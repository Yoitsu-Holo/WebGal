using WebGal.API;
using WebGal.API.Data;
using WebGal.Global;
using WebGal.Types;

namespace WebGal.MeoInterpreter;

public partial class MoeInterpreter
{
	public partial class Syscall
	{
		/// <summary>
		/// 打印一个 MoeVariable 变量
		/// </summary>
		public static void Log(MoeVariable variable)
		{
			string s = "";
			s += $"\t{variable}";
			if (variable.Type == MoeVariableType.Int || variable.Type == MoeVariableType.Double)
				for (int i = 0; i < variable.Size; i++)
					s += $"{(i % 5 == 0 ? "\n" : "")}\t\tobj[{i}]: {variable[i]}";
			else if (variable.Type == MoeVariableType.String)
				for (int i = 0; i < variable.Size; i++)
					s += $"{(i % 5 == 0 ? "\n" : "")}\t\tobj[{i}]: {variable[i]}";
			Logger.LogInfo(s, Global.LogLevel.Info);
		}

		/// <summary>
		/// 加载一个剧本
		/// </summary>
		/// <param name="opera"> string: 剧本名称 </param>
		public static void LoadOpera(MoeVariable operaID)
		{
		}

		/// <summary>
		/// 加载当前剧本的场景
		/// </summary>
		/// <param name="sceneID"> int: 场景ID </param>
		public static void LoadScene(MoeVariable sceneID)
		{

		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="LayoutID"> int: </param>
		public static void RegLayout(MoeVariable layout)
		{
			LayoutInfo layoutInfo = new()
			{
				LayoutID = layout,
			};
			Driver.RegisterLayout(layoutInfo);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="LayoutID"> int: </param>
		public static void SetLayout(MoeVariable layout)
		{
			LayerIdInfo layoutInfo = new()
			{
				LayoutID = layout,
			};
			Driver.SetActiveLayout(layoutInfo);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="LayoutID"> int: </param>
		/// <param name="LayerID"> int: </param>
		/// <param name="LayerType"> string: 图层类型</param>
		public static void RegLayer(
			MoeVariable layout, MoeVariable layer, MoeVariable type,
			MoeVariable posx, MoeVariable posy, MoeVariable width, MoeVariable height
		)
		{
			LayerBox layerBox = new()
			{
				Attribute = new()
				{
					ID = new() { LayoutID = layout, LayerID = layer, },
					Type = type,
					Position = new() { X = posx, Y = posy },
					Size = new() { X = width, Y = height },
				}
			};
			Driver.RegisterLayer(layerBox);
		}

		public static void SetImageBox(
			MoeVariable layout, MoeVariable layer,
			MoeVariable image,
			MoeVariable subx, MoeVariable suby, MoeVariable width, MoeVariable height
		)
		{
			ImageBoxInfo imageBox = new()
			{
				ID = new() { LayoutID = layout, LayerID = layer, },
				Image = new()
				{
					ImageName = image,
					SubRect = new() { X = subx, Y = suby, W = width, H = height },
				},
			};
			Driver.SetImageBoxInfo(imageBox);

		}

		public static void SetTextBox(
			MoeVariable layout, MoeVariable layer,
			MoeVariable text, MoeVariable font, MoeVariable size
		)
		{
			TextBoxInfo textBox = new()
			{
				ID = new() { LayoutID = layout, LayerID = layer, },
				Text = text,
				Font = font,
				FontSize = size,
			};
			Driver.SetTextBoxInfo(textBox);
		}

		public static void SetColorBox(
			MoeVariable layout, MoeVariable layer,
			MoeVariable a, MoeVariable r, MoeVariable g, MoeVariable b
		)
		{
			ColorBoxInfo colorBox = new()
			{
				ID = new() { LayoutID = layout, LayerID = layer, },
				A = (byte)a,
				R = (byte)r,
				G = (byte)g,
				B = (byte)b,
			};
			Driver.SetColorBoxInfo(colorBox);
		}
	}
}
