namespace WebGal.Types;

public record struct AudioSetting(
	bool Loop,      // 循环
	int TimeMs,     // 播放时间，毫秒
	float Volume   // 音量大小 [0,1]
);