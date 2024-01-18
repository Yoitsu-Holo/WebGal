该目录下放置脚本可以调用系统功能

目前将其分为两个部分：

- EngineNative
- EngineExtend

# EngineNative

这里存放传统的调用方式，其包含以下几个方法：

- read
- write
- ctl

其中 read 操作表示从该结构中读取数据，write 操作表示向结构中写入数据，ctl 操作表示控制结构的行为

整体设计仿造 Linux 驱动，参考一切皆文件的思想，并且任意结构不需要open和close，只要在其生命周期内，其均为open，生命周期外均不存在该对象

在这里的任何代码，需要实现一个 Regist 接口，包含且仅一个包含 EngineCallManager 对象的构造函数，向引擎注册这个驱动的行为，以便在脚本中调用

# EngineExtend

这里存放扩展调用，也是对于传统调用的扩展，例如播放音频。

在这里的任何代码，同样需要实现一个 Regist 接口，包含且仅一个包含 EngineCallManager 对象的构造函数

# 推荐设计

由于在脚本中过多代码会使得解释器的压力增加，而使用 EngineNative 的方式不可避免的会造成执行效率下降，所以我们推荐逻辑全部使用 EngineExtend 方式来实现脚本调用。而 EngineNative 更加偏向于引擎底层的应用和逻辑。

# 注意事项

这里驱动层是属于引擎，而非脚本解释器。故不需要任何脚本解释的代码。

而脚本解释器为了能够访问这些驱动接口，必须将脚本进行转发，并且脚本引擎只能访问 EngineCallManager 来获取。