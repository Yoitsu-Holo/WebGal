# 脚本语法

为简化代码编写，代码块使用花括号区分

需要注意的一点，此为解释型语言，仅会依靠标志性提示词来解析当前语句，如遇到语法错误，如括号未封闭，则可能引发未定义的错误

## 变量定义

脚本程序可以，也只能在函数任意地方定义变量

如果需要全局访问，请将变量放置到主加载文件的 data 区中

与全局变量又区别的是，函数中变量只能为`变量`和`引用`，也就是 `var`和`ref` 类型，且在函数执行完成后自动销毁，与类C函数类似，变量只能在声明或者定义后才可被使用，否者将抛出错误

**变量定义语法如下**

```
[var|ref] [int|string|double|dictionary] name
```

例如定义局部整型变量 A，字符串变量 S，浮点变量 F，字典类型 D

```
var int A
var string S
var double F
var dictionary D
```

**数组定义方式如下**

```
var [int|string|double|dictionary] name [ '[' <number> : ... ']' ]
```

例如定义一维和二维整型数组（大小为9和9*9）

```
var int A[9]
var int B[9:9]
```

对于多维变量访问同理

```
A[5]
B[2:1]
```

## 函数定义

```text
func [return] [name] ( [parameter], ... )
{
	...
}

func int func1 ( var int a,ref int b)
{
	...
}
```

- [return] : 返回值
- [name] : 函数名称
- [paramater] : 参数列表，只能传入引用或者单变量，不支持数组传递

需要注意，没有返回值返回类型应该写入void，而不应留空

例如

```text
func int func1 (var int a,var int b)
{
	var int c
	c=a+b
	return c
}

func void func2()
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
| foreground | 立绘 |
| background | 背景，CG |
| charactor | 角色头像 |
|  |  |
| 简单音频功能 |  |
| bgm | 背景音乐，默认循环，有淡入淡出效果 |
| voice | 语音，默认单次 |
| audio | 基本音頻，bgm和voice由该基本api接口封装 |
|  |  |
| 简单系统功能 |  |
| choice | 选项 |
| animation |  |
| 自动化 |  |
| play | 播放缓冲区中所有 |
| pause | 包含play功能，等待鼠标单击事件 |
| delay | 延迟一段时间，ms单位 |

## 结构语句

一下包含一些基础的条件语句

基本语法类似C/C++，但为了效用精简，只保留了最小集合

条件分支：只保留 `if ... else if ... else` 结构

循环分支：只保留 `while` 循环，保留 `continue`、`break` 关键字

任意跳转：保留 `goto` 语法，添加标签定义语法 `lable`

### 条件分支

```c++
if ( exp )
{
	...
}
elif( exp )
{
	...
}
else
{
	...
}
```

### 循环分支

```c++
while ( exp )
{
	continue;
	break;
}
```

## 运算符

### 赋值运算

| 运算符 | 作用 |
|---|---|
| = | 浮点数转到整数会截断小数部分。<br>字符串->数：尝试转化，不能返回0。<br>数->字符串：整数转化为对应字符串，浮点数保留六位小数 |

### 数学四则运算

| 运算符 | 作用 |
|---|---|
| + | 两数相加，整数 -> 浮点数。字符串拼接，整数转化位对应字符串，浮点数保留六位小数 |
| - | 两数相减，整数 -> 浮点数。|
| * | 两数相乘，整数 -> 浮点数。|
| / | 两数相除，整数只能得到整数，若其中之一未浮点则自动转化为浮点数|

### 逻辑运算

只对于整数有效

| 运算符 | 作用 |
|---|---|
| ~ | 按位取反 |
| \| | 按位或 |
| & | 按位与 |
| ^ | 按位异或 |
| << | 左移 |
| >> | 右移 |

### 条件运算

| 运算符 | 作用 |
|---|---|
| \|\| | 条件或 |
| && | 条件与|

### 扩展

| 运算符 | 作用 |
|---|---|
| ^^ | 乘方 |

## 表达式

表达式分为两种，分别为：

1. 左值：数学表达式（由变量组合成的表达式）
2. 右值：函数表达式（由函数返回的数值）

其中右值一般作为函数返回值存在，不能参与任何数学运算（包括和右值运算），仅能向左值赋值。

例如：

```c++
func int ret1 ()
{
	return 1;
}
```

正确获取右值的方式（唯一方式）：

```c++
// 使用变量保存，自动申请空间
var int a = ret1();

// 不适用任何变量保存，自动丢弃右值
ret1();
```

错误获取右值的方式（部分例子）：

```c++
// 右值不能与左值参与运算
var int a = ret1() + 1;

// 右值不能与右值参与运算
var int a = ret1() + ret1();

// 函数参数只接收左值
print(ret1());
```

### 复杂表达式

解释器不支持复杂表达式求值，只会按照从左到右顺序求值（无运算符优先级），这样会节约性能，减少函数处理和调用（其实就是懒得做）

如：

```text
1 + 1 * 2 => 4

过程如下：
( 1 + 1 ) * 2
( 2 * 2 )
4
```

## 内置函数

所有的内置函数都会返回一个执行器id，可以通过这个id来绑定多个动作同时执行（play）

如果不绑定play动作，那么所有动作都将不会执行，且占用注册的内存空间（运行后函数就销毁）

### 基础设置

#### tag() 标记 [todo]

标记当前的状态机状态，可以使用该方法来回顾历史文本和剧情

```c#
int tag(string tagName);
```

#### taskCreat() 创建一个任务

脚本中的任务概念类似子线程或者子进程，实现方式是新建一个函数栈

```c#
// 创建一个名称为taskName的计划，并且冲funcname开始执行
void taskCreat(string taskName, string funcName);
```

#### taskRun() 激活一个任务

运行一个指定的任务，即将活动函数栈切换到目标函数栈，并且展厅当前函数栈的执行

```c#
void taskRun(string taskName, object param [...]);
```

#### taskDispose() 销毁一个任务

销毁一个任务，并且清理其所有占用的资源（不能自己销毁自己）,在销毁后依然返回自己执行。

```c#
void taskDispose(string taskName);
```

### 图像控制

基础界面图像分为以下几个部分：

```txt
+-------------------------------------------+
|                                           |
|      bg       +----------+       bg       |
|               |     s    |                |
|               |     t    |                |
|               |          |                |
+-------+[name]-----------------------------|
|   c   |     text...................       |
|   h   |   text.....................       |
+--------------------[........menu........]-+
```

每一张图片在初始化时会被设置一个参考点，该参考点默认为图片中心点，但可以在加载文件中设置图片的参考点位置（任意坐标数值，甚至可以为负数）

#### say() 显示文字

将指定文字加载到文本框显示，有如下两个方法：

```c#
// 旁白，不包含人名，即直接在文本框中显示
int say(string text);

// 对话，包含人名，除了文本框，还有对应的名字
int say(string name,string text);
```

#### ch() 显示角色头像

将指定图片作为立绘显示在文本框旁边

```c#
// 显示立绘图像，默认参考点居中
int st(string imageName);
int st(string imageName, string imageMask);

// 显示立绘图像，指定参考点坐标
int st(string imageName, int X, int Y);
int st(string imageName, string imageMask, int X, int Y);
```

#### st() 显示角色立绘

将指定立绘显示在背景图片上方

```c#
// 显示立绘图像，默认参考点居中
int st(string imageName);
int st(string imageName, string imageMask);

// 显示立绘图像，指定参考点坐标
int st(string imageName, int X, int Y);
int st(string imageName, string imageMask, int X, int Y);
```

#### bg() 显示背景

设置游戏背景图片

```c#
// 显示背景图像，默认参考点居中
int st(string imageName);
int st(string imageName, string imageMask);

// 显示背景图像，指定参考点坐标
int st(string imageName, int X, int Y);
int st(string imageName, string imageMask, int X, int Y);
```

#### anime() 注册动画效果

todo：可以控制图片类的出场效果，例如位置和时间

### 声音控制

#### bgm() 背景音乐

循环播放一段音频

```c#
// 播放音频,，默认全音量
int bgm(string audioName);

// 播放音频，指定音量 [0,1]
int bgm(string audioName, double volume);
```

#### voice 语音

单次播放一段音频

```c#
// 播放音频,，默认全音量
int voice(string audioName);

// 播放音频，指定音量 [0,1]
int voice(string audioName, double volume);
```

#### audio

基本音頻，bgm和voice由该基本api接口封装

```c#
// 播放音频，设置音量，音频之间的延迟，循环次数
int voice(string audioName, double volume, int delay, int times);
```

- audioName： 一个音频数组，方法会循环播放这个列表的音乐
- volume： 音量，范围[0,1]，可以为一个数组，方法会循环使用数组的值（注意，此数组大小处应该尽量与audioName大小一致，除非你知道你在干什么）
- delay： 每一段音频之间的延迟，可以为负数，即前一段音频未播放完，后一段音频就加入同时播放
- times： 播放次数，注意，播放一段音乐为一次，而非整个数组的全部音乐为一次。`-1`表示无限循环

### 事件控制

#### choice() 选项

`注意`：这是一个中断事件，会暂停动画的执行

choice函数会更具选项的数量来自定规划选项位置，默认为水平居中，垂直尽量居中于窗口上边框和文字框下边框区域

```c#
// 显示选项
int choice(string choiceName);

// 显示选项，并且使用自定义的选项图片
int choice(string choiceName,string imageNormal,string imageHover, string imagePress);
```

- imageNormal: 通常状态的图片
- imageHover: 鼠标悬停的图片
- imagePressed: 鼠标按下的图片

### 自动化

#### play() 播放动作效果

播放缓冲区中效果

```c#
// 播放全部动作
void play();

// 播放数组中的动作
void play(int actionId);
```

#### end() 结束动作效果

强制结束动作效果，直接进入动作的最终状态

```c#
// 结束播放全部动作
void end();

// 结束播放数组中的动作
void end(int actionId);
```

#### pause() 暂停执行脚本 [todo]

等待程序事件 

#### delay() 延迟执行脚本

```c#
// 延迟一段时间，ms单位。若出现其他事件，直接终止延迟。
void delay(int time);
```
