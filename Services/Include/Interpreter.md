`version 0.0.1 Alpha`

# 自研脚本功能文档

暂定名称为 moe script

文件形式为 `file.moe`

# 主加载

参考 linux 下 ELF 内存分段
将基础脚本分为


| 名称 | 含义 |
|--|--|
| .table | 程序所有文件的名称、类型和url路径对应表 |
| .data | 全局变量部分，可以通过修饰符来定义常量或变量 |
| .form | 界面描述文件，主要通过json格式文件来注册 |
| .start | 程序开始位置，相当于脚本的main函数位置 |

注意，所有的资源路径必须在程序被初始化时被设置，不接受任何中途加载的资源路径，包括音频，图像

## table

基本格式：

```text
name type path
```

## data

基本格式：

```text
[const|static|var] [int|string|float] name
```

例如定义一个全局静态整形变量 A，全局常量字符串 S，全局浮点变量 F，格式如下：

```
static int A
const string S
var float F
```

为保证代码简单性，脚本只提供三种基本类型（与 C# 类型严格对应）：

| 类型 | 说明 |
|-|-|
| int | 32位带符号整数 |
| string | 字符串，不限制长度，UTF-8编码 |
| float | 32位浮点数 |

这里不建议存储字符量，如果需要，请只用string类型存储

## form

这里用作界面描述文件的存储，文件必须在这里被定义

## start

指向一个函数，程序会从这个函数开始运行

# 脚本语法

## 函数定义

```text
func [return] name ([parameter], ...):
	...
```

需要注意的是，任何流程都是一个函数，每一个函数都可以使用 call 来调用其他函数，syscall 来访问游戏引擎

同时，游戏引擎为了方便代码书写，降低重复度，加入一些预定义函数作为语法糖

| 预定义函数(关键字) | 功能 |
| say | 显示文字 |
| stand | 立绘 |
| backgroud | 背景，CG |
| charactor | 角色头像 |
|  |  |
| bgm | 背景音乐 |
| voice | 语音 |
|  |  |
| play | 播放缓冲区中所有 |
| pause | 包含play功能，等待鼠标单击事件 |
| delay | 延迟一段时间，ms单位 |