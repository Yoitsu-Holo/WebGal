# WebGal

# 更新日志

## V0.0.2(beta)

更改渲染逻辑

Render 将对 Scene 中的每一个 Layer 调用 NextFrame(timeoff) 方法来渲染下一帧，并且通过 (SKPoint)FramePosition 和 (SKBitMap)FrameBuffer 来获取要在 Redner 中绘制显示的帧信息。

Layer 中的 NextFrame(timeoff) 方法会根据初始化的 animation (`todo`) 来渲染下一帧。

## V0.0.1(beta)

实现界面渲染