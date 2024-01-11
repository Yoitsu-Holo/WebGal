`version 0.0.1 Alpha`

# 自研脚本功能文档

暂定名称为 moe script

需要注意的一点，此为解释型语言，仅会依靠标志性提示词来解析当前语句，如遇到语法错误，如括号未封闭，则可能引发未定义的错误

文件形式为 `.moe` 后缀名

# 主加载

参考 linux 下 ELF 内存分段
将基础脚本分为


| 名称 | 含义 |
|--|--|
| .file | 程序所有文件的名称、类型和url路径对应表 |
| .data | 全局变量部分，可以通过修饰符来定义常量或变量 |
| .start | 程序开始位置，相当于脚本的main函数位置 |
| .table | 程序所有函数与文件对应关系，程序不会处理，仅作为标识 |
| .form | 界面文件，通过json文件来注册，程序不会处理，仅作为标识 |
| .code | 代码文件，通过moe文件来注册，程序不会处理，仅作为标识 |

注意，所有的资源路径必须在程序被初始化时被设置，不接受任何中途加载的资源路径，包括音频，图像

## file

基本格式：

```text
name type url
```

即名称、类型、URL的合集

注意，这里URL应该是资源的相对路劲URL，而非绝对路劲。完整路径为基本网址+相对路径来组成完整的游戏资源访问路径。

文件类型有如下几大类：

1. 图片
2. 音频
3. 脚本（文本/代码）
4. 界面（文本/代码）
5. 二进制

值得注意的是，对于文本文件，脚本(script)和界面(ui)文件默认为代码文件，会自动读取和分析

## table

需要注意的是，table不会被初始加载，这只是ELF文件的一个属性，当读取到一个新的脚本文件时，程序会自动加载该文件的函数表到table段中。即这是一个自动化的处理过程，代码编写者不需要关心

另外，对于代码的函数而言，任何函数都可以在任何位置被调用，任意函数名称都应该只出现一次，如果同一个函数名称出现多次，那么会在table预处理时抛出异常

## data

基本格式：

```text
[const|static|var] [int|string|double] name:[size]
```

例如定义一个全局静态整形变量 A，全局常量字符串 S，全局浮点变量 F，格式如下：

```
static int A
const string S
var double F
```

同时，一行可以定义多个变量，每个变量之间使用空格隔开（任意个数），数组名称后面紧跟冒号表示变量长度（数组大小）

例如定义三个整型全局变量A，B，C：

```text
var int A:1, B:1, C:1
```

同时，代码允许缺省数组长度定义，这样将会得到一个长度为1的数组（即单个变量）


为保证代码简单性，脚本只提供三种基本类型（与 C# 类型严格对应）：

| 类型 | 说明 |
|-|-|
| int | 32位带符号整数 |
| string | 字符串，不限制长度，UTF-8编码 |
| double | 64位浮点数 |

需要注意的是，这些基本类型都是定长数组，在定义时必须指定长度！

如果需要单个变量而不是数组，则定义长度为1即可，同时，对于任意数组，直接访问数组名将返回第一个第一个元素，也就是下标为0的元素

同时，对于其对应的数组，与类C语言数组定义方式相似

```text
var int A:10, B:20
```

注意，基本类型数组不支持动态扩容，仅能在定义时初始化大小

## form

这里用作界面描述文件的存储，文件必须在这里被定义

定义方式为直接指定在.file字段中加载的文件，即：

```text
filename
```

注意，任何文件都应该在file字段中被加载后才能使用

## start

指向一个函数，程序会从这个函数开始运行

# 脚本语法

为简化代码编写，代码块使用花括号区分


## 变量定义

脚本程序可以，也只能在函数任意地方定义变量

如果需要全局访问，请将变量放置到主加载文件的 data 区中

与全局变量又区别的是，函数中变量只能为`变量`，也就是 `var` 类型，且在函数执行完成后自动销毁，与类C函数类似，变量只能在声明或者定义后才可被使用，否者将抛出错误

变量定义语法如下

```
var [int|string|double] name
```

例如定义局部整型变量 A，字符串变量 S，浮点变量 F

```
var int A
var string S
var double F
```

## 函数定义

```text
func [return] [name] : [parameter], ...
{
	...
}
```

- [return] : 返回值
- [name] : 函数名称
- [paramater] : 参数列表

需要注意，没有返回值返回类型应该写入void，而不应留空

例如

```text
func int func1 :var int a,var int b
{
	var int c
	c=a+b
	return c
}

func void func2:
{
	return
}
```

对于参数列表，参考变量定义

需要注意的是，任何流程都是一个函数，每一个函数都可以使用 `funcName()` 来调用其他函数，syscall 来访问游戏引擎底层驱动接口层（仿造Linux系统调用）

同时，对于任何代码的执行流程，都应该和操作系统类似：

用户脚本 -> syscall -> 驱动程序 -> 游戏引擎

同时，游戏引擎为了方便代码书写，降低重复度，加入一些预定义函数作为语法糖

| 预定义函数(关键字) | 功能 |
|--|--|
| 简单显示功能 |  |
| say | 显示文字 |
| stand | 立绘 |
| backgroud | 背景，CG |
| charactor | 角色头像 |
|  |  |
| 简单音频功能 |  |
| bgm | 背景音乐，默认循环，有淡入淡出效果 |
| voice | 语音，默认单次 |
| audio | 基本音頻，bgm和voice由该基本api接口封装 |
|  |  |
| 简单系统功能 |  |
| play | 播放缓冲区中所有 |
| pause | 包含play功能，等待鼠标单击事件 |
| delay | 延迟一段时间，ms单位 |

## 结构语句

一下包含一些基础的条件语句

### 条件分支

基本语法，同C/C++

```
if ()
{

}
else if()
{

}
else
{
	
}
```