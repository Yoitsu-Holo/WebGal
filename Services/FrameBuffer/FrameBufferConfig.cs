namespace WebGal.Services.Data;

public class FrameBufferConfig
{
	public List<string> LayerNames = new();

	public int Width { get; set; } = 1280;
	public int Height { get; set; } = 720;

	public void PushLayer(string name)
	{
		lock (this)
		{
			LayerNames.Add(name);
		}
	}

	public void SetResolution(int width, int height)
	{
		Width = width;
		Height = height;
	}
}